using System.Text;
using System.Text.Json;
using CashFlow.Transaction.Api.Domain.Events;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CashFlow.Transaction.Api.Infrastructure.EventBus;

public class EventBus : IEventBus
{
    private readonly ILogger<EventBus> _logger;
    private readonly RabbitMQConfiguration _configuration;

    public EventBus(ILogger<EventBus> logger, IOptions<RabbitMQConfiguration> settings)
    {
        _logger = logger;
        _configuration = settings.Value;
    }

    public async Task PublishAsync(IDomainEvent domainEvent)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration.HostName,
            Port = _configuration.Port,
            UserName = _configuration.Username,
            Password = _configuration.Password,
        };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        var message = JsonSerializer.Serialize(domainEvent);
        var body = Encoding.UTF8.GetBytes(message);
        var queueName = GetQueueName(domainEvent);
        
        await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.BasicPublishAsync(exchange: _configuration.ExchangeName, routingKey: queueName, body: body);
        
        _logger.LogInformation($"Publishing domain event: {queueName}");
    }

    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration.HostName,
            Port = _configuration.Port,
            UserName = _configuration.Username,
            Password = _configuration.Password,
        };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        foreach (var domainEvent in domainEvents)
        {
            var message = JsonSerializer.Serialize(domainEvent);
            var body = Encoding.UTF8.GetBytes(message);
            var queueName = GetQueueName(domainEvent);
        
            await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            await channel.BasicPublishAsync(exchange: _configuration.ExchangeName, routingKey: queueName, body: body);
        
            _logger.LogInformation($"Publishing domain event: {queueName}");    
        }
    }

    private static string GetQueueName(IDomainEvent domainEvent)
    {
        var eventTypeName = domainEvent.GetType().Name;
        var queueName = "Queuing";
        
        foreach (var letter in eventTypeName)
        {
            if (char.IsUpper(letter))
                queueName += ".";
            
            queueName += letter;
        }

        return queueName;
    }
}