namespace CashFlow.Transactions.Application.Requests;

public record DebitTransactionRequest(Guid AccountId, decimal Value);
