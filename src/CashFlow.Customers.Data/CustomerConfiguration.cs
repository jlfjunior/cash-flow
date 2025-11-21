using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Customers.Data;

public class CustomerConfiguration : IEntityTypeConfiguration<Customers.Domain.Entities.Customer>
{
    public void Configure(EntityTypeBuilder<Customers.Domain.Entities.Customer> builder)
    {
        builder.ToTable("Customers");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id);
        builder.Property(x => x.FullName)
            .HasMaxLength(256)
            .IsRequired();
    }
}