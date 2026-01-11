using MongoDB.Bson.Serialization.Attributes;

namespace CashFlow.Domain.Entities;

public class Customer
{
    [BsonId]
    [BsonElement("_id")]
    public Guid Id { get; private set; }
    
    [BsonElement("fullName")]
    public string FullName { get; private set; }

    public Customer(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required");
        
        Id = Guid.CreateVersion7();
        FullName = fullName;
    }

    public void WithFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required");
        
        FullName = fullName;
    }
}
