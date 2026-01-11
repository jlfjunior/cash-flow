using CashFlow.Domain;

namespace CashFlow.Domain.Events;

public record AccountUpdated(Guid AccountId, DateTime ReferenceDate, decimal Balance, TransactionCreated Transaction) : IEvent;
