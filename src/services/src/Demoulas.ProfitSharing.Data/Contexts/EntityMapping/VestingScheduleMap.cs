using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class VestingScheduleMap : IEntityTypeConfiguration<VestingSchedule>
{
    public void Configure(EntityTypeBuilder<VestingSchedule> builder)
    {
        builder.ToTable("VESTING_SCHEDULE");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .HasColumnName("NAME")
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .HasColumnName("DESCRIPTION");

        builder.Property(e => e.IsActive)
            .HasColumnName("IS_ACTIVE")
            .IsRequired();

        builder.Property(e => e.CreatedDate)
            .HasColumnName("CREATED_DATE")
            .IsRequired();

        builder.Property(e => e.EffectiveDate)
            .HasColumnName("EFFECTIVE_DATE")
            .IsRequired();

        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("UK_VESTING_SCHEDULE_NAME");

        builder.HasMany(e => e.Details)
            .WithOne(d => d.VestingSchedule)
            .HasForeignKey(d => d.VestingScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed historical vesting schedules
        builder.HasData(
            new VestingSchedule
            {
                Id = VestingSchedule.Constants.OldPlan,
                Name = "Old Plan (Pre-2007)",
                Description = "7-year vesting schedule with vesting starting at year 3. Used for employees whose first contribution was before 2007.",
                IsActive = true,
                CreatedDate = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Utc),
                EffectiveDate = new DateOnly(1917, 1, 1) // Market Basket founding date
            },
            new VestingSchedule
            {
                Id = VestingSchedule.Constants.NewPlan,
                Name = "New Plan (2007+)",
                Description = "6-year vesting schedule with vesting starting at year 2. Used for employees whose first contribution was 2007 or later.",
                IsActive = true,
                CreatedDate = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Utc),
                EffectiveDate = new DateOnly(2007, 1, 1) // New vesting rules effective date
            }
        );
    }
}
