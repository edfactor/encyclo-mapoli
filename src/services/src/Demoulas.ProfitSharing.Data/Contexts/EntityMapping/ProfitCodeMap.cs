using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class ProfitCodeMap : IEntityTypeConfiguration<ProfitCode>
{
    public void Configure(EntityTypeBuilder<ProfitCode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("PROFIT_CODE");

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever().HasColumnName("ID");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128).HasColumnName("NAME");
        builder.Property(x => x.Frequency).IsRequired().HasMaxLength(128).HasColumnName("FREQUENCY");

        builder.HasData(GetPredefinedProfitCodes());
    }

    //See https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/39944312/Quick+Guide+to+Profit+Sharing+Tables
    private static List<ProfitCode> GetPredefinedProfitCodes()
    {
        return
        [
            ProfitCode.Constants.IncomingContributions,
            ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal,
            ProfitCode.Constants.OutgoingForfeitures,
            ProfitCode.Constants.OutgoingDirectPayments,
            ProfitCode.Constants.OutgoingXferBeneficiary,
            ProfitCode.Constants.IncomingQdroBeneficiary,
            ProfitCode.Constants.Incoming100PercentVestedEarnings,
            ProfitCode.Constants.Outgoing100PercentVestedPayment
        ];
    }

}
