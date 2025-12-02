using CashFlow.Lib.Sharable;
using CashFlow.Transactions.Domain.Events;

namespace CashFlow.Transactions.Domain.Entities;

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
    
    public void AddDebit(decimal amount)
    {
        ProcessDebit(amount, TransactionType.Withdraw);
    }
    
    public void AddCredit(decimal amount)
    {

        if (Transactions.IsNull())
            Transactions = new List<Transaction>();

        var transaction = new Transaction(Id, Direction.Credit, TransactionType.Deposit, amount);
        
        Transactions.Add(transaction);
        Balance += amount;
    }
    
    public void PayBill(decimal amount)
    {
        ProcessDebit(amount, TransactionType.BillPayment);
    }

    private void ProcessDebit(decimal amount, TransactionType transactionType)
    {
        if (Balance < amount) 
            throw new InvalidOperationException("Debit amount can't be less than current balance");
        
        if (Transactions.IsNull())
            Transactions = new List<Transaction>();

        var transaction = new Transaction(Id, Direction.Debit, transactionType, amount);
        
        Transactions.Add(transaction);
        Balance -= amount;
        
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