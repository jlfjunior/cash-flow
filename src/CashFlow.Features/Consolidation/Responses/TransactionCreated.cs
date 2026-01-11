namespace CashFlow.Features.Consolidation.Responses;

public record TransactionCreated(Guid Id, Guid CustomerId, DateTime ReferenceDate, string Direction, string TransactionType, decimal Value);
