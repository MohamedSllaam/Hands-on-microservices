using Shared.Entities.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories;

public interface IOutboxMessageConsumerRepository
{
    Task<bool> IsMessageProcessedAsync(Guid messageId, string consumerType, CancellationToken cancellationToken = default);
    Task AddProcessedMessageAsync(Guid messageId, string consumerType, CancellationToken cancellationToken = default);
    Task<OutboxMessageConsumer?> GetByIdAsync(Guid messageId, string consumerType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboxMessageConsumer>> GetProcessedMessagesAsync(DateTime fromDate, CancellationToken cancellationToken = default);
}
