using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Customer.Data;

public class CustomerConfiguration : IEntityTypeConfiguration<Domain.Entities.Customer>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Customer> builder)
    {
        builder.ToTable("Customers");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id);
        builder.Property(x => x.FullName)
            .HasMaxLength(256)
            .IsRequired();
    }
}