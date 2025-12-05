using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CashFlow.Lib.EventBus;

public class EventBus(ILogger<EventBus> logger, ConnectionFactory factory) : IEventBus
{
    public async Task PublishAsync<T>(T @event, string queueName) where T : class
    {
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);
        
        await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
        
        logger.LogInformation($"Publishing domain event: {queueName} - {message}");
    }

    public async Task SubscribeAsync<T>(string queueName, Func<T, Task> handler) where T : class
    {
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var _jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // ajuda quando vier "id" vs "Id"
                };
                var message = JsonSerializer.Deserialize<T>(json, _jsonOptions);

                if (message != null)
                {
                    await handler(message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing message from queue {QueueName}", queueName);
            }
        };

        await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);
        
        logger.LogInformation("Started consuming from queue: {QueueName}", queueName);
    }
}