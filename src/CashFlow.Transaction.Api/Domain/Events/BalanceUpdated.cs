namespace CashFlow.Transaction.Api.Domain.Events;

public record BalanceUpdated(Guid AccountId, DateTime ReferenceDate, decimal Balance) : IEvent;