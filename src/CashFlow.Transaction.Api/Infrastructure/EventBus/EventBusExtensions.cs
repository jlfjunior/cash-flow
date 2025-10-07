namespace CashFlow.Transaction.Api.Infrastructure.EventBus;

public static class EventBusExtensions
{
    public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<IEventBus, EventBus>();
        services.Configure<RabbitMQConfiguration>(configuration.GetSection("RabbitMQ"));
    }
}