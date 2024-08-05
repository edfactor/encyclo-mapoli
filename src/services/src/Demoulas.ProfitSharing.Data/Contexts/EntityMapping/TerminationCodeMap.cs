using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class TerminationCodeMap : IEntityTypeConfiguration<TerminationCode>
{
    public void Configure(EntityTypeBuilder<TerminationCode> builder)
    {
        builder.ToTable("TerminationCode");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(64)
            .HasColumnName("NAME")
            .IsRequired();

        builder.HasData(
            new TerminationCode { Id = TerminationCode.Constants.LeftOnOwn, Name = "Left On Own" },
            new TerminationCode { Id = TerminationCode.Constants.PersonalOrFamilyReason, Name = "Personal Or Family Reason" },
            new TerminationCode { Id = TerminationCode.Constants.CouldNotWorkAvailableHours, Name = "Could Not Work Available Hours" },
            new TerminationCode { Id = TerminationCode.Constants.Stealing, Name = "Stealing" },
            new TerminationCode { Id = TerminationCode.Constants.NotFollowingCompanyPolicy, Name = "Not Following Company Policy" },
            new TerminationCode { Id = TerminationCode.Constants.FMLAExpired, Name = "FMLA Expired" },
            new TerminationCode { Id = TerminationCode.Constants.TerminatedPrivate, Name = "Terminated Private" },
            new TerminationCode { Id = TerminationCode.Constants.JobAbandonment, Name = "Job Abandonment" },
            new TerminationCode { Id = TerminationCode.Constants.HealthReasonsNonFMLA, Name = "Health Reasons Non-FMLA" },
            new TerminationCode { Id = TerminationCode.Constants.LayoffNoWork, Name = "Layoff No Work" },
            new TerminationCode { Id = TerminationCode.Constants.Military, Name = "Military" },
            new TerminationCode { Id = TerminationCode.Constants.SchoolOrSports, Name = "School Or Sports" }
        );
    }
}
