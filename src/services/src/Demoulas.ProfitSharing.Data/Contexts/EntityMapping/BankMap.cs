using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public sealed class BankMap : IEntityTypeConfiguration<Bank>
{
    public void Configure(EntityTypeBuilder<Bank> builder)
    {
        _ = builder.ToTable("BANK");

        _ = builder.HasKey(x => x.Id);

        _ = builder.Property(x => x.Id)
            .HasColumnName("ID")
            .HasColumnType("NUMBER(10)")
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("BANK_SEQ.NEXTVAL");

        _ = builder.Property(x => x.RoutingNumber)
            .HasMaxLength(9)
            .HasColumnName("ROUTING_NUMBER");

        _ = builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired()
            .HasColumnName("NAME");

        _ = builder.Property(x => x.OfficeType)
            .HasMaxLength(50)
            .HasColumnName("OFFICE_TYPE");

        _ = builder.Property(x => x.City)
            .HasMaxLength(100)
            .HasColumnName("CITY");

        _ = builder.Property(x => x.State)
            .HasMaxLength(2)
            .HasColumnName("STATE");

        _ = builder.Property(x => x.Phone)
            .HasMaxLength(24)
            .HasColumnName("PHONE");

        _ = builder.Property(x => x.Status)
            .HasMaxLength(24)
            .HasColumnName("STATUS");

        _ = builder.Property(x => x.FedAchChangeDate)
            .HasColumnType("DATE")
            .HasColumnName("FEDACH_CHANGE_DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(x => x.ServicingFedRoutingNumber)
            .HasMaxLength(9)
            .HasColumnName("SERVICING_FED_ROUTING_NUMBER");

        _ = builder.Property(x => x.ServicingFedAddress)
            .HasMaxLength(200)
            .HasColumnName("SERVICING_FED_ADDRESS");

        _ = builder.Property(x => x.FedwireTelegraphicName)
            .HasMaxLength(50)
            .HasColumnName("FEDWIRE_TELEGRAPHIC_NAME");

        _ = builder.Property(x => x.FedwireLocation)
            .HasMaxLength(100)
            .HasColumnName("FEDWIRE_LOCATION");

        _ = builder.Property(x => x.FedwireRevisionDate)
            .HasColumnType("DATE")
            .HasColumnName("FEDWIRE_REVISION_DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(x => x.IsDisabled)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("IS_DISABLED");

        _ = builder.Property(x => x.CreatedAtUtc)
            .IsRequired()
            .HasColumnName("CREATED_AT_UTC")
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .HasDefaultValueSql("SYSTIMESTAMP");

        _ = builder.Property(x => x.CreatedBy)
            .HasMaxLength(96)
            .HasColumnName("CREATED_BY");

        _ = builder.Property(x => x.ModifiedAtUtc)
            .HasColumnName("MODIFIED_AT_UTC")
            .HasColumnType("TIMESTAMP WITH TIME ZONE");

        _ = builder.Property(x => x.ModifiedBy)
            .HasMaxLength(96)
            .HasColumnName("MODIFIED_BY");

        // Navigation property - one bank to many accounts
        _ = builder.HasMany(x => x.Accounts)
            .WithOne(x => x.Bank)
            .HasForeignKey(x => x.BankId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index on Name for lookups
        _ = builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_BANK_NAME");

        // Index on IsDisabled for filtering active banks
        _ = builder.HasIndex(x => x.IsDisabled)
            .HasDatabaseName("IX_BANK_IS_DISABLED");

        // Index on RoutingNumber for backwards compatibility lookups
        _ = builder.HasIndex(x => x.RoutingNumber)
            .HasDatabaseName("IX_BANK_ROUTING_NUMBER");

        _ = builder.HasData(GetSeedData());
    }

    private static List<Bank> GetSeedData()
    {
        var now = DateTimeOffset.UtcNow;
        return
        [
            new()
            {
                Id = 1,
                RoutingNumber = "026004297",
                Name = "Newtek Bank, NA",
                OfficeType = "Main Office",
                City = "Lake Success",
                State = "NY",
                Phone = "516-254-7586",
                Status = "Active",
                FedAchChangeDate = new DateOnly(2024, 7, 30),
                ServicingFedRoutingNumber = "021001208",
                ServicingFedAddress = "100 Orchard Street, East Rutherford, NJ",
                FedwireTelegraphicName = "NEWTEK BANK, NA",
                FedwireLocation = "Miami, FL",
                FedwireRevisionDate = new DateOnly(2023, 7, 6),
                IsDisabled = false,
                CreatedAtUtc = now,
                CreatedBy = "SYSTEM",
                ModifiedAtUtc = null,
                ModifiedBy = null,
            }
        ];
    }
}
