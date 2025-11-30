using CashFlow.Transactions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Transactions.Data.Configurations;

public class OutboxConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Type)
            .HasMaxLength(254)
            .IsRequired();
        
        builder.Property(o => o.Content)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.ProcessedAt);

        builder.Property(o => o.Status)
            .HasMaxLength(254)
            .HasConversion<string>()
            .IsRequired();
        
        builder.Property(o => o.StatusReason)
            .HasMaxLength(254)
            .IsRequired();

        builder.HasIndex(o => o.Status);
    }
}