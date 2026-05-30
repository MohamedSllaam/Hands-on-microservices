namespace Application.Features.EventHandlers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly IOutboxMessageConsumerRepository _outboxMessageConsumerRepository;

    public OrderCreatedConsumer(
        IMediator mediator,
        ILogger<OrderCreatedConsumer> logger,
        IOutboxMessageConsumerRepository outboxMessageConsumerRepository)
    {
        _mediator = mediator;
        _logger = logger;
        _outboxMessageConsumerRepository = outboxMessageConsumerRepository;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var messageId = context.Message.EventId;
        var consumerType = GetType().FullName!;

        // 1. Check if already processed (idempotency)
        var alreadyProcessed = await _outboxMessageConsumerRepository
            .IsMessageProcessedAsync(messageId, consumerType);

        if (alreadyProcessed)
        {
            _logger.LogWarning("Message {MessageId} already processed. Skipping.", messageId);
            return;
        }

        _logger.LogInformation("Processing Order {OrderId} with {ItemCount} items",
            context.Message.OrderId, context.Message.Items.Count);

        // 2. Process the message
        foreach (var item in context.Message.Items)
        {
            var command = new ReserveStockCommand
            {
                OrderId = context.Message.OrderId,
                OrderNumber = context.Message.OrderNumber,
                ProductSku = item.ProductSku,
                ProductName = item.ProductName,
                Quantity = item.Quantity
            };

            await _mediator.Send(command);
        }

        // 3. Mark as processed (only if successful)
        await _outboxMessageConsumerRepository.AddProcessedMessageAsync(messageId, consumerType);

        _logger.LogInformation("Order {OrderId} processed successfully", context.Message.OrderId);
    }
}