using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DistributionThirdPartyPayeeMap : IEntityTypeConfiguration<DistributionThirdPartyPayee>
{
    public void Configure(EntityTypeBuilder<DistributionThirdPartyPayee> builder)
    {
        builder.ToTable("DISTRIBUTION_PAYEE");
        builder.HasKey(d => d.Id);
        
        builder.HasIndex(d => d.Ssn, "IX_SSN");

        builder.Property(d => d.Id).ValueGeneratedOnAdd();
        builder.Property(d => d.Ssn).HasColumnName("SSN").HasPrecision(9);
        
        builder.Property(d => d.Payee).HasMaxLength(35).HasColumnName("PAYEE");
        builder.Property(d => d.Name).HasMaxLength(35).HasColumnName("NAME");
        builder.Property(d => d.Account).HasMaxLength(30).HasColumnName("ACCOUNT");
        builder.OwnsOne(d => d.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(30).HasColumnName("STREET");
            address.Property(a => a.Street2).HasMaxLength(30).HasColumnName("STREET2");
            address.Property(a => a.Street3).HasMaxLength(30).HasColumnName("STREET3");
            address.Property(a => a.Street4).HasMaxLength(30).HasColumnName("STREET4");
            address.Property(a => a.City).HasMaxLength(25).HasColumnName("CITY");
            address.Property(a => a.State).HasMaxLength(3).HasColumnName("STATE");
            address.Property(a => a.PostalCode).HasMaxLength(9).HasColumnName("POSTAL_CODE");
            address.Property(a => a.CountryIso).HasMaxLength(2).HasColumnName("COUNTRY_ISO")
                .HasDefaultValue(Country.Constants.Us);
            address.HasOne<Country>()
                .WithMany()
                .HasForeignKey(o => o.CountryIso);
        });
        

        
    }
}
