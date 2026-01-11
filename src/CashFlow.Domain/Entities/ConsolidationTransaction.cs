namespace CashFlow.Domain.Entities;

public class ConsolidationTransaction
{
    public Guid Id { get; set; }
    public Guid DailyClosureId { get; set; }
    public Guid CustomerId { get; set; }
    public DateOnly ReferenceDate { get; set; }
    public Direction Direction { get; set; }
    public decimal Value { get; set; }
}
