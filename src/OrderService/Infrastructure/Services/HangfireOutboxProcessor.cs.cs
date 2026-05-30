using Hangfire;
using Microsoft.Extensions.Logging;
using MassTransit;
namespace Infrastructure.Services;
public class HangfireOutboxProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HangfireOutboxProcessor> _logger;
    private readonly int _batchSize = 20;
    private readonly int _maxRetryCount = 3;

    public HangfireOutboxProcessor(
        IServiceProvider serviceProvider,
        ILogger<HangfireOutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 0)] // We handle retries manually
    [JobDisplayName("Process Outbox Messages - Batch {0}")]
    public async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        // Get unprocessed messages
        var messages = await dbContext.OutboxMessages
            .Where(m => !m.Processed && m.RetryCount <= _maxRetryCount)
            .OrderBy(m => m.OccurredOn)
            .Take(_batchSize)
            .ToListAsync(cancellationToken);

        if (!messages.Any())
            return;

        _logger.LogInformation("Found {Count} outbox messages to process", messages.Count);

        foreach (var message in messages)
        {
            try
            {
                // Deserialize the integration event
                var eventType = Type.GetType(message.Type);
                if (eventType == null)
                {
                    _logger.LogWarning("Could not resolve type: {Type}", message.Type);
                    message.Error = $"Could not resolve type: {message.Type}";
                    message.ProcessedOn = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync(cancellationToken);
                    continue;
                }

                var integrationEvent = JsonConvert.DeserializeObject(message.Content, eventType);

                // Publish to RabbitMQ
                await publishEndpoint.Publish(integrationEvent, cancellationToken);

                // Mark as processed
                message.ProcessedOn = DateTime.UtcNow;
                message.Error = null;

                _logger.LogInformation("Successfully published outbox message {MessageId} of type {Type}",
                    message.Id, message.Type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish outbox message {MessageId}", message.Id);

                message.RetryCount++;
                message.Error = ex.Message;

                if (message.RetryCount >= _maxRetryCount)
                {
                    message.ProcessedOn = DateTime.UtcNow; // Mark as failed permanently
                    _logger.LogWarning("Outbox message {MessageId} failed after {RetryCount} retries",
                        message.Id, message.RetryCount);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        // Schedule next batch if there are more messages
        var remainingCount = await dbContext.OutboxMessages
            .CountAsync(m => !m.Processed && m.RetryCount <= _maxRetryCount, cancellationToken);

        if (remainingCount > 0)
        {
            BackgroundJob.Enqueue<HangfireOutboxProcessor>(x =>
                x.ProcessOutboxMessagesAsync(CancellationToken.None));
        }
    }
}
