using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CashFlow.Lib.EventBus;

public class EventBus : IEventBus
{
    private readonly ILogger<EventBus> _logger;
    private readonly RabbitMqConfiguration _configuration;

    public EventBus(ILogger<EventBus> logger, IOptions<RabbitMqConfiguration> settings)
    {
        _logger = logger;
        _configuration = settings.Value;
    }

    public async Task PublishAsync<T>(T domainEvent) where T : class
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

    public async Task PublishAsync<T>(IEnumerable<T> domainEvents) where T : class
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

    public async Task SubscribeAsync<T>(T domainEvent) where T : class
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
        
        _logger.LogInformation($"Waiting for messages. Queue: {queueName}");
        
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation($" {queueName} message. Received {message}");
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer);
    }

    private static string GetQueueName<IDomainEvent>(IDomainEvent domainEvent) where IDomainEvent : class
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