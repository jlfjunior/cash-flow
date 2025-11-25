using CashFlow.Transactions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Transactions.Data;

public class TransactionContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
}