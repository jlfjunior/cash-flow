namespace CashFlow.Consolidation.Api.Sharable;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateOnly ReferenceDate { get; set; }
    public string Direction { get; set; }
    public decimal Value { get; set; }
}