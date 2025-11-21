using Microsoft.EntityFrameworkCore;

namespace CashFlow.Customer.Data;

public class CustomerContext : DbContext
{
    public CustomerContext(DbContextOptions<CustomerContext> options)
    : base(options)
    {
        
    }
    
    public DbSet<Domain.Entities.Customer>  Customers { get; set; }
}