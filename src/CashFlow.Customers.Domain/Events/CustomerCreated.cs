namespace CashFlow.Customers.Domain.Events;

public record CustomerCreated(Guid Id, string FullName);