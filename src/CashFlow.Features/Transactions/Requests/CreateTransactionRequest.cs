namespace CashFlow.Features.Transactions.Requests;

public record CreateTransactionRequest(Guid AccountId, string Direction, decimal Value);
