using CashFlow.Lib.Sharable;

namespace CashFlow.Transactions.Domain.Entities;

public class Transaction : Entity
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Account Account { get; private set; }
    public Direction Direction { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public DateTime ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    
    protected Transaction() { }

    public Transaction(Guid accountId, Direction direction, TransactionType transactionType, decimal value)
    {
        if (value.IsLessThanOrEqualTo(decimal.Zero)) 
            throw new ArgumentException("Transaction value must be greater than zero", nameof(value));

        Id = Guid.CreateVersion7();
        AccountId = accountId;
        Direction = direction;
        TransactionType = transactionType;
        Value = value;
        ReferenceDate = DateTime.UtcNow;
    }
}