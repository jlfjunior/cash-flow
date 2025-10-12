namespace CashFlow.Transaction.Api.Domain.Events;

public record TransactionCreated(
    Guid Id,
    Guid CustomerId,
    string Type,
    DateTime ReferenceDate,
    decimal Value) : IEvent;