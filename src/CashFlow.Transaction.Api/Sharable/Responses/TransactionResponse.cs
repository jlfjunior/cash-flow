namespace CashFlow.Transaction.Api.Sharable.Responses;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Direction { get; set; } = string.Empty;
    public DateTime ReferenceDate { get; set; }
    public decimal Value { get; set; }
}
