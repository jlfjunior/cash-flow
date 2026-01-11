using CashFlow.Domain;

namespace CashFlow.Domain.Events;

public record TransactionCreated(
    Guid Id,
    Guid AccountId,
    string Direction,
    string TransactionType,
    DateTime ReferenceDate,
    decimal Value) : IEvent;
