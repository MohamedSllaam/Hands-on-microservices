using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Entities.Outbox;
using System.Reflection;

public class OrderingDbContext : DbContext
{
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // IMPORTANT: Call base first to configure Identity tables
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure global delete behavior (prevent accidental cascading deletes)

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.OccurredOn).IsRequired();
            entity.HasIndex(e => e.ProcessedOn);
            entity.HasIndex(e => new { e.ProcessedOn, e.RetryCount });
        });

        modelBuilder.Entity<OutboxMessageConsumer>(entity =>
        {
            entity.ToTable("OutboxMessageConsumers");
            entity.HasKey(e => new { e.Id, e.ConsumerType });
            entity.Property(e => e.ConsumerType).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.ProcessedOn);
        });

    }

    
}

