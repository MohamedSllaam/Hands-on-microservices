using Shared.Entities.Outbox;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories;


public class OutboxMessageConsumerRepository : IOutboxMessageConsumerRepository
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<OutboxMessageConsumerRepository> _logger;

    public OutboxMessageConsumerRepository(
        InventoryDbContext context,
        ILogger<OutboxMessageConsumerRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> IsMessageProcessedAsync(Guid messageId, string consumerType, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OutboxMessageConsumers
                .AnyAsync(x => x.Id == messageId && x.ConsumerType == consumerType, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if message {MessageId} processed by {ConsumerType}", messageId, consumerType);
            throw;
        }
    }

    public async Task AddProcessedMessageAsync(Guid messageId, string consumerType, CancellationToken cancellationToken = default)
    {
        try
        {
            var processedMessage = new OutboxMessageConsumer
            {
                Id = messageId,
                ConsumerType = consumerType,
                ProcessedOn = DateTime.UtcNow
            };

            await _context.OutboxMessageConsumers.AddAsync(processedMessage, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Marked message {MessageId} as processed by {ConsumerType}", messageId, consumerType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding processed message {MessageId} for consumer {ConsumerType}", messageId, consumerType);
            throw;
        }
    }

    public async Task<OutboxMessageConsumer?> GetByIdAsync(Guid messageId, string consumerType, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OutboxMessageConsumers
                .FirstOrDefaultAsync(x => x.Id == messageId && x.ConsumerType == consumerType, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting message {MessageId} for consumer {ConsumerType}", messageId, consumerType);
            throw;
        }
    }

    public async Task<IReadOnlyList<OutboxMessageConsumer>> GetProcessedMessagesAsync(DateTime fromDate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OutboxMessageConsumers
                .Where(x => x.ProcessedOn >= fromDate)
                .OrderBy(x => x.ProcessedOn)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting processed messages from {FromDate}", fromDate);
            throw;
        }
    }
}
