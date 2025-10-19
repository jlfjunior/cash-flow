namespace CashFlow.Customer.Api.Domain.Events;

public record CustomerCreated(Guid Id, string FullName);