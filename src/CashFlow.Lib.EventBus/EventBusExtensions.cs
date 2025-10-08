using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace CashFlow.Lib.EventBus;

public static class EventBusExtensions
{
    public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
        services.AddSingleton<IEventBus, EventBus>();
        
        var rabbitMqConfiguration = configuration.GetSection("RabbitMq");
        services.Configure<RabbitMqConfiguration>(rabbitMqConfiguration);
    }
}