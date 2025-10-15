namespace CashFlow.Transaction.Api.Sharable.Requests;

public record CreditTransactionRequest(Guid AccountId, decimal Value);