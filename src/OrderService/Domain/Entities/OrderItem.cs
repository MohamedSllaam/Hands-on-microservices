namespace Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


 
    public class OrderItem : BaseEntity
    {
        private OrderItem() { }

        public OrderItem(string productSku, string productName, int quantity, decimal unitPrice, Order order)
        {
            ProductSku = productSku;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Order = order;
            OrderId = order.Id;
        }

        public string ProductSku { get; private set; } = null!;
        public string ProductName { get; private set; } = null!;
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        public int OrderId { get; private set; }
        public Order Order { get; private set; } = null!;
    }
