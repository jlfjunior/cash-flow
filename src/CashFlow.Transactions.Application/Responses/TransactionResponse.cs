namespace CashFlow.Transactions.Application.Responses;

public record TransactionResponse(
    Guid Id,
    Guid AccountId,
    string Direction,
    string TransactionType,
    DateTime ReferenceDate,
    decimal Value);
