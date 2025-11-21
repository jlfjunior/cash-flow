namespace CashFlow.Transactions.Application.Requests;

public record CreditTransactionRequest(Guid AccountId, decimal Value);