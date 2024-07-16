using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class ProfitCodeMap : IEntityTypeConfiguration<ProfitCode>
{
    public void Configure(EntityTypeBuilder<ProfitCode> builder)
    {
        builder.HasKey(x => x.Code);
        builder.ToTable("ProfitCode");

        builder.Property(x => x.Code).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.Definition).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Frequency).IsRequired().HasMaxLength(128);

        builder.HasData(GetPredefinedProfitCodes());
    }

    //See https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/39944312/Quick+Guide+to+Profit+Sharing+Tables
    private List<ProfitCode> GetPredefinedProfitCodes()
    {
        return
        [
            new ProfitCode() {Code=0,Definition="Incoming contributions, forfeitures, earnings", Frequency="Year-end only"},
            new ProfitCode() {Code=1,Definition="Outgoing payments (not rollovers or direct payments) - Partial withdrawal", Frequency="Multiple Times"},
            new ProfitCode() {Code=2,Definition="Outgoing forfeitures", Frequency="Multiple Times"},
            new ProfitCode() {Code=3,Definition="Outgoing direct payments / rollover payments", Frequency="Multiple Times"},
            new ProfitCode() {Code=5,Definition="Outgoing XFER beneficiary / QDRO allocation (beneficiary payment)", Frequency="Once"},
            new ProfitCode() {Code=6,Definition="Incoming QDRO beneficiary allocation  (beneficiary receipt)", Frequency="Once"},
            new ProfitCode() {Code=8,Definition="Incoming \"100% vested\" earnings", Frequency="Usually year-end, unless there is special processing – i.e. Class Action settlement.  Earnings are 100% vested."},
            new ProfitCode() {Code=9,Definition="Outgoing payment from 100% vesting amount (payment of ETVA funds)", Frequency="Multiple Times"}
        ];
    }
}
