using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class PayProfitMap : IEntityTypeConfiguration<PayProfit>
{
    public void Configure(EntityTypeBuilder<PayProfit> builder)
    {
        builder.ToTable("PayProfit");

        builder.HasKey(e => e.EmployeeBadge);

        builder.Property(e => e.EmployeeBadge)
            .HasPrecision(7)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(e => e.EmployeeSSN)
            .HasMaxLength(9)
            .IsRequired();


        builder.HasOne(e => e.Enrollment).WithMany(p => p.Profits);
    }
}
