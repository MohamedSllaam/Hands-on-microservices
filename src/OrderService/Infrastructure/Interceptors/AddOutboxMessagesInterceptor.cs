using Shared.Entities.Outbox;

namespace Infrastructure.Interceptors;


public class AddOutboxMessagesInterceptor
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AddOutboxMessagesAsync(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await AddOutboxMessagesAsync(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public async Task AddOutboxMessagesAsync(DbContext? context)
    {
        if (context == null) return;

      var entities = context.ChangeTracker.Entries<BaseEntity>()
                                   .Where(e=> e.Entity.DomainEvents.Any())
                                   .Select(e=> e.Entity)
                                   .ToList();

        var outboxMessages = new List<OutboxMessage>();
        foreach (var entity in entities)
        {

            var domainEvents = entity.DomainEvents.ToList();

            foreach (var domainEvent in domainEvents)
              {
                // Convert OrderCreatedDomainEvent to OrderCreatedEvent
                if (domainEvent is OrderCreatedDomainEvent orderCreatedEvent)
                {
                    var integrationEvent = new OrderCreatedEvent
                    {
                        EventId = Guid.NewGuid(),
                        OccurredOn = DateTime.UtcNow,
                        OrderId = orderCreatedEvent.Order.Id,
                        OrderNumber = orderCreatedEvent.Order.OrderNumber,
                        CustomerName = orderCreatedEvent.Order.CustomerName,
                        CustomerEmail = orderCreatedEvent.Order.CustomerEmail,
                        TotalAmount = orderCreatedEvent.Order.TotalAmount,
                        Items = orderCreatedEvent.Order.Items.Select(x => new OrderItemMessage
                        {
                            ProductSku = x.ProductSku,
                            ProductName = x.ProductName,
                            Quantity = x.Quantity,
                            UnitPrice = x.UnitPrice
                        }).ToList()
                    };

                    outboxMessages.Add(new OutboxMessage
                    {
                        Id = Guid.NewGuid(),
                        Type = integrationEvent.EventType,
                        Content = JsonConvert.SerializeObject(integrationEvent),
                        OccurredOn = DateTime.UtcNow,
                        RetryCount = 0
                    });
                  }
              
                // Add other domain event conversions here
            }
            if (outboxMessages.Count>0)
            {
                await context.AddRangeAsync(outboxMessages);
            }
        }

   
    }
}
