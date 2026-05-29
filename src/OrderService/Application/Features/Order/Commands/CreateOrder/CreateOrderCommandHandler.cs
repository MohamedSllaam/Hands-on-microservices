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
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IMapper mapper,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Generate order number
        var orderNumber = GenerateOrderNumber();

        // Create order entity (this will add the OrderCreatedDomainEvent)
        var order = new Order(orderNumber, request.CustomerName, request.CustomerEmail);

        // Add items
        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductSku, item.ProductName, item.Quantity, item.UnitPrice);
        }

        // Save to database - this will automatically trigger domain events
        // because the DbContext will dispatch them in SaveChangesAsync
        await _orderRepository.AddAsync(order, cancellationToken);

        _logger.LogInformation("Order {OrderNumber} created successfully with ID {OrderId}",
            order.OrderNumber, order.Id);

        return _mapper.Map<OrderDto>(order);
    }

    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.Now:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
    }
}