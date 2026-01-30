namespace CashFlow.Features.Transactions.Requests;

public record PayBillRequest(Guid AccountId, decimal Value);
