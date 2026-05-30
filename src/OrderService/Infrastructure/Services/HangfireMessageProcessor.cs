using Hangfire;
using Microsoft.Extensions.Logging;
using MassTransit;

namespace Infrastructure.Services;

public class HangfireMessageProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HangfireMessageProcessor> _logger;

    public HangfireMessageProcessor(
        IServiceProvider serviceProvider,
        ILogger<HangfireMessageProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 30, 60 },
        OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    [JobDisplayName("Process Outbox Message: {0}")]
    public async Task ProcessSingleMessageAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var message = await dbContext.OutboxMessages
            .FirstOrDefaultAsync(m => m.Id == messageId && !m.Processed, cancellationToken);

        if (message == null)
        {
            _logger.LogWarning("Message {MessageId} not found or already processed", messageId);
            return;
        }

        // Deserialize the integration event
        var eventType = Type.GetType(message.Type);
        if (eventType == null)
        {
            _logger.LogWarning("Could not resolve type: {Type}", message.Type);
            message.Error = $"Could not resolve type: {message.Type}";
            message.ProcessedOn = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var integrationEvent = JsonConvert.DeserializeObject(message.Content, eventType);

        // Publish to RabbitMQ
        await publishEndpoint.Publish(integrationEvent, cancellationToken);

        // Mark as processed
        message.ProcessedOn = DateTime.UtcNow;
        message.Error = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully published outbox message {MessageId} of type {Type}",
            message.Id, message.Type);
    }
}