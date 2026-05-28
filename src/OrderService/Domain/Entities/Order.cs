using System.ComponentModel.DataAnnotations;
using Shared.Entities;
using BuildingBlocks.Messaging.Events;
using Domain.Events;
namespace Domain.Entities;



    public class Order : BaseEntity
    {
        private Order() { } // EF Core constructor

        public Order(string orderNumber, string customerName, string customerEmail)
        {
            OrderNumber = orderNumber;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            OrderDate = DateTime.UtcNow;
            Status = OrderStatus.Pending;
            Items = new List<OrderItem>();

            AddDomainEvent(new OrderCreatedDomainEvent(this));
        }

        public string OrderNumber { get; private set; } = null!;
        public string CustomerName { get; private set; } = null!;
        public string CustomerEmail { get; private set; } = null!;
        public DateTime OrderDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public OrderStatus Status { get; private set; }
        public ICollection<OrderItem> Items { get; private set; }

        public void AddItem(string productSku, string productName, int quantity, decimal unitPrice)
        {
            var item = new OrderItem(productSku, productName, quantity, unitPrice, this);
            Items.Add(item);
            CalculateTotalAmount();
        }

        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Cannot confirm order with status {Status}");

            Status = OrderStatus.Confirmed;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new OrderConfirmedDomainEvent(this));
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
                throw new InvalidOperationException($"Cannot cancel order with status {Status}");

            Status = OrderStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new OrderCancelledDomainEvent(this));
        }

        private void CalculateTotalAmount()
        {
            TotalAmount = Items.Sum(x => x.TotalPrice);
        }
    }

    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }
