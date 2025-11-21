using CashFlow.Lib.Sharable;

namespace CashFlow.Transaction.Domain.Events;

public record AccountUpdated(Guid AccountId, DateTime ReferenceDate, decimal Balance, TransactionCreated Transaction) : IEvent;