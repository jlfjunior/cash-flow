namespace CashFlow.Transaction.Api.Sharable.Requests;

public record CreateDebitTransactionRequest(Guid CustomerId, decimal Value);
