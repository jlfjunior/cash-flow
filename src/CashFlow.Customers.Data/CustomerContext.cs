using CashFlow.Customers.Data.Configurations;
using CashFlow.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Customers.Data;

public class CustomerContext(DbContextOptions<CustomerContext> options) : DbContext(options)
{
    public DbSet<Customer>  Customers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
    }
}