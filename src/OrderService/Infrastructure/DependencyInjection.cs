namespace Infrastructure;
using Domain.Interfaces;
using Domain.Repositories;
using Infrastructure.Interceptors;
using Infrastructure.Persistence.Repositories;
 using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore.Diagnostics;
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


        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Database
        services.AddDbContext<OrderingDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<OrderingDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
        });



        return services;
    }
}