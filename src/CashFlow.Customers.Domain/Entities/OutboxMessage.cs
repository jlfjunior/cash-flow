using MongoDB.Bson.Serialization.Attributes;

namespace CashFlow.Customers.Domain.Entities;

public class OutboxMessage
{
    [BsonId]
    [BsonElement("_id")]
    public Guid Id { get; set; }
    
    [BsonElement("type")]
    public string Type { get; set; }
    
    [BsonElement("content")]
    public string Content { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("processedAt")]
    public DateTime? ProcessedAt { get; set; }
    
    [BsonElement("status")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public OutboxStatus Status { get; set; }
    
    [BsonElement("statusReason")]
    public string StatusReason { get; set; }
}