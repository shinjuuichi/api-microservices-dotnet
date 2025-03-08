using System;
using System.Collections.Generic;
using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Serialization.SystemTextJson;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace OrderService.Messaging.RabbitMQ;

public static class RabbitInstaller
{
    public static IServiceCollection AddRabbitListeners(this IServiceCollection services)
    {
        var host = "localhost";
        var connectionStr = $"host={host}:5672;username=guest;password=guest";
        var bus = RabbitHutch.CreateBus(connectionStr, x => x.Register<ISerializer>(_ => new SystemTextJsonSerializer()));
        bus.Advanced.ExchangeDeclare("lab-dotnet-micro", ExchangeType.Topic);
        services.AddSingleton(bus);

        services.AddSingleton(svc => new RabbitEventListener(
                    svc.GetRequiredService<IBus>(),
                    svc,
                    svc.GetRequiredService<ILogger<RabbitEventListener>>()
                ));

        return services;
    }
}

public static class RabbitListenersInstaller
{
    public static void UseRabbitListeners(this IApplicationBuilder app, List<Type> eventTypes)
    {
        app.ApplicationServices.GetRequiredService<RabbitEventListener>().ListenTo(eventTypes);
    }
}