using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DistributionRequestMap : ModifiedBaseMap<DistributionRequest>
{
    public override void Configure(EntityTypeBuilder<DistributionRequest> builder)
    {
        base.Configure(builder);

        builder.ToTable("DISTRIBUTION_REQUEST");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasPrecision(9).HasColumnName("ID").ValueGeneratedOnAdd();
        builder.Property(c => c.DemographicId).HasPrecision(9).HasColumnName("DEMOGRAPHIC_ID");
        builder.Property(c => c.ReasonId).HasColumnName("REASON_ID");
        builder.Property(c => c.StatusId).HasColumnName("STATUS_ID");
        builder.Property(c => c.TaxCodeId).HasColumnName("TAX_CODE_ID");
        builder.Property(c => c.TypeId).HasColumnName("TYPE_ID");
        builder.Property(d => d.ReasonText).HasColumnName("REASON_TEXT").HasMaxLength(250);
        builder.Property(d => d.ReasonOtherText).HasColumnName("REASON_OTHER").HasMaxLength(500);
        builder.Property(d => d.AmountRequested).HasColumnName("AMOUNT_REQUESTED").HasPrecision(10, 2);
        builder.Property(d => d.AmountAuthorized).HasColumnName("AMOUNT_AUTHORIZED").HasPrecision(10, 2);
        builder.Property(d => d.DateRequested).HasColumnName("DATE_REQUESTED").HasColumnType("Date").HasConversion<DateOnlyConverter>();
        builder.Property(d => d.DateDecided).HasColumnName("DATE_DECIDED").HasColumnType("Date").HasConversion<DateOnlyConverter>();

        builder.HasOne(e => e.Reason).WithMany().HasForeignKey(p => p.ReasonId);
        builder.HasOne(e => e.Status).WithMany().HasForeignKey(p => p.StatusId);
        builder.HasOne(e => e.Type).WithMany().HasForeignKey(p => p.TypeId);
        builder.HasOne(e => e.TaxCode).WithMany().HasForeignKey(p => p.TaxCodeId);
    }
}
