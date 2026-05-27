namespace Infrastructure;
using Domain.Interfaces;
using Infrastructure.Persistence.Repositories;
 using Infrastructure.Settings;
using Microsoft.Extensions.Options;

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

         



        return services;
    }
}