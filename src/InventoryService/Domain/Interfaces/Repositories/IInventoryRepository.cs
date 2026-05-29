namespace Domain.Interfaces.Repositories;

using Domain.Entities;

public interface IInventoryRepository
{
    // Query methods
    Task<InventoryItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryItem>> GetLowStockItemsAsync(int threshold, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryItem>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);

    // Command methods
    Task AddAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);

    // Bulk operations
    Task AddRangeAsync(IEnumerable<InventoryItem> inventoryItems, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<InventoryItem> inventoryItems, CancellationToken cancellationToken = default);

    // Transaction support
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    // Stock specific operations
    Task<int> GetAvailableStockBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<bool> HasSufficientStockAsync(string sku, int requestedQuantity, CancellationToken cancellationToken = default);
}