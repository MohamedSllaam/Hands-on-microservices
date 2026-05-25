namespace Infrastructure;

using Application.Common.Interfaces;
using Domain.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services.Authentication;
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


        services.AddScoped<ITokenService, JWTService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        
 
 
        // Database
        services.AddDbContext<MyAppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddIdentityServicesServices(configuration);
        



        return services;
    }
}