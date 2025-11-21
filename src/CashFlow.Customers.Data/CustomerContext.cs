using Microsoft.EntityFrameworkCore;

namespace CashFlow.Customers.Data;

public class CustomerContext : DbContext
{
    public CustomerContext(DbContextOptions<CustomerContext> options)
    : base(options)
    {
        
    }
    
    public DbSet<Customers.Domain.Entities.Customer>  Customers { get; set; }
}