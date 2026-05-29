namespace Infrastructure.Persistence.Repositories;


using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Data;


public class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<InventoryRepository> _logger;
    private IDbContextTransaction? _currentTransaction;

    public InventoryRepository(
        InventoryDbContext context,
        ILogger<InventoryRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Query Methods
    public async Task<InventoryItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.InventoryItems
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory item by ID {Id}", id);
            throw;
        }
    }

    public async Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.InventoryItems
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.ProductSku == sku, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory item by SKU {Sku}", sku);
            throw;
        }
    }

    public async Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.InventoryItems
                .Include(x => x.Transactions)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all inventory items");
            throw;
        }
    }

    public async Task<IReadOnlyList<InventoryItem>> GetLowStockItemsAsync(int threshold, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.InventoryItems
                .Where(x => x.CurrentStock - x.ReservedStock <= threshold && x.IsActive)
                .Include(x => x.Transactions)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock items with threshold {Threshold}", threshold);
            throw;
        }
    }

    public async Task<IReadOnlyList<InventoryItem>> GetByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.InventoryItems
                .Where(x => x.Location == location && x.IsActive)
                .Include(x => x.Transactions)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory items by location {Location}", location);
            throw;
        }
    }

    // Command Methods
    public async Task AddAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.InventoryItems.AddAsync(inventoryItem, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Added new inventory item with SKU {Sku}", inventoryItem.ProductSku);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding inventory item with SKU {Sku}", inventoryItem.ProductSku);
            throw;
        }
    }

    public async Task UpdateAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default)
    {
        try
        {
            inventoryItem.UpdatedAt = DateTime.UtcNow;
            _context.Entry(inventoryItem).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated inventory item with SKU {Sku}", inventoryItem.ProductSku);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict updating inventory item with SKU {Sku}", inventoryItem.ProductSku);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inventory item with SKU {Sku}", inventoryItem.ProductSku);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var inventoryItem = await GetByIdAsync(id, cancellationToken);
            if (inventoryItem != null)
            {
                inventoryItem.IsActive = false; // Soft delete
                await UpdateAsync(inventoryItem, cancellationToken);
                _logger.LogInformation("Soft deleted inventory item with ID {Id}", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inventory item with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.InventoryItems
                .AnyAsync(x => x.ProductSku == sku, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of SKU {Sku}", sku);
            throw;
        }
    }

    // Bulk Operations
    public async Task AddRangeAsync(IEnumerable<InventoryItem> inventoryItems, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.InventoryItems.AddRangeAsync(inventoryItems, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Added {Count} inventory items", inventoryItems.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding range of inventory items");
            throw;
        }
    }

    public async Task UpdateRangeAsync(IEnumerable<InventoryItem> inventoryItems, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var item in inventoryItems)
            {
                item.UpdatedAt = DateTime.UtcNow;
                _context.Entry(item).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated {Count} inventory items", inventoryItems.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating range of inventory items");
            throw;
        }
    }

    // Transaction Support
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction ??= await _context.Database.BeginTransactionAsync(cancellationToken);
        _logger.LogDebug("Started new database transaction");
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
                _logger.LogDebug("Committed database transaction");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing transaction");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
                _logger.LogDebug("Rolled back database transaction");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back transaction");
            throw;
        }
    }

    // Stock Specific Operations
    public async Task<int> GetAvailableStockBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        try
        {
            var inventoryItem = await GetBySkuAsync(sku, cancellationToken);
            return inventoryItem?.AvailableStock ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available stock for SKU {Sku}", sku);
            throw;
        }
    }

    public async Task<bool> HasSufficientStockAsync(string sku, int requestedQuantity, CancellationToken cancellationToken = default)
    {
        try
        {
            var inventoryItem = await GetBySkuAsync(sku, cancellationToken);
            return inventoryItem != null && inventoryItem.AvailableStock >= requestedQuantity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking sufficient stock for SKU {Sku}", sku);
            throw;
        }
    }
}