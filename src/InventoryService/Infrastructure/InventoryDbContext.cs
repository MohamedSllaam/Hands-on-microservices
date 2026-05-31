namespace Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shared.Entities.Outbox;
using System.Reflection;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // IMPORTANT: Call base first to configure Identity tables
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure global delete behavior (prevent accidental cascading deletes)
        modelBuilder.Entity<OutboxMessageConsumer>(entity =>
        {
            entity.ToTable("OutboxMessageConsumers");
            entity.HasKey(e => new { e.Id, e.ConsumerType });
            entity.Property(e => e.ConsumerType).IsRequired().HasMaxLength(500);
        });


    }

    
}

