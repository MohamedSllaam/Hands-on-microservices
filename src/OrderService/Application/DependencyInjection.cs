namespace Application;
 
using Application.Mappings;
using AutoMapper;
using BuildingBlocks.Messaging.MassTransit;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Shared.Behaviors;
using System.Reflection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddValidatorsFromAssembly(
        //typeof(RegisterCommandValidator).Assembly);

        //  services.AddAutoMapper(typeof(ProductMappingProfile));
        services.AddAutoMapper(typeof(MappingProfile));


        services.AddValidatorsFromAssembly(
    Assembly.GetExecutingAssembly());
        services.AddMediatR(config =>
        {

            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddFeatureManagement();
        services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());


        return services;
    }
}
