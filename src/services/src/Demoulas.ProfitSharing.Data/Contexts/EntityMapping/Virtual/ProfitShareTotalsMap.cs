using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Virtual;
internal sealed class ProfitShareTotalsMap : IEntityTypeConfiguration<ProfitShareTotal>
{
    //This table is virtual in nature.  It uses the FromSql method to access data.
    public void Configure(EntityTypeBuilder<ProfitShareTotal> builder)
    {
        builder.Metadata.SetIsTableExcludedFromMigrations(true);

        builder.HasNoKey();

        builder.Property(x => x.WagesTotal)
            .HasColumnName("WAGES_TOTAL")
            .HasPrecision(18, 2);

        builder.Property(x => x.HoursTotal)
            .HasColumnName("HOURS_TOTAL")
            .HasPrecision(18, 2);

        builder.Property(x => x.PointsTotal)
            .HasColumnName("POINTS_TOTAL")
            .HasPrecision(18, 2);

        builder.Property(x => x.TerminatedWagesTotal)
            .HasColumnName("TERMINATED_WAGES_TOTAL")
            .HasPrecision(18, 2);

        builder.Property(x => x.TerminatedHoursTotal)
            .HasColumnName("TERMINATED_HOURS_TOTAL")
            .HasPrecision(18, 2);

        builder.Property(x => x.TerminatedPointsTotal)
            .HasColumnName("TERMINATED_POINTS_TOTAL")
            .HasPrecision(18, 2);
        

        builder.Property(x => x.NumberOfEmployees)
            .HasColumnName("NUMBER_OF_EMPLOYEES");

        builder.Property(x => x.NumberOfNewEmployees)
            .HasColumnName("NUMBER_OF_NEW_EMPLOYEES");

        builder.Property(x => x.NumberOfEmployeesUnder21)
            .HasColumnName("NUMBER_OF_EMPLOYEES_UNDER21");

        builder.Property(x => x.BalanceTotal)
            .HasColumnName("BALANCE_TOTAL")
            .HasPrecision(18, 2);

        builder.Property(x => x.TerminatedBalanceTotal)
            .HasColumnName("TERMINATED_BALANCE_TOTAL")
            .HasPrecision(18, 2);
    }
}
