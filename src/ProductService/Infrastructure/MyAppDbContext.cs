using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Infrastructure;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class MyAppDbContext : IdentityDbContext<User, Role, int>
{
    public MyAppDbContext(DbContextOptions<MyAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> ProductCategories { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // IMPORTANT: Call base first to configure Identity tables
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure global delete behavior (prevent accidental cascading deletes)
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
        .Where(e => !typeof(User).IsAssignableFrom(e.ClrType) &&
                    !typeof(Role).IsAssignableFrom(e.ClrType) &&
                    !e.ClrType.Namespace?.Contains("Microsoft.AspNetCore.Identity") == true)
        .SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }


    }

    
}

