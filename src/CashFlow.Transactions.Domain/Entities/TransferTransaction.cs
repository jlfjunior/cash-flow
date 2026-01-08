using MongoDB.Bson.Serialization.Attributes;

namespace CashFlow.Transactions.Domain.Entities;

public class TransferTransaction : Transaction
{
    [BsonElement("destinationAccountId")]
    public Guid DestinationAccountId { get; private set; }
    
    protected TransferTransaction() { }

    public TransferTransaction(Guid accountId, Guid destinationAccountId, decimal value) 
        : base(accountId, Direction.Debit, value)
    {
        TransactionType = TransactionType.Transfer;
        DestinationAccountId = destinationAccountId;
    }
}

