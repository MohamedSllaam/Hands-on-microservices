namespace Application.Features.EventHandlers;

using Application.Features.ReserveStock.Commands;
using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;


public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(IMediator mediator, ILogger<OrderCreatedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        _logger.LogInformation("Received OrderCreatedEvent for Order {OrderId} with {ItemCount} items",
            context.Message.OrderId, context.Message.Items.Count);

        foreach (var item in context.Message.Items)
        {
            var command = new ReserveStockCommand
            {
                OrderId = context.Message.OrderId,
                OrderNumber = context.Message.OrderNumber,
                ProductSku = item.ProductSku,
                ProductName = item.ProductName,
                Quantity = item.Quantity
            };

            await _mediator.Send(command);
        }

        _logger.LogInformation("Successfully processed inventory reservation for Order {OrderId}", context.Message.OrderId);
    }
}