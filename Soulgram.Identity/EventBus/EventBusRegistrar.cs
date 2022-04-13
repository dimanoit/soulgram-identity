using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Soulgram.Eventbus;
using Soulgram.Eventbus.Interfaces;
using Soulgram.EventBus.RabbitMq;
using soulgram.identity.EventBus;

namespace soulgram.identity;

public static class EventBusRegistrar
{
    public static void AddEventBus(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var eventBusOption = configuration
            .GetSection("EventBus")
            .Get<EventBusOption>();

        serviceCollection.AddSingleton<IRabbitMQConnection>(_ =>
        {
            var factory = new ConnectionFactory
            {
                HostName = eventBusOption.Url,
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(eventBusOption.Username))
            {
                factory.UserName = eventBusOption.Username;
            }

            if (!string.IsNullOrEmpty(eventBusOption.Password))
            {
                factory.Password = eventBusOption.Password;
            }

            return new RabbitMQConnection(factory);
        });

        serviceCollection.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        serviceCollection.AddSingleton<IEventBus, Soulgram.EventBus.RabbitMq.EventBus>(sp =>
        {
            var rabbitMqConnection = sp.GetService<IRabbitMQConnection>();
            var busSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
            return new Soulgram.EventBus.RabbitMq.EventBus(
                rabbitMqConnection,
                busSubscriptionsManager,
                sp,
                eventBusOption.Exchange,
                eventBusOption.Queue);
        });
    }
}