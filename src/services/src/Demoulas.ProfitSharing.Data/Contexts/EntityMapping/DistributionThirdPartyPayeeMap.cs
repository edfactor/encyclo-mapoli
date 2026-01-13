using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DistributionThirdPartyPayeeMap : IEntityTypeConfiguration<DistributionThirdPartyPayee>
{
    public void Configure(EntityTypeBuilder<DistributionThirdPartyPayee> builder)
    {
        _ = builder.ToTable("DISTRIBUTION_THIRDPARTY_PAYEE");
        _ = builder.HasKey(d => d.Id);

        _ = builder.Property(d => d.Id).ValueGeneratedOnAdd();

        _ = builder.Property(d => d.Payee).HasMaxLength(64).HasColumnName("PAYEE");
        _ = builder.Property(d => d.Name).HasMaxLength(84).HasColumnName("NAME");
        _ = builder.OwnsOne(d => d.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(56).HasColumnName("STREET");
            address.Property(a => a.Street2).HasMaxLength(56).HasColumnName("STREET2");
            address.Property(a => a.Street3).HasMaxLength(56).HasColumnName("STREET3");
            address.Property(a => a.Street4).HasMaxLength(56).HasColumnName("STREET4");
            address.Property(a => a.City).HasMaxLength(36).HasColumnName("CITY");
            address.Property(a => a.State).HasMaxLength(3).HasColumnName("STATE");
            address.Property(a => a.PostalCode).HasMaxLength(9).HasColumnName("POSTAL_CODE");
            address.Property(a => a.CountryIso).HasMaxLength(2).HasColumnName("COUNTRY_ISO")
                .HasDefaultValue(Country.Constants.Us);
            address.HasOne<Country>()
                .WithMany()
                .HasForeignKey(o => o.CountryIso);
        });

        _ = builder.Property(d => d.Memo).HasColumnName("MEMO").HasMaxLength(128);

    }
}
