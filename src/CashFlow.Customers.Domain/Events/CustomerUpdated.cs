namespace CashFlow.Customers.Domain.Events;

public record CustomerUpdated(Guid Id, string FullName);