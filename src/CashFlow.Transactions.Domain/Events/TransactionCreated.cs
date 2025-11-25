using CashFlow.Lib.Sharable;

namespace CashFlow.Transactions.Domain.Events;

public record TransactionCreated(
    Guid Id,
    Guid AccountId,
    string Direction,
    string TransactionType,
    DateTime ReferenceDate,
    decimal Value) : IEvent;