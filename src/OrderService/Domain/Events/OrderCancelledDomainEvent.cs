
namespace Domain.Events;

 
    public record OrderCancelledDomainEvent(Order Order) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }