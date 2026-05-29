namespace Application.Features.Order.EventHandlers;


using BuildingBlocks.Messaging.Events;
using Domain.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

public class SendOrderCreatedIntegrationEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<SendOrderCreatedIntegrationEventHandler> _logger;

    public SendOrderCreatedIntegrationEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<SendOrderCreatedIntegrationEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling OrderCreatedDomainEvent for Order {OrderId}", notification.Order.Id);

        // Create integration event
        var integrationEvent = new OrderCreatedEvent
        {
            OrderId = notification.Order.Id,
            OrderNumber = notification.Order.OrderNumber,
            CustomerName = notification.Order.CustomerName,
            CustomerEmail = notification.Order.CustomerEmail,
            TotalAmount = notification.Order.TotalAmount,
            Items = notification.Order.Items.Select(x => new OrderItemMessage
            {
                ProductSku = x.ProductSku,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList()
        };

        // Publish to message broker
        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        _logger.LogInformation("Published OrderCreatedEvent for Order {OrderId} to message broker", notification.Order.Id);
    }
}