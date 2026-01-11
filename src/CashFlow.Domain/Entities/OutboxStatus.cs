namespace CashFlow.Domain.Entities;

public enum OutboxStatus
{
    Pending,
    Processed,
    Failed
}
