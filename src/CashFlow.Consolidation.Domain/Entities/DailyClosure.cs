namespace CashFlow.Consolidation.Domain.Entities;

public class DailyClosure
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateOnly ReferenceDate { get; set; }
    public decimal Value { get; set; }
}