namespace CashFlow.Transactions.Domain.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public OutboxStatus Status { get; set; }
    public string StatusReason { get; set; }
}