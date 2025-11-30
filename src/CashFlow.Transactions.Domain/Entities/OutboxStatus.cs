namespace CashFlow.Transactions.Domain.Entities;

public enum OutboxStatus
{
    Pending,
    Processed,
    Failed
}