namespace CashFlow.Transaction.Api.Domain.Events;

public record TransactionCreatedEvent(
    Guid Id,
    Guid CustomerId,
    string Type,
    DateTime ReferenceDate,
    decimal Value);