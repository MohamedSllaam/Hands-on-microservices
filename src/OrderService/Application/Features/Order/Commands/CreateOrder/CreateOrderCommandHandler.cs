namespace Application.Features.Order.Commands.CreateOrder;

using Application.Features.Order.DTOs;
using AutoMapper;
using BuildingBlocks.Messaging.Events;
 using Domain.Repositories;
using MassTransit; 
using MediatR; 
using Microsoft.Extensions.Logging;
using Domain.Entities;



public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IPublishEndpoint publishEndpoint,
        IMapper mapper,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _publishEndpoint = publishEndpoint;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Generate order number
        var orderNumber = GenerateOrderNumber();

        // Create order entity
        var order = new Order(orderNumber, request.CustomerName, request.CustomerEmail);

        // Add items
        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductSku, item.ProductName, item.Quantity, item.UnitPrice);
        }

        // Save to database
        await _orderRepository.AddAsync(order, cancellationToken);

        // Publish integration event using Building Blocks messages
        var integrationEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            Items = request.Items.Select(x => new OrderItemMessage
            {
                ProductSku = x.ProductSku,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList()
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        _logger.LogInformation("Published OrderCreatedEvent for Order {OrderId} with {ItemCount} items",
            order.Id, integrationEvent.Items.Count);

        return _mapper.Map<OrderDto>(order);
    }

    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.Now:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
    }
}