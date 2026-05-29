using MediatR;

namespace Application.Features.ReserveStock.Commands;




public record ReserveStockCommand : IRequest<ReserveStockResult>
{
    public int OrderId { get; init; }
    public string OrderNumber { get; init; } = null!;
    public string ProductSku { get; init; } = null!;
    public string ProductName { get; init; } = null!;
    public int Quantity { get; init; }
}

public record ReserveStockResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = null!;
    public int AvailableStock { get; init; }
    public int ReservedStock { get; init; }
}