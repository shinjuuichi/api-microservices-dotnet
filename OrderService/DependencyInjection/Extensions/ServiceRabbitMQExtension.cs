using MassTransit;
using OrderService.DependencyInjection.Extensions.DependencyInjection.Options;


namespace OrderService.DependencyInjection.Extensions.DependencyInjection.Extensions
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
