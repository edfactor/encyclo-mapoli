using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class VestingScheduleDetailMap : IEntityTypeConfiguration<VestingScheduleDetail>
{
    public void Configure(EntityTypeBuilder<VestingScheduleDetail> builder)
    {
        builder.ToTable("VESTING_SCHEDULE_DETAIL");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .IsRequired();

        builder.Property(e => e.VestingScheduleId)
            .HasColumnName("VESTING_SCHEDULE_ID")
            .IsRequired();

        builder.Property(e => e.YearsOfService)
            .HasColumnName("YEARS_OF_SERVICE")
            .IsRequired();

        builder.Property(e => e.VestingPercent)
            .HasColumnName("VESTING_PERCENT")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.HasIndex(e => e.VestingScheduleId)
            .HasDatabaseName("IX_VESTING_SCHEDULE_DETAIL_VESTING_SCHEDULE_ID");

        builder.HasIndex(e => new { e.VestingScheduleId, e.YearsOfService })
            .IsUnique()
            .HasDatabaseName("UK_VSD_SCHEDULE_YEARS");

        builder.HasOne(e => e.VestingSchedule)
            .WithMany(s => s.Details)
            .HasForeignKey(e => e.VestingScheduleId)
            .HasConstraintName("FK_VSD_SCHEDULE");

        // Seed Old Plan (Pre-2007) vesting schedule
        // Years: 0=0%, 1=0%, 2=0%, 3=20%, 4=40%, 5=60%, 6=80%, 7+=100%
        builder.HasData(
            new VestingScheduleDetail { Id = 1, VestingScheduleId = VestingSchedule.Constants.OldPlan, YearsOfService = 0, VestingPercent = 0 },
            new VestingScheduleDetail { Id = 2, VestingScheduleId = VestingSchedule.Constants.OldPlan, YearsOfService = 1, VestingPercent = 0 },
            new VestingScheduleDetail { Id = 3, VestingScheduleId = VestingSchedule.Constants.OldPlan, YearsOfService = 2, VestingPercent = 0 },
            new VestingScheduleDetail { Id = 4, VestingScheduleId = VestingSchedule.Constants.OldPlan, YearsOfService = 3, VestingPercent = 20 },
            new VestingScheduleDetail { Id = 5, VestingScheduleId = VestingSchedule.Constants.OldPlan, YearsOfService = 4, VestingPercent = 40 },
            new VestingScheduleDetail { Id = 6, VestingScheduleId = VestingSchedule.Constants.OldPlan, YearsOfService = 5, VestingPercent = 60 },
            new VestingScheduleDetail { Id = 7, VestingScheduleId = VestingSchedule.Constants.OldPlan, YearsOfService = 6, VestingPercent = 80 },
            new VestingScheduleDetail { Id = 8, VestingScheduleId = VestingSchedule.Constants.OldPlan, YearsOfService = 7, VestingPercent = 100 },

            // Seed New Plan (2007+) vesting schedule
            // Years: 0=0%, 1=0%, 2=20%, 3=40%, 4=60%, 5=80%, 6+=100%
            new VestingScheduleDetail { Id = 9, VestingScheduleId = VestingSchedule.Constants.NewPlan, YearsOfService = 0, VestingPercent = 0 },
            new VestingScheduleDetail { Id = 10, VestingScheduleId = VestingSchedule.Constants.NewPlan, YearsOfService = 1, VestingPercent = 0 },
            new VestingScheduleDetail { Id = 11, VestingScheduleId = VestingSchedule.Constants.NewPlan, YearsOfService = 2, VestingPercent = 20 },
            new VestingScheduleDetail { Id = 12, VestingScheduleId = VestingSchedule.Constants.NewPlan, YearsOfService = 3, VestingPercent = 40 },
            new VestingScheduleDetail { Id = 13, VestingScheduleId = VestingSchedule.Constants.NewPlan, YearsOfService = 4, VestingPercent = 60 },
            new VestingScheduleDetail { Id = 14, VestingScheduleId = VestingSchedule.Constants.NewPlan, YearsOfService = 5, VestingPercent = 80 },
            new VestingScheduleDetail { Id = 15, VestingScheduleId = VestingSchedule.Constants.NewPlan, YearsOfService = 6, VestingPercent = 100 }
        );
    }
}
