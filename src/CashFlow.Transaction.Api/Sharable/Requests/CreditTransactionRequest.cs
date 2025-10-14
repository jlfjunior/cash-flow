namespace CashFlow.Transaction.Api.Sharable.Requests;

public record CreditTransactionRequest(Guid CustomerId, decimal Value);