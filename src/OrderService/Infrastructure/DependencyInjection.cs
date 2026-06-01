namespace Infrastructure;
using Domain.Interfaces;
using Domain.Repositories;
using Infrastructure.Interceptors;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services.BackgroundService;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)

    {
 
       
        services.Configure<OutboxSetting>(
             configuration.GetSection("OutboxSetting"),
             options => options.BindNonPublicProperties = true);

       //// Register as singleton for direct access (optional)
       //services.AddSingleton(resolver =>
       //     resolver.GetRequiredService<IOptions<OutboxSetting>>().Value);



    
     
        services.AddHostedService<OutboxProcessorService>();

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<ISaveChangesInterceptor, AddOutboxMessagesInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<OrderingDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
        });



        return services;
    }

}