namespace Domain.Entities;


    public class InventoryItem : BaseEntity
    {
        public string ProductName { get; set; } = null!;
        public string ProductSku { get; set; } = null!;
        public string? Description { get; set; }
        public int CurrentStock { get; set; }
        public int ReservedStock { get; set; }
        public int AvailableStock => CurrentStock - ReservedStock;
        public int MinimumStockThreshold { get; set; }
        public int MaximumStockThreshold { get; set; }
        public string Location { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();

        public void ReduceStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive");

            if (AvailableStock < quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {AvailableStock}, Requested: {quantity}");

            CurrentStock -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive");

            CurrentStock += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReserveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive");

            if (AvailableStock < quantity)
                throw new InvalidOperationException($"Cannot reserve {quantity}. Available stock: {AvailableStock}");

            ReservedStock += quantity;
        }

        public void ReleaseReservedStock(int quantity)
        {
            if (quantity <= 0 || quantity > ReservedStock)
                throw new ArgumentException($"Invalid quantity. Reserved stock: {ReservedStock}");

            ReservedStock -= quantity;
        }

        public void ConfirmReservedStock(int quantity)
        {
            if (quantity <= 0 || quantity > ReservedStock)
                throw new ArgumentException($"Invalid quantity. Reserved stock: {ReservedStock}");

            ReservedStock -= quantity;
            CurrentStock -= quantity;
        }
    }
