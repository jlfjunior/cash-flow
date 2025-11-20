namespace CashFlow.Customer.Domain.Events;

public record CustomerUpdated(Guid Id, string FullName);