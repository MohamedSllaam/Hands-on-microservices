using Domain.Entities;
using Shared.Entities;


namespace Domain.Events;


    public record OrderConfirmedDomainEvent(Order Order) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
