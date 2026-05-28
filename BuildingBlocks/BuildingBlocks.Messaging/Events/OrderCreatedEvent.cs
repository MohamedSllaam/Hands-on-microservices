
namespace BuildingBlocks.Messaging.Events;

    public record OrderCreatedEvent : IntegrationEvent
    {
      
        public int OrderId { get; init; }
        public string OrderNumber { get; init; } = null!;
        public string CustomerName { get; init; } = null!;
        public string CustomerEmail { get; init; } = null!;
        public decimal TotalAmount { get; init; }
        public List<OrderItemMessage> Items { get; init; } = new();
}

public record OrderItemMessage
{
    public string ProductSku { get; init; } = null!;
    public string ProductName { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}


