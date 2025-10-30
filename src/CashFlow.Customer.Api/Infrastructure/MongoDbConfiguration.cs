namespace CashFlow.Customer.Api.Infrastructure;

public record class MongoDbConfiguration
{
    public MongoDbConfiguration() { }

    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public string Database { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}