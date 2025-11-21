namespace CashFlow.Transaction.Application.Responses;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string Direction { get; set; } = string.Empty;
    public DateTime ReferenceDate { get; set; }
    public decimal Value { get; set; }
}
