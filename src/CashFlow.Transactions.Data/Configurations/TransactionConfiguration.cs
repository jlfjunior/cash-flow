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
        builder.Property(x => x.TransactionType);
    }
}