using CashFlow.Lib.Sharable;
using CashFlow.Transaction.Domain.Events;

namespace CashFlow.Transaction.Domain.Entities;

public class Account : Entity
{
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; }
    
    public ICollection<Transaction> Transactions { get; set; }

    public Account(Guid customerId)
    {
        Id = Guid.NewGuid();
        Version = 1;
        CustomerId = customerId;
        Balance = decimal.Zero;
    }

    public void AddTransaction(string direction, decimal amount) { }
    
    public void AddDebit(decimal amount)
    {
        if (Balance < amount) 
            throw new InvalidOperationException("Debit amount can't be less than current balance");
        
        if (Transactions.IsNull())
            Transactions = new List<Transaction>();

        var transaction = new Transaction(Id, Direction.Debit, amount);
        
        Transactions.Add(transaction);
        Balance -= amount;
        
        var transactionEventCreated = new TransactionCreated(
            transaction.Id,
            transaction.AccountId,
            transaction.Direction.ToString(),
            transaction.ReferenceDate,
            transaction.Value);
        
        var balanceEvent = new AccountUpdated(Id, transactionEventCreated.ReferenceDate, Balance, transactionEventCreated);
        
        AddEvent(transactionEventCreated);
        AddEvent(balanceEvent);
    }
    
    public void AddCredit(decimal amount)
    {

        if (Transactions.IsNull())
            Transactions = new List<Transaction>();

        var transaction = new Transaction(Id, Direction.Credit, amount);
        
        Transactions.Add(transaction);
        Balance += amount;
    }
}