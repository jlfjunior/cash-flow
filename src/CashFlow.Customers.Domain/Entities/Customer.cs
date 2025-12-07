namespace CashFlow.Customers.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    
    public string FullName { get; private set; }

    public Customer(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required");
        
        Id = Guid.CreateVersion7();
        FullName = fullName;
    }

    public void WithFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required");
        
        FullName = fullName;
    }
}