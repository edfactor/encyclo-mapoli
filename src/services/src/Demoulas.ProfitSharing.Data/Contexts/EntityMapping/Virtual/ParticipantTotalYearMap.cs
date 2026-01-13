using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Virtual;

internal sealed class ParticipantTotalYearMap : IEntityTypeConfiguration<ParticipantTotalYear>
{
    //This table is virtual in nature.  It uses the FromSql method to access data.
    public void Configure(EntityTypeBuilder<ParticipantTotalYear> builder)
    {
        builder.Metadata.SetIsTableExcludedFromMigrations(true);

        builder.HasKey(x => new { x.DemographicId, x.Ssn });
        builder.Property(x => x.Ssn)
            .HasColumnName("SSN")
            .IsRequired();

        builder.Property(x => x.DemographicId)
            .HasColumnName("DEMOGRAPHIC_ID")
            .IsRequired();

        builder.Property(x => x.Years)
            .HasColumnName("YEARS");
    }
}
