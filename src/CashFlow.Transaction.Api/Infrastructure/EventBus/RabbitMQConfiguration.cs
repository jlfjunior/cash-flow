namespace CashFlow.Transaction.Api.Infrastructure.EventBus;

public class RabbitMQConfiguration
{
    public string HostName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public string ExchangeName { get; set; }
}    
