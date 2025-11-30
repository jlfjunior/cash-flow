namespace CashFlow.Customers.Domain.Entities;

public enum OutboxStatus
{
    Pending,
    Processed,
    Failed
}