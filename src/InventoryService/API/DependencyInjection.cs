using API.Extentions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Shared.Exceptions.Handler;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
       
        services.AddHttpContextAccessor();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("DefaultConnection")!);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddswaggerDocmentation();
        services.AddCors(opt => {
            opt.AddPolicy("CorsPolicy", policy => {
                policy.AllowAnyHeader().AllowAnyMethod().WithOrigins(configuration["JWT:ClientUrl"]!);
            });
        });

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        
        app.UseCors("CorsPolicy");
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerDocmentation();

        }
        app.UseExceptionHandler(options => { });
        app.UseHealthChecks("/health",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        return app;
    }
}

