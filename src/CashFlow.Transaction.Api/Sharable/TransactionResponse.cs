namespace CashFlow.Transaction.Api.Sharable;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime ReferenceDate { get; set; }
    public decimal Value { get; set; }
}
