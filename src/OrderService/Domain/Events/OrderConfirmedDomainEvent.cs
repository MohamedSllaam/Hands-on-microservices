using Domain.Entities;
using Shared.Entities;


namespace Domain.Events;


public record OrderConfirmedDomainEvent : IDomainEvent
{
    public OrderConfirmedDomainEvent(Order order)
    {
        Order = order;
        OccurredOn = DateTime.UtcNow;
    }

    public Order Order { get; }
    public DateTime OccurredOn { get; }
}