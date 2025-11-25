namespace CashFlow.Transactions.Application.Responses;

public record TransactionResponse(
    Guid Id,
    Guid AccountId,
    string Direction,
    DateTime ReferenceDate,
    decimal Value);
