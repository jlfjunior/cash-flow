using System.Reflection;
using CashFlow.Transactions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Transactions.Data;

public class TransactionContext(DbContextOptions<TransactionContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}