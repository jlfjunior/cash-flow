namespace CashFlow.Transaction.Api.Domain;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public TransactionType Type { get; set; }
    public DateTime ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
}