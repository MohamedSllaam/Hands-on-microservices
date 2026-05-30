namespace Infrastructure;
using Domain.Interfaces;
using Domain.Repositories;
using Hangfire;
using Hangfire.SqlServer;
using Infrastructure.Interceptors;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
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


        services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount; // Number of concurrent workers
            options.Queues = new[] { "default", "outbox" };
        });

        services.AddHostedService<HangfireOutboxScheduler>();

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, AddOutboxMessagesInterceptor>();

        services.AddDbContext<OrderingDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
        });



        return services;
    }
}