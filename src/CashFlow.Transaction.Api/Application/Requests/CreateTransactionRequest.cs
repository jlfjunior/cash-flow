namespace CashFlow.Transaction.Api.Application.Requests;

public record CreateTransactionRequest(Guid AccountId, string Direction, decimal Value);