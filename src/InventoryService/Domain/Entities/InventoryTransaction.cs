namespace Domain.Entities;

    public class InventoryTransaction : BaseEntity
    {
        public TransactionType Type { get; set; }
        public int Quantity { get; set; }
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
        public string ReferenceNumber { get; set; } = null!; // Order number or PO number
        public string? Notes { get; set; }

        // Foreign key
        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; } = null!;
    }

    public enum TransactionType
    {
        StockIn = 1,
        StockOut = 2,
        Reservation = 3,
        ReservationReleased = 4,
        ReservationConfirmed = 5,
        Adjustment = 6
    }
