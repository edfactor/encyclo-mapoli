using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class ParticipantTotalVestingBalanceMap : IEntityTypeConfiguration<ParticipantTotalVestingBalance>
{
    //This table is virtual in nature.  It uses the FromSql method to access data.
    public void Configure(EntityTypeBuilder<ParticipantTotalVestingBalance> builder)
    {
        builder.Metadata.SetIsTableExcludedFromMigrations(true);

        builder.HasKey(x => x.Ssn);
        builder.Property(x => x.Ssn)
            .HasColumnName("SSN")
            .IsRequired();

        builder.Property(x => x.VestedBalance)
            .HasColumnName("VESTEDBALANCE");
        builder.Property(x => x.CurrentBalance)
            .HasColumnName("CURRENTBALANCE");
        builder.Property(x => x.VestingPercent)
            .HasColumnName("RATIO");

        builder.Property(x => x.YearsInPlan)
            .HasColumnName("YEARS");
    }
}
