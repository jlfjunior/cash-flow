namespace CashFlow.Customer.Api.Domain;

public class Customer
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }

    public Customer(string fullName)
    {
        Id = Guid.CreateVersion7();
        FullName = fullName;
    }
}