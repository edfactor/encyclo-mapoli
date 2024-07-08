using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class TerminationCodeMap : IEntityTypeConfiguration<TerminationCode>
{
    public void Configure(EntityTypeBuilder<TerminationCode> builder)
    {
        builder.ToTable("TerminationCode");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasData(
            new TerminationCode { Id = TerminationCode.Constants.LeftOnOwn, Description = "Left On Own" },
            new TerminationCode { Id = TerminationCode.Constants.PersonalOrFamilyReason, Description = "Personal Or Family Reason" },
            new TerminationCode { Id = TerminationCode.Constants.CouldNotWorkAvailableHours, Description = "Could Not Work Available Hours" },
            new TerminationCode { Id = TerminationCode.Constants.Stealing, Description = "Stealing" },
            new TerminationCode { Id = TerminationCode.Constants.NotFollowingCompanyPolicy, Description = "Not Following Company Policy" },
            new TerminationCode { Id = TerminationCode.Constants.FMLAExpired, Description = "FMLA Expired" },
            new TerminationCode { Id = TerminationCode.Constants.TerminatedPrivate, Description = "Terminated Private" },
            new TerminationCode { Id = TerminationCode.Constants.JobAbandonment, Description = "Job Abandonment" },
            new TerminationCode { Id = TerminationCode.Constants.HealthReasonsNonFMLA, Description = "Health Reasons Non-FMLA" },
            new TerminationCode { Id = TerminationCode.Constants.LayoffNoWork, Description = "Layoff No Work" }
            );
    }
}
