```csharp
public abstract class Entity
{
    private readonly List<IEvent> _events = new();

    public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();
    
    public void AddEvent(IEvent @event) => _events.Add(@event);
    
    public void ClearEvents() => _events.Clear();
}

public class Account : Entity
{
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; }
    
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


    public void AddTransaction(string direction, decimal amount)
    {
        switch (direction?.ToUpperInvariant())
        {
            case "CREDIT":
                AddCredit(amount);
                break;
            case "DEBIT":
                AddDebit(amount);
                break;
            default:
                throw new ArgumentException($"Invalid direction: {direction}. Must be 'Credit' or 'Debit'.", nameof(direction));
        }
    }

    private void ProcessCredit(Transaction transaction)
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

    private void ProcessDebit(Transaction transaction)
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

public abstract class Transaction : Entity
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Account? Account { get; private set; }
    public Direction Direction { get; private set; }
    public TransactionType TransactionType { get; protected set; }
    public DateTime ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    
    protected Transaction() { }

    protected Transaction(Guid accountId, Direction direction, decimal value)
    {
        if (value.IsLessThanOrEqualTo(decimal.Zero)) 
            throw new ArgumentException("Transaction value must be greater than zero", nameof(value));

        Id = Guid.CreateVersion7();
        AccountId = accountId;
        Direction = direction;
        Value = value;
        ReferenceDate = DateTime.UtcNow;
    }
}

public class BillPaymentTransaction : Transaction
{
    protected BillPaymentTransaction() { }

    public BillPaymentTransaction(Guid accountId, decimal value) 
        : base(accountId, Direction.Debit, value)
    {
        TransactionType = TransactionType.BillPayment;
    }
}


public class DepositTransaction : Transaction
{
    protected DepositTransaction() { }

    public DepositTransaction(Guid accountId, decimal value) 
        : base(accountId, Direction.Credit, value)
    {
        TransactionType = TransactionType.Deposit;
    }
}

public class WithdrawTransaction : Transaction
{
    protected WithdrawTransaction() { }

    public WithdrawTransaction(Guid accountId, decimal value) 
        : base(accountId, Direction.Debit, value)
    {
        TransactionType = TransactionType.Withdraw;
    }
}
```

```csharp
public class Account : Entity
{
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; }
    
    public ICollection<Transaction>? Transactions { get; set; }

    public Account(Guid customerId)
    {
        Id = Guid.NewGuid();
        Version = 1;
        CustomerId = customerId;
        Balance = decimal.Zero;
    }
    
    public void AddDebit(decimal amount) {...}
    
    public void AddCredit(decimal amount) {...}
    
    public void PayBill(decimal amount) {...}

    public void AddTransaction(string direction, decimal amount) {...}

    private void ProcessCredit(Transaction transaction) {...}

    private void ProcessDebit(Transaction transaction) {...}
}
```