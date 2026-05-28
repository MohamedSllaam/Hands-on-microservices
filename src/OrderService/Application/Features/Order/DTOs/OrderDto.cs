
namespace Application.Features.Order.DTOs;

    public record OrderDto
    {
        public int Id { get; init; }
        public string OrderNumber { get; init; } = null!;
        public string CustomerName { get; init; } = null!;
        public string CustomerEmail { get; init; } = null!;
        public DateTime OrderDate { get; init; }
        public decimal TotalAmount { get; init; }
        public string Status { get; init; } = null!;
        public List<OrderItemDto> Items { get; init; } = new();
    }

    public record OrderItemDto
    {
        public string ProductSku { get; init; } = null!;
        public string ProductName { get; init; } = null!;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice { get; init; }
    }
