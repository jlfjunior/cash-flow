namespace CashFlow.Transaction.Api.Application.Requests;

public record DebitTransactionRequest(Guid AccountId, decimal Value);
