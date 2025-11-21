namespace CashFlow.Transaction.Application.Requests;

public record CreditTransactionRequest(Guid AccountId, decimal Value);