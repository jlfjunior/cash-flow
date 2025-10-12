using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace CashFlow.Lib.EventBus;

public static class EventBusExtensions
{
    public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
        var rabbitSection = configuration.GetSection("RabbitMq");
        var rabbitMqConfiguration = rabbitSection.Get<RabbitMqConfiguration>();
        
        var factory = new ConnectionFactory
        {
            HostName = rabbitMqConfiguration.HostName,
            Port = rabbitMqConfiguration.Port,
            UserName = rabbitMqConfiguration.Username,
            Password = rabbitMqConfiguration.Password,
        };
        
        services.Configure<RabbitMqConfiguration>(rabbitSection);
        
        services.AddSingleton<IEventBus, EventBus>();
        services.AddSingleton<ConnectionFactory>(factory);
    }
}