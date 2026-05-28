namespace Application.Features.Order.Commands.CreateOrder;

using Application.Features.Order.DTOs;
using MediatR;

public record CreateOrderCommand : IRequest<OrderDto>
    {
        public string CustomerName { get; init; } = null!;
        public string CustomerEmail { get; init; } = null!;
        public List<OrderItemCommand> Items { get; init; } = new();
    }

    public record OrderItemCommand
    {
        public string ProductSku { get; init; } = null!;
        public string ProductName { get; init; } = null!;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
    }
