namespace CashFlow.Consolidation.Api.Application
{
    public record TransactionCreated
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime ReferenceDate { get; set; }
        public string Direction { get; set; }
        public string TransactionType { get; set; }
        public decimal Value { get; set; }
    }
}