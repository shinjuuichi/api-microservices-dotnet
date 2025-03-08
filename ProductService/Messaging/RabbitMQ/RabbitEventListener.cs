using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ProductService.Messaging.RabbitMQ;
public class RabbitEventListener
{
    private readonly IBus _bus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitEventListener> _logger;

    public RabbitEventListener(IBus bus, IServiceProvider serviceProvider, ILogger<RabbitEventListener> logger)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void ListenTo(List<Type> eventTypes)
    {
        foreach (var eventType in eventTypes)
        {
            var method = typeof(RabbitEventListener).GetMethod(nameof(Subscribe), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var genericMethod = method.MakeGenericMethod(eventType);
            genericMethod.Invoke(this, null);
        }
    }

    private void Subscribe<T>() where T : class
    {
        _bus.PubSub.SubscribeAsync<T>(typeof(T).Name, async message =>
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetService<IEventHandler<T>>();
            if (handler != null)
            {
                await handler.Handle(message);
            }
            else
            {
                _logger.LogWarning($"No handler found for event: {typeof(T).Name}");
            }
        });
    }

}

public interface IEventHandler<T>
{
    Task Handle(T message);
}
