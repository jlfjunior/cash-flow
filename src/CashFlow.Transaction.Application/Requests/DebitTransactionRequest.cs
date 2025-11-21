namespace CashFlow.Transaction.Application.Requests;

public record DebitTransactionRequest(Guid AccountId, decimal Value);
