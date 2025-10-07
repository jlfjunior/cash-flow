using CashFlow.Lib.Common;
using CashFlow.Transaction.Api.Domain.Events;
using MongoDB.Bson.Serialization.Attributes;

namespace CashFlow.Transaction.Api.Domain.Entities;

public class Transaction
{
    [BsonIgnore]
    private ICollection<IDomainEvent> _domainEvents = new List<IDomainEvent>();
    [BsonIgnore]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();
    
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid Id { get; private set; }
    
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid CustomerId { get; private set; }
    
    public Direction Direction { get; private set; }
    public DateTime ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    
    protected Transaction() { }

    public Transaction(Guid id, Guid customerId, Direction direction, decimal value)
    {
        if (value.IsLessThanOrEqualTo(decimal.Zero)) 
            throw new ArgumentException("Transaction value must be greater than zero", nameof(value));

        Id = id;
        CustomerId = customerId;
        Direction = direction;
        Value = value;
        ReferenceDate = DateTime.UtcNow;
    }

    public void AddEvent(IDomainEvent domainDomainEvent) => _domainEvents.Add(domainDomainEvent);
    
    public void ClearDomainEvents() => _domainEvents.Clear();
}