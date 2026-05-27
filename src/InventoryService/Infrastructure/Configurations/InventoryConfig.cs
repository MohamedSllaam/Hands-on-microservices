using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations;


public class InventoryConfig : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        // Table name (optional)
        builder.ToTable("Inventories");
           
        // Primary key (assuming BaseEntity has Id)
        builder.HasKey(pc => pc.Id);

        // Property configurations
        builder.Property(pc => pc.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");


      
    }
}