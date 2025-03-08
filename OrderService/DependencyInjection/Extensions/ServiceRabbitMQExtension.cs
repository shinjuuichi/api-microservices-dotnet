using MassTransit;
using OrderService.DependencyInjection.Extensions.DependencyInjection.Options;
using RabbitMQ.Contracts.Events;
using System.Reflection;


namespace OrderService.DependencyInjection.Extensions.DependencyInjection.Extensions
{
    public static class ServiceRabbitMQExtension
    {
        public static IServiceCollection AddRabbitMQServices(this IServiceCollection services, IConfiguration configuration)
        {
            var massTransitConfig = new MassTransitConfiguration();
            configuration.GetSection("MassTransitConfiguration").Bind(massTransitConfig);

            services.AddMassTransit(mt =>
            {
                mt.AddConsumers(Assembly.GetExecutingAssembly());

                mt.UsingRabbitMq((context, bus) =>
                {
                    bus.Host(massTransitConfig.Host, h =>
                    {
                        h.Username(massTransitConfig.Username);
                        h.Password(massTransitConfig.Password);
                    });

                    bus.ConfigureEndpoints(context);
                });
                mt.AddRequestClient<StockCheckEvent>();
            });

            return services;
        }
    }
}
