namespace CashFlow.Consolidation.Application.Responses;

public record TransactionCreated(Guid Id, Guid CustomerId, DateTime ReferenceDate, string Direction, string TransactionType, decimal Value);