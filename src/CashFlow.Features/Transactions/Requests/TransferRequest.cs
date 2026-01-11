namespace CashFlow.Features.Transactions.Requests;

public record TransferRequest(Guid SourceAccountId, Guid DestinationAccountId, decimal Value);
