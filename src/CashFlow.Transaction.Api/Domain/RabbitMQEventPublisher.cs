using CashFlow.Transaction.Api.Domain.Events;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CashFlow.Transaction.Api.Domain;

public class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly RabbitMQSettings _settings;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQEventPublisher(ILogger<RabbitMQEventPublisher> logger, IOptions<RabbitMQSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;

        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare exchange
        _channel.ExchangeDeclare(_settings.ExchangeName, ExchangeType.Topic, durable: true);
        
        // Declare queue
        _channel.QueueDeclare(_settings.QueueName, durable: true, exclusive: false, autoDelete: false);
        
        // Bind queue to exchange
        _channel.QueueBind(_settings.QueueName, _settings.ExchangeName, "transaction.created");
    }

    public async Task PublishAsync<T>(T eventData, string routingKey) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(eventData);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: _settings.ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Event published successfully. RoutingKey: {RoutingKey}, EventType: {EventType}", 
                routingKey, typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event. RoutingKey: {RoutingKey}, EventType: {EventType}", 
                routingKey, typeof(T).Name);
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}

public class RabbitMQSettings
{
    public string HostName { get; set; } = string.Empty;
    public int Port { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
}
