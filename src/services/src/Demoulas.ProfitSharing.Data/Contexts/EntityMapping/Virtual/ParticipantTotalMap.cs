using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Virtual;
internal sealed class ParticipantTotalMap : IEntityTypeConfiguration<ParticipantTotal>
{
    //This table is virtual in nature.  It uses the FromSql method to access data.
    public void Configure(EntityTypeBuilder<ParticipantTotal> builder)
    {
        builder.Metadata.SetIsTableExcludedFromMigrations(true);
        
        builder.HasKey(x => x.Ssn);
        builder.Property(x => x.Ssn)
            .HasColumnName("SSN")
            .IsRequired();

        builder.Property(x => x.TotalAmount)
            .HasColumnName("TOTAL")
            .HasPrecision(18, 2);
    }
}
