namespace CashFlow.Transaction.Api.Sharable.Requests;

public record DebitTransactionRequest(Guid AccountId, decimal Value);
