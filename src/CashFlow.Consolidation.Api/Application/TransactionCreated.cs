namespace CashFlow.Consolidation.Api.Application;

public record TransactionCreated(Guid Id, Guid CustomerId, DateTime ReferenceDate, string Direction, string TransactionType, decimal Value);