

namespace Domain.Events;

using Domain.Entities;
using Shared.Entities;


    public record OrderCreatedDomainEvent(Order Order) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }