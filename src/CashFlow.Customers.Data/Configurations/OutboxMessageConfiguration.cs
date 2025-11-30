using CashFlow.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Customers.Data.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
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