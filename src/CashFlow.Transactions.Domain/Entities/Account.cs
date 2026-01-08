using CashFlow.Lib.Sharable;
using CashFlow.Transactions.Domain.Events;
using MongoDB.Bson.Serialization.Attributes;

namespace CashFlow.Transactions.Domain.Entities;

public class Account : Entity
{
    [BsonId]
    [BsonElement("_id")]
    public Guid Id { get; private set; }
    
    [BsonElement("version")]
    public int Version { get; private set; }
    
    [BsonElement("customerId")]
    public Guid CustomerId { get; private set; }
    
    [BsonElement("balance")]
    public decimal Balance { get; private set; }
    
    [BsonElement("transactions")]
    public ICollection<Transaction>? Transactions { get; set; }

    public Account(Guid customerId)
    {
        Id = Guid.NewGuid();
        Version = 1;
        CustomerId = customerId;
        Balance = decimal.Zero;
    }
    
    public void AddDebit(decimal amount)
    {
        ProcessDebit(new WithdrawTransaction(Id, amount));
    }
    
    public void AddCredit(decimal amount)
    {
        ProcessCredit(new DepositTransaction(Id, amount));
    }
    
    public void PayBill(decimal amount)
    {
        ProcessDebit(new BillPaymentTransaction(Id, amount));
    }

    public void AddTransaction(string direction, decimal amount)
    {
        if (string.Equals(direction, "Credit", StringComparison.OrdinalIgnoreCase))
        {
            AddCredit(amount);
        }
        else if (string.Equals(direction, "Debit", StringComparison.OrdinalIgnoreCase))
        {
            AddDebit(amount);
        }
        else
        {
            throw new ArgumentException($"Invalid direction: {direction}. Must be 'Credit' or 'Debit'.", nameof(direction));
        }
    }

    public void ProcessCredit(Transaction transaction)
    {
        Transactions ??= new List<Transaction>();

        Transactions.Add(transaction);
        Balance += transaction.Value;
        
        var transactionEventCreated = new TransactionCreated(
            transaction.Id,
            transaction.AccountId,
            transaction.Direction.ToString(),
            transaction.TransactionType.ToString(),
            transaction.ReferenceDate,
            transaction.Value);
        
        var balanceEvent = new AccountUpdated(Id, transactionEventCreated.ReferenceDate, Balance, transactionEventCreated);
        
        AddEvent(transactionEventCreated);
        AddEvent(balanceEvent);
    }

    public void ProcessDebit(Transaction transaction)
    {
        if (Balance < transaction.Value) 
            throw new InvalidOperationException("Debit amount can't be less than current balance");
        
        Transactions ??= new List<Transaction>();

        Transactions.Add(transaction);
        Balance -= transaction.Value;
        
        var transactionEventCreated = new TransactionCreated(
            transaction.Id,
            transaction.AccountId,
            transaction.Direction.ToString(),
            transaction.TransactionType.ToString(),
            transaction.ReferenceDate,
            transaction.Value);
        
        var balanceEvent = new AccountUpdated(Id, transactionEventCreated.ReferenceDate, Balance, transactionEventCreated);
        
        AddEvent(transactionEventCreated);
        AddEvent(balanceEvent);
    }
}