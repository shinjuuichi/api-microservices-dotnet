using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.DependencyInjection.Options;
using System.Reflection;

namespace ProductService.DependencyInjection.Extensions
{
    public static class ServiceRabbitMQExtension
    {
        public static IServiceCollection AddRabbitMQServices(this IServiceCollection services, IConfiguration configuration)
        {
            var massTransitConfig = new MassTransitConfiguration();
            configuration.GetSection("MassTransitConfiguration").Bind(massTransitConfig);

            services.AddMassTransit(mt =>
            {
                // Register ALL consumers in the assembly
                mt.AddConsumers(Assembly.GetExecutingAssembly());

                mt.UsingRabbitMq((context, bus) =>
                {
                    bus.Host(massTransitConfig.Host, h =>
                    {
                        h.Username(massTransitConfig.Username);
                        h.Password(massTransitConfig.Password);
                    });

                    // Dynamically create queues for all consumers
                    bus.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
