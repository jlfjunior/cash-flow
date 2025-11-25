using CashFlow.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Customers.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id);
        builder.Property(x => x.FullName)
            .HasMaxLength(256)
            .IsRequired();
    }
}