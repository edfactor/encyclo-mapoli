using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

/// <summary>
/// Entity Framework Core configuration for BankAccount entity.
/// Maps bank account details including routing numbers, account numbers, and account status.
/// </summary>
internal sealed class BankAccountMap : ModifiedBaseMap<BankAccount>
{
    public override void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        base.Configure(builder); // Configure ModifiedBase properties

        _ = builder.ToTable("BANK_ACCOUNT");

        _ = builder.HasKey(x => x.Id);

        _ = builder.Property(x => x.Id)
            .HasColumnName("ID")
            .HasColumnType("NUMBER(10)")
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("BANK_ACCOUNT_SEQ.NEXTVAL");

        _ = builder.Property(x => x.BankId)
            .HasColumnName("BANK_ID")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        _ = builder.Property(x => x.RoutingNumber)
            .HasMaxLength(9)
            .IsRequired()
            .HasColumnName("ROUTING_NUMBER");

        _ = builder.Property(x => x.AccountNumber)
            .HasMaxLength(34)
            .IsRequired()
            .HasColumnName("ACCOUNT_NUMBER");

        _ = builder.Property(x => x.AccountName)
            .HasMaxLength(200)
            .HasColumnName("ACCOUNT_NAME");

        _ = builder.Property(x => x.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("IS_PRIMARY");

        _ = builder.Property(x => x.IsDisabled)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("IS_DISABLED");

        _ = builder.Property(x => x.ServicingFedRoutingNumber)
            .HasMaxLength(9)
            .HasColumnName("SERVICING_FED_ROUTING_NUMBER");

        _ = builder.Property(x => x.ServicingFedAddress)
            .HasMaxLength(255)
            .HasColumnName("SERVICING_FED_ADDRESS");

        _ = builder.Property(x => x.FedwireTelegraphicName)
            .HasMaxLength(100)
            .HasColumnName("FEDWIRE_TELEGRAPHIC_NAME");

        _ = builder.Property(x => x.FedwireLocation)
            .HasMaxLength(100)
            .HasColumnName("FEDWIRE_LOCATION");

        _ = builder.Property(x => x.FedAchChangeDate)
            .HasColumnName("FED_ACH_CHANGE_DATE")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(x => x.FedwireRevisionDate)
            .HasColumnName("FEDWIRE_REVISION_DATE")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(x => x.Notes)
            .HasMaxLength(1000)
            .HasColumnName("NOTES");

        _ = builder.Property(x => x.EffectiveDate)
            .HasColumnName("EFFECTIVE_DATE")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(x => x.DiscontinuedDate)
            .HasColumnName("DISCONTINUED_DATE")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        // Foreign key relationship to Bank
        _ = builder.HasOne(x => x.Bank)
            .WithMany(x => x.Accounts)
            .HasForeignKey(x => x.BankId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index on BankId for efficient lookups of all accounts for a bank
        _ = builder.HasIndex(x => x.BankId)
            .HasDatabaseName("IX_BANK_ACCOUNT_BANK_ID");

        // Index on RoutingNumber for account lookups by routing number
        _ = builder.HasIndex(x => x.RoutingNumber)
            .HasDatabaseName("IX_BANK_ACCOUNT_ROUTING_NUMBER");

        // Compound index for finding primary account per bank
        _ = builder.HasIndex(x => new { x.BankId, x.IsPrimary })
            .HasDatabaseName("IX_BANK_ACCOUNT_BANK_PRIMARY");

        // Index on IsDisabled for filtering active accounts
        _ = builder.HasIndex(x => x.IsDisabled)
            .HasDatabaseName("IX_BANK_ACCOUNT_IS_DISABLED");

        _ = builder.HasData(GetSeedData());
    }

    private static List<BankAccount> GetSeedData()
    {
        var now = DateTimeOffset.UtcNow;
        return
        [
            new()
            {
                Id = 1,
                BankId = 1, // References Newtek Bank
                RoutingNumber = "026004297",
                AccountNumber = "PLACEHOLDER", // Will be updated with actual account number
                AccountName = "Profit Sharing Distribution Account",
                IsPrimary = true,
                IsDisabled = false,
                CreatedAtUtc = now,
                CreatedBy = "SYSTEM",
                ModifiedAtUtc = null,
                ModifiedBy = null,
            }
        ];
    }
}
