using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Transactions.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Domain.Entities.Transaction>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Transaction> builder)
    {
        builder.ToTable("Transactions");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.AccountId);
        builder.Property(x => x.ReferenceDate);
        builder.Property(x => x.Value);
        builder.Property(x => x.Direction);
        
        builder.HasDiscriminator(x => x.TransactionType)
            .HasValue<Domain.Entities.DepositTransaction>(Domain.Entities.TransactionType.Deposit)
            .HasValue<Domain.Entities.WithdrawTransaction>(Domain.Entities.TransactionType.Withdraw)
            .HasValue<Domain.Entities.BillPaymentTransaction>(Domain.Entities.TransactionType.BillPayment);
    }
}