using CashFlow.Lib.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace CashFlow.Transaction.Api.Domain.Entities;

public class Transaction : Entity
{
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
}