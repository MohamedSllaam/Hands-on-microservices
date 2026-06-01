namespace Infrastructure.Services.BackgroundService;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using MassTransit;


public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorService> _logger;
    private readonly OutboxSetting _settings;

    public OutboxProcessorService(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorService> logger,
        IOptions<OutboxSetting> settings) // Inject settings
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor Service is starting with settings: {@Settings}", new
        {
            _settings.BatchSize,
            _settings.PollingIntervalSeconds,
            _settings.MaxRetryCount
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing outbox messages");
            }

            await Task.Delay(TimeSpan.FromSeconds(_settings.PollingIntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("Outbox Processor Service is stopping");
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        // Get unprocessed messages using settings
        var messages = await dbContext.OutboxMessages
            .Where(m => !m.Processed && m.RetryCount <= _settings.MaxRetryCount)
            .OrderBy(m => m.OccurredOn)
            .Take(_settings.BatchSize)
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
  //                eventType = AppDomain.CurrentDomain
  //.GetAssemblies()
  //.SelectMany(a => a.GetTypes())
  //.FirstOrDefault(t => t.FullName == message.Type);
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
                message.Processed = true;
                _logger.LogInformation("Successfully published outbox message {MessageId} of type {Type}",
                    message.Id, message.Type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish outbox message {MessageId}", message.Id);

                message.RetryCount++;
                message.Error = ex.Message;

                if (message.RetryCount >= _settings.MaxRetryCount)
                {
                    message.ProcessedOn = DateTime.UtcNow; // Mark as failed permanently
                    _logger.LogWarning("Outbox message {MessageId} failed after {RetryCount} retries",
                        message.Id, message.RetryCount);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}