namespace Infrastructure;
using Domain.Interfaces;
using Infrastructure.Persistence.Repositories;
 using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Reflection;
using BuildingBlocks.Messaging.MassTransit;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)

    {

        // Register JwtSettings with IOptions pattern
        services.Configure<JwtSettings>(
             configuration.GetSection("JwtSettings"));

        // Or with validation
        services.Configure<JwtSettings>(
             configuration.GetSection("JwtSettings"),
             options => options.BindNonPublicProperties = true);

        // Register as singleton for direct access (optional)
        services.AddSingleton(resolver =>
            resolver.GetRequiredService<IOptions<JwtSettings>>().Value);

 
 
        // Database
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        services.AddMessageBroker(
    configuration,
    Assembly.GetExecutingAssembly() // Or specifically: typeof(OrderCreatedConsumer).Assembly
);



        return services;
    }
}