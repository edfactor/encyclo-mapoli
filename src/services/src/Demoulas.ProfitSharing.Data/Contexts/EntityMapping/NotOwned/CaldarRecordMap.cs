using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities.NotOwned;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.NotOwned;
internal sealed class CaldarRecordMap : IEntityTypeConfiguration<CaldarRecord>
{
    public void Configure(EntityTypeBuilder<CaldarRecord> builder)
    {
        builder.ToTable("CALDAR_RECORD");

        builder.HasKey(e => e.AccWkendN)
            .HasName("PK_CALDAR_RECORD");

        builder.Property(e => e.AccWkendN)
            .HasColumnName("ACC_WKEND_N")
            .IsRequired();

        builder.Property(e => e.AccApWkend)
            .HasColumnName("ACC_APWKEND");

        builder.Property(e => e.AccWeekN)
            .HasColumnName("ACC_WEEKN");

        builder.Property(e => e.AccPeriod)
            .HasColumnName("ACC_PERIOD");

        builder.Property(e => e.AccQuarter)
            .HasColumnName("ACC_QUARTER");

        builder.Property(e => e.AccCalPeriod)
            .HasColumnName("ACC_CALPERIOD");

        builder.Property(e => e.AccCln60Week)
            .HasColumnName("ACC_CLN60_WEEK");

        builder.Property(e => e.AccCln60Period)
            .HasColumnName("ACC_CLN60_PERIOD");

        builder.Property(e => e.AccCln61Week)
            .HasColumnName("ACC_CLN61_WEEK");

        builder.Property(e => e.AccCln61Period)
            .HasColumnName("ACC_CLN61_PERIOD");

        builder.Property(e => e.AccCln7XWeek)
            .HasColumnName("ACC_CLN7X_WEEK");

        builder.Property(e => e.AccCln7XPeriod)
            .HasColumnName("ACC_CLN7X_PERIOD");

        builder.Property(e => e.AccCln6XWeek)
            .HasColumnName("ACC_CLN6X_WEEK");

        builder.Property(e => e.AccCln6XPeriod)
            .HasColumnName("ACC_CLN6X_PERIOD");

        builder.Property(e => e.AccWkend2N)
            .HasColumnName("ACC_WKEND2_N");

        builder.Property(e => e.AccAltKeyNum)
            .HasColumnName("ACC_ALT_KEY_NUM");

        // Constraints
        builder.HasIndex(e => e.AccApWkend)
            .IsUnique()
            .HasDatabaseName("CALDAR_RECORD_ACC_APWKEND_N");

        builder.HasIndex(e => e.AccWkend2N)
            .IsUnique()
            .HasDatabaseName("CALDAR_RECORD_ACC_WEDATE2");
    }
}
