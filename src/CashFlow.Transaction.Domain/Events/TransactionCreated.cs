using CashFlow.Lib.Sharable;

namespace CashFlow.Transaction.Domain.Events;

public record TransactionCreated(
    Guid Id,
    Guid AccountId,
    string Direction,
    DateTime ReferenceDate,
    decimal Value) : IEvent;