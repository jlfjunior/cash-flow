namespace CashFlow.Transaction.Api.Sharable.Requests;

public record CreateCreditTransactionRequest(Guid CustomerId, decimal Value);