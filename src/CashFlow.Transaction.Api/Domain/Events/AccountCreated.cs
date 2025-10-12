namespace CashFlow.Transaction.Api.Domain.Events;

public record AccountCreated(Guid AccountId, Guid CustomerId) : IEvent;