 

namespace Application.Features.Order.EventHandlers;

using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
 
 
public class UpdateOrderStatisticsEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly ILogger<UpdateOrderStatisticsEventHandler> _logger;

    public UpdateOrderStatisticsEventHandler(ILogger<UpdateOrderStatisticsEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Update some statistics or cache
        _logger.LogInformation("Updating order statistics for customer {CustomerEmail}",
            notification.Order.CustomerEmail);

        // Example: Update Redis cache, increment counters, etc.
        await Task.CompletedTask;
    }
}