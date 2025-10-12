using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CashFlow.Lib.EventBus;

public class EventBus : IEventBus
{
    private readonly ILogger<EventBus> _logger;
    private readonly ConnectionFactory _factory;

    public EventBus(ILogger<EventBus> logger, ConnectionFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    public async Task PublishAsync<T>(T @event) where T : class
    {
        await using var connection = await _factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);
        var queueName = GetQueueName(@event);
        
        await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
        
        _logger.LogInformation($"Publishing domain event: {queueName} - {message}");
    }

    private static string GetQueueName<T>(T @event)
    {
        var queueName = "Queuing" + string.Concat(
            @event.GetType().Name.Select(c => char.IsUpper(c) ? $".{c}" : c.ToString())
        );

        return queueName;
    }
}