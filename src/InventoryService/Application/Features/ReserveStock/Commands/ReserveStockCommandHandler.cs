namespace Application.Features.ReserveStock.Commands;

using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;


public class ReserveStockCommandHandler : IRequestHandler<ReserveStockCommand, ReserveStockResult>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<ReserveStockCommandHandler> _logger;

    public ReserveStockCommandHandler(
        IInventoryRepository inventoryRepository,
        ILogger<ReserveStockCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<ReserveStockResult> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var inventoryItem = await _inventoryRepository.GetBySkuAsync(request.ProductSku, cancellationToken);

            if (inventoryItem == null)
            {
                _logger.LogWarning("Product {ProductSku} not found in inventory", request.ProductSku);
                return new ReserveStockResult
                {
                    Success = false,
                    Message = $"Product {request.ProductSku} not found",
                    AvailableStock = 0,
                    ReservedStock = 0
                };
            }

            var availableBefore = inventoryItem.AvailableStock;

            inventoryItem.ReserveStock(request.Quantity);
            await _inventoryRepository.UpdateAsync(inventoryItem, cancellationToken);

            _logger.LogInformation(
                "Reserved {Quantity} units of {ProductSku} for Order {OrderNumber}. Available before: {AvailableBefore}, Available after: {AvailableAfter}",
                request.Quantity, request.ProductSku, request.OrderNumber,
                availableBefore, inventoryItem.AvailableStock);

            return new ReserveStockResult
            {
                Success = true,
                Message = "Stock reserved successfully",
                AvailableStock = inventoryItem.AvailableStock,
                ReservedStock = inventoryItem.ReservedStock
            };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to reserve stock for Order {OrderNumber}, Product {ProductSku}",
                request.OrderNumber, request.ProductSku);

            return new ReserveStockResult
            {
                Success = false,
                Message = ex.Message,
                AvailableStock = 0,
                ReservedStock = 0
            };
        }
    }
}