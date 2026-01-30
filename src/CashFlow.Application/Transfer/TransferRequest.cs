namespace CashFlow.Features.Transfer;

public record TransferRequest(Guid SourceAccountId, Guid DestinationAccountId, decimal Value);
