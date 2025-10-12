namespace CashFlow.Transaction.Api.Domain.Entities;

public class Account : Entity
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; }

    public Account(Guid customerId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Balance = decimal.Zero;
    }
}