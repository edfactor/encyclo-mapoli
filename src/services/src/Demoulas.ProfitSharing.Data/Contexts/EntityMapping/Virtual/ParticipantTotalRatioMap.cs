using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Data.Entities.Virtual;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.VirtualTables;
internal sealed class ParticipantTotalRatioMap : IEntityTypeConfiguration<ParticipantTotalRatio>
{
    //This table is virtual in nature.  It uses the FromSql method to access data.
    public void Configure(EntityTypeBuilder<ParticipantTotalRatio> builder)
    {
        builder.Metadata.SetIsTableExcludedFromMigrations(true);

        builder.HasKey(x => x.Ssn);
        builder.Property(x => x.Ssn)
            .HasColumnName("SSN")
            .IsRequired();

        builder.Property(x => x.Ratio)
            .HasColumnName("RATIO");
    }
}
