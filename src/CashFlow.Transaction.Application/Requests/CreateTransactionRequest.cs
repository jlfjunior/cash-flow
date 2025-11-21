namespace CashFlow.Transaction.Application.Requests;

public record CreateTransactionRequest(Guid AccountId, string Direction, decimal Value);