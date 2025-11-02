using CashFlow.Lib.Sharable;

namespace CashFlow.Transaction.Api.Domain.Events;

public record TransactionCreated(
    Guid Id,
    Guid AccountId,
    string Direction,
    DateTime ReferenceDate,
    decimal Value) : IEvent;