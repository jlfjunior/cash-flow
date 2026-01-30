namespace CashFlow.Features.Withdraw;

public record WithdrawRequest(Guid AccountId, string Direction, decimal Value);
