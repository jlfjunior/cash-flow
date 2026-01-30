namespace CashFlow.Features.Transactions.Requests;

public record DepositRequest(Guid AccountId, string Direction, decimal Value);
