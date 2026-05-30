namespace Infrastructure.Services;

using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


public class HangfireOutboxScheduler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HangfireOutboxScheduler> _logger;
    private string? _jobId;

    public HangfireOutboxScheduler(
        IServiceProvider serviceProvider,
        ILogger<HangfireOutboxScheduler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Hangfire Outbox Scheduler is starting");

         


        RecurringJob.AddOrUpdate<HangfireOutboxProcessor>(
    "outbox-processor",
    processor => processor.ProcessOutboxMessagesAsync(CancellationToken.None),
    "*/5 * * * * *");

        _logger.LogInformation("Hangfire Outbox Scheduler scheduled with Job ID: {JobId}", _jobId);

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        if (_jobId != null)
        {
            RecurringJob.RemoveIfExists(_jobId);
            _logger.LogInformation("Removed recurring job: {JobId}", _jobId);
        }

        return base.StopAsync(cancellationToken);
    }
}