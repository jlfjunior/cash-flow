namespace CashFlow.Transactions.Application.Requests;

public record TransferRequest(Guid SourceAccountId, Guid DestinationAccountId, decimal Value);

