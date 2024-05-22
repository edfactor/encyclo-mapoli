using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DemographicsMap : IEntityTypeConfiguration<Demographics>
{
    public void Configure(EntityTypeBuilder<Demographics> builder)
    {

        _ = builder.HasKey(e => e.BadgeNumber);
        _ = builder.ToTable("DEMOGRAPHICS");

        _ = builder.Property(e => e.BadgeNumber)
            .HasPrecision(7)
            .ValueGeneratedNever()
            .HasColumnName("DEM_BADGE");
    }
}
