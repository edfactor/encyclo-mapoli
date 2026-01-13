using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class RmdsFactorByAgeMap : IEntityTypeConfiguration<RmdsFactorByAge>
{
    public void Configure(EntityTypeBuilder<RmdsFactorByAge> builder)
    {
        _ = builder.ToTable("RMDS_FACTOR_BY_AGE");
        _ = builder.HasKey(c => c.Age);

        _ = builder.Property(c => c.Age)
            .IsRequired()
            .HasColumnName("AGE")
            .HasPrecision(3);

        _ = builder.Property(c => c.Factor)
            .IsRequired()
            .HasColumnName("FACTOR")
            .HasPrecision(4, 1);  // DECIMAL(4,1) supports values like 26.5 (1 decimal place)

        // Seed data: IRS Required Minimum Distribution life expectancy divisors for ages 73-99
        // Source: IRS Publication 590-B, Uniform Lifetime Table
        // These are life expectancy divisors (years), not percentages
        // Formula: RMD = Account Balance ÷ Factor
        _ = builder.HasData(
            new RmdsFactorByAge { Age = 73, Factor = 26.5m },
            new RmdsFactorByAge { Age = 74, Factor = 25.5m },
            new RmdsFactorByAge { Age = 75, Factor = 24.6m },
            new RmdsFactorByAge { Age = 76, Factor = 23.7m },
            new RmdsFactorByAge { Age = 77, Factor = 22.9m },
            new RmdsFactorByAge { Age = 78, Factor = 22.0m },
            new RmdsFactorByAge { Age = 79, Factor = 21.1m },
            new RmdsFactorByAge { Age = 80, Factor = 20.2m },
            new RmdsFactorByAge { Age = 81, Factor = 19.4m },
            new RmdsFactorByAge { Age = 82, Factor = 18.5m },
            new RmdsFactorByAge { Age = 83, Factor = 17.7m },
            new RmdsFactorByAge { Age = 84, Factor = 16.8m },
            new RmdsFactorByAge { Age = 85, Factor = 16.0m },
            new RmdsFactorByAge { Age = 86, Factor = 15.2m },
            new RmdsFactorByAge { Age = 87, Factor = 14.4m },
            new RmdsFactorByAge { Age = 88, Factor = 13.7m },
            new RmdsFactorByAge { Age = 89, Factor = 12.9m },
            new RmdsFactorByAge { Age = 90, Factor = 12.2m },
            new RmdsFactorByAge { Age = 91, Factor = 11.5m },
            new RmdsFactorByAge { Age = 92, Factor = 10.8m },
            new RmdsFactorByAge { Age = 93, Factor = 10.1m },
            new RmdsFactorByAge { Age = 94, Factor = 9.5m },
            new RmdsFactorByAge { Age = 95, Factor = 8.9m },
            new RmdsFactorByAge { Age = 96, Factor = 8.4m },
            new RmdsFactorByAge { Age = 97, Factor = 7.8m },
            new RmdsFactorByAge { Age = 98, Factor = 7.3m },
            new RmdsFactorByAge { Age = 99, Factor = 6.8m }
        );
    }
}
