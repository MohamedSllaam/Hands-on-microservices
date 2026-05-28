

namespace Infrastructure.Configurations;

    public class InventoryItemConfig : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems");

            builder.HasKey(ii => ii.Id);

            builder.Property(ii => ii.ProductName)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("ProductName");

            builder.Property(ii => ii.ProductSku)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("ProductSku");

            builder.HasIndex(ii => ii.ProductSku)
                .IsUnique();

            builder.Property(ii => ii.Description)
                .HasMaxLength(500)
                .HasColumnName("Description");

            builder.Property(ii => ii.CurrentStock)
                .IsRequired()
                .HasColumnName("CurrentStock")
                .HasDefaultValue(0);

            builder.Property(ii => ii.ReservedStock)
                .IsRequired()
                .HasColumnName("ReservedStock")
                .HasDefaultValue(0);

            builder.Property(ii => ii.MinimumStockThreshold)
                .IsRequired()
                .HasColumnName("MinimumStockThreshold")
                .HasDefaultValue(5);

            builder.Property(ii => ii.MaximumStockThreshold)
                .IsRequired()
                .HasColumnName("MaximumStockThreshold")
                .HasDefaultValue(1000);

            builder.Property(ii => ii.Location)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Location");

            builder.Property(ii => ii.IsActive)
                .IsRequired()
                .HasColumnName("IsActive")
                .HasDefaultValue(true);

            builder.Property(ii => ii.CreatedAt)
                .IsRequired()
                .HasColumnName("CreatedAt")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(ii => ii.UpdatedAt)
                .HasColumnName("UpdatedAt");

            // Ignore computed property
            builder.Ignore(ii => ii.AvailableStock);

            // Relationships
            builder.HasMany(ii => ii.Transactions)
                .WithOne(t => t.InventoryItem)
                .HasForeignKey(t => t.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
