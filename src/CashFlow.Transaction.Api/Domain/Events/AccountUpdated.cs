using CashFlow.Lib.Sharable;

namespace CashFlow.Transaction.Api.Domain.Events;

public record AccountUpdated(Guid AccountId, DateTime ReferenceDate, decimal Balance, TransactionCreated Transaction) : IEvent;