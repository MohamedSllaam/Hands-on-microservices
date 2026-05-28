

namespace Infrastructure.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.ProductName)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("ProductName");

            builder.Property(oi => oi.ProductSku)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("ProductSku");

            builder.HasIndex(oi => oi.ProductSku);

            builder.Property(oi => oi.Quantity)
                .IsRequired()
                .HasColumnName("Quantity");

            builder.Property(oi => oi.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasColumnName("UnitPrice");

            builder.Property(oi => oi.OrderId)
                .IsRequired()
                .HasColumnName("OrderId");

            builder.Property(oi => oi.CreatedAt)
                .IsRequired()
                .HasColumnName("CreatedAt")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(oi => oi.UpdatedAt)
                .HasColumnName("UpdatedAt");

            // Ignore computed property
            builder.Ignore(oi => oi.TotalPrice);
        }
    }
