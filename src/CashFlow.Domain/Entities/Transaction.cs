using CashFlow.Domain;
using MongoDB.Bson.Serialization.Attributes;

namespace CashFlow.Domain.Entities;

[BsonKnownTypes(typeof(DepositTransaction), typeof(WithdrawTransaction), typeof(BillPaymentTransaction), typeof(TransferTransaction))]
public abstract class Transaction : Entity
{
    [BsonElement("_id")]
    public Guid Id { get; private set; }
    
    [BsonElement("accountId")]
    public Guid AccountId { get; private set; }
    
    [BsonIgnore]
    public Account? Account { get; private set; }
    
    [BsonElement("direction")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Direction Direction { get; private set; }
    
    [BsonElement("transactionType")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public TransactionType TransactionType { get; protected set; }
    
    [BsonElement("referenceDate")]
    public DateTime ReferenceDate { get; private set; }
    
    [BsonElement("value")]
    public decimal Value { get; private set; }
    
    protected Transaction() { }

    protected Transaction(Guid accountId, Direction direction, decimal value)
    {
        if (value <= decimal.Zero) 
            throw new ArgumentException("Transaction value must be greater than zero", nameof(value));

        Id = Guid.CreateVersion7();
        AccountId = accountId;
        Direction = direction;
        Value = value;
        ReferenceDate = DateTime.UtcNow;
    }
}
