using CashFlow.Customers.Data.Configurations;
using CashFlow.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Customers.Data;

public class CustomerContext : DbContext
{
    public CustomerContext(DbContextOptions<CustomerContext> options)
    : base(options)
    {
        
    }
    
    public DbSet<Customer>  Customers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
    }
}