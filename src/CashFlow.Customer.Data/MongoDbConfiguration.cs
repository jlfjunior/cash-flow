namespace CashFlow.Customer.Data;

public record MongoDbConfiguration(string Host, int Port, string Database, string Username, string Password);