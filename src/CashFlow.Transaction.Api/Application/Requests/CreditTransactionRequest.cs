namespace CashFlow.Transaction.Api.Application.Requests;

public record CreditTransactionRequest(Guid AccountId, decimal Value);