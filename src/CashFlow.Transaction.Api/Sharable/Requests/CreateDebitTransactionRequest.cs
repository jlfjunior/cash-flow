namespace CashFlow.Transaction.Api.Sharable;

public class CreateDebitTransactionRequest
{
    public Guid CustomerId { get; set; }
    public decimal Value { get; set; }
    public DateTime? ReferenceDate { get; set; }
}
