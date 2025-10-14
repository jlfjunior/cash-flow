namespace CashFlow.Transaction.Api.Sharable.Requests;

public record DebitTransactionRequest(Guid CustomerId, decimal Value);
