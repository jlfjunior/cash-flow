using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CashFlow.Customer.Api.Domain;

public class Customer
{
    [BsonId, BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }
    
    public string FullName { get; private set; }

    public Customer(string fullName)
    {
        Id = Guid.CreateVersion7();
        FullName = fullName;
    }
}