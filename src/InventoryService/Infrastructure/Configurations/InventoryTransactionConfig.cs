

namespace Infrastructure.Configurations;



    public class InventoryTransactionConfig : IEntityTypeConfiguration<InventoryTransaction>
    {
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            builder.ToTable("InventoryTransactions");

            builder.HasKey(it => it.Id);

            builder.Property(it => it.Type)
                .IsRequired()
                .HasColumnName("Type")
                .HasConversion<int>();

            builder.Property(it => it.Quantity)
                .IsRequired()
                .HasColumnName("Quantity");

            builder.Property(it => it.QuantityBefore)
                .IsRequired()
                .HasColumnName("QuantityBefore");

            builder.Property(it => it.QuantityAfter)
                .IsRequired()
                .HasColumnName("QuantityAfter");

            builder.Property(it => it.ReferenceNumber)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ReferenceNumber");

            builder.HasIndex(it => it.ReferenceNumber);

            builder.Property(it => it.Notes)
                .HasMaxLength(500)
                .HasColumnName("Notes");

            builder.Property(it => it.InventoryItemId)
                .IsRequired()
                .HasColumnName("InventoryItemId");

            builder.Property(it => it.CreatedAt)
                .IsRequired()
                .HasColumnName("CreatedAt")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(it => it.UpdatedAt)
                .HasColumnName("UpdatedAt");
        }
    }
