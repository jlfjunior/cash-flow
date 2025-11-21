namespace CashFlow.Transactions.Application.Requests;

public record CreateTransactionRequest(Guid AccountId, string Direction, decimal Value);