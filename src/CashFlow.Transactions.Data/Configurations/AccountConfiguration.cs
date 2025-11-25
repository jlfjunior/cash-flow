using CashFlow.Transactions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Transactions.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Balance);
        builder.Property(x => x.CustomerId);

        builder.HasMany(x => x.Transactions)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId);
    }
}