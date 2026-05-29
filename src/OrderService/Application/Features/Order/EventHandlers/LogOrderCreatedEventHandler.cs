namespace Application.Features.Order.EventHandlers;

using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
 

 
public class LogOrderCreatedEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly ILogger<LogOrderCreatedEventHandler> _logger;

    public LogOrderCreatedEventHandler(ILogger<LogOrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderNumber} created with {ItemCount} items, Total: {TotalAmount:C}",
            notification.Order.OrderNumber,
            notification.Order.Items.Count,
            notification.Order.TotalAmount);

        return Task.CompletedTask;
    }
}