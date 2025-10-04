using CashFlow.Lib.Common;
using CashFlow.Transaction.Api.Domain.Events;

namespace CashFlow.Transaction.Api.Domain.Entities;

public class Transaction
{
    private ICollection<IEvent> _domainEvents = new List<IEvent>();
    
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    

    // Private constructor for entity framework or other ORM frameworks
    private Transaction() { }

    // Public constructor for creating new transactions
    public Transaction(Guid id, Guid customerId, TransactionType type, decimal value, DateTime referenceDate)
    {
        if (value.IsLessThanOrEqualTo(decimal.Zero)) 
            throw new ArgumentException("Transaction value must be greater than zero", nameof(value));

        Id = id;
        CustomerId = customerId;
        Type = type;
        Value = value;
        ReferenceDate = referenceDate;
    }

    public void AddEvent(IEvent domainEvent) => _domainEvents.Add(domainEvent);

}