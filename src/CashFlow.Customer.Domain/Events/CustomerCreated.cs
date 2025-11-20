namespace CashFlow.Customer.Domain.Events;

public record CustomerCreated(Guid Id, string FullName);