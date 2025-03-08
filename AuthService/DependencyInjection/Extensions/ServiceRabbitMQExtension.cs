using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthService.DependencyInjection.Options;

namespace AuthService.DependencyInjection.Extensions
{
    public static class ServiceRabbitMQExtension
    {
        public static IServiceCollection AddRabbitMQServices(this IServiceCollection services, IConfiguration configuration)
        {
            var massTransitConfig = new MassTransitConfiguration();
            configuration.GetSection("MassTransitConfiguration").Bind(massTransitConfig);

            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(massTransitConfig.Host, h =>
                    {
                        h.Username(massTransitConfig.Username);
                        h.Password(massTransitConfig.Password);
                    });
                });
            });

            return services;
        }
    }
}
