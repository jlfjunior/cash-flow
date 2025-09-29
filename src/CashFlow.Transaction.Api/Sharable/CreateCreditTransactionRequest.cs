namespace CashFlow.Transaction.Api.Sharable;

public class CreateCreditTransactionRequest
{
    public Guid CustomerId { get; set; }
    public decimal Value { get; set; }
    public DateTime? ReferenceDate { get; set; }
}
