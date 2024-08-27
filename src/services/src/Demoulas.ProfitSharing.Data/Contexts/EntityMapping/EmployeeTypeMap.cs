using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class EmployeeTypeMap : IEntityTypeConfiguration<EmployeeType>
{
    public void Configure(EntityTypeBuilder<EmployeeType> builder)
    {
        builder.ToTable("EMPLOYEE_TYPE");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("ID");
        builder.Property(c => c.Name).IsRequired().HasMaxLength(30).HasColumnName("NAME");

        builder.HasMany(b => b.Profits).WithOne(p => p.EmployeeType);

        builder.HasData(
            new EmployeeType { Id = EmployeeType.Constants.NotNewLastYear, Name = "NOT New in plan last year" },
            new EmployeeType { Id = EmployeeType.Constants.NewLastYear, Name = "New last year" }
        );
    }
}
