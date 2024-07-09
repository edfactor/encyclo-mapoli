using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class EmployeeTypeMap : IEntityTypeConfiguration<EmployeeType>
{
    public void Configure(EntityTypeBuilder<EmployeeType> builder)
    {
        builder.ToTable("EmployeeType");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(30);

        builder.HasMany(b => b.Profits).WithOne(p => p.EmployeeType);

        builder.HasData(
            new PayFrequency { Id = EmployeeType.Constants.Not_New_LastYear, Name = "NOT New in plan last year" },
            new PayFrequency { Id = EmployeeType.Constants.New_LastYear, Name = "New last year" }
        );
    }
}
