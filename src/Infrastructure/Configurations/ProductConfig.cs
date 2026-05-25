

namespace Infrastructure.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table name
        builder.ToTable("Products");

        // Primary key
        builder.HasKey(p => p.Id);

        // Property configurations
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2); // 18 total digits, 2 decimal places

        builder.Property(p => p.quantity)
            .IsRequired()
            .HasDefaultValue(0);


        // Relationship
        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

      
    }
}