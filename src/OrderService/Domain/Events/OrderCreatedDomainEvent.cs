

namespace Domain.Events;

using Domain.Entities;
using Shared.Entities;


 
public record OrderCreatedDomainEvent : IDomainEvent
{
    public OrderCreatedDomainEvent(Order order)
    {
        Order = order;
        OccurredOn = DateTime.UtcNow;
    }

    public Order Order { get; }
    public DateTime OccurredOn { get; }
}