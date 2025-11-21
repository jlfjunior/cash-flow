using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Transaction.Api.Infrastructure.Data;

public class TransactionConfiguration : IEntityTypeConfiguration<Transactions.Domain.Entities.Transaction>
{
    public void Configure(EntityTypeBuilder<Transactions.Domain.Entities.Transaction> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.AccountId);
        builder.Property(x => x.ReferenceDate);
        builder.Property(x => x.Value);
        builder.Property(x => x.Direction);
    }
}