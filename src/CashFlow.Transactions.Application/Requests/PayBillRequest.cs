namespace CashFlow.Transactions.Application.Requests;

public record PayBillRequest(Guid AccountId, decimal Value);

