using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public class MissiveMap : IEntityTypeConfiguration<Missive>
{
    public void Configure(EntityTypeBuilder<Missive> builder)
    {
        builder.ToTable("MISSIVES");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id).HasColumnName("ID");

        builder.Property(f => f.Message)
            .HasColumnName("MESSAGE")
            .HasMaxLength(60);

        builder.Property(f => f.Description)
            .HasColumnName("DESCRIPTION")
            .HasMaxLength(250);

        builder.Property(f => f.Severity)
            .HasColumnName("SEVERITY")
            .HasMaxLength(16);

        builder.HasData(
            new Missive()
            {
                Id = Missive.Constants.VestingIncreasedOnCurrentBalance,
                Message = "** VESTING INCREASED ON   CURRENT BALANCE ( > 1000 HRS) **",
                Description = "The employee has between 2 and 7 years in Profit Sharing, has 1000+ plus hours towards Profit Sharing in the fiscal year, and has company contribution records under the new vesting schedule.",
                Severity = "Information"
            },
            new Missive()
            {
                Id = Missive.Constants.VestingIsNow100Percent,
                Message = "VEST IS NOW 100%, 65+/5 YRS",
                Description = "The Employee's Zero Contribution Flag is set at 6",
                Severity = "Information"
            },
            new Missive()
            {
                Id = Missive.Constants.EmployeeIsAlsoABeneficiary,
                Message = "Employee is also a Beneficiary",
                Description = "Employee is a beneficiary of another employee",
                Severity = "Information"
            },
            new Missive()
            {
                Id = Missive.Constants.BeneficiaryIsNotOnFile,
                Message = "Beneficiary not on file",
                Description = "The PSN you have entered was not found.  Re-enter using a valid PSN",
                Severity = "Error"
            },
            new Missive()
            {
                Id = Missive.Constants.EmployeeBadgeIsNotOnFile,
                Message = "Employee badge not on file",
                Description = "The Employee Badge Number you have entered is not found.  Re-enter using a valid badge number",
                Severity = "Error"
            },
            new Missive()
            {
                Id = Missive.Constants.EmployeeSsnIsNotOnFile,
                Message = "Employee SSN not on file",
                Description = "The Employee SSN you have entered is not on file or you don't have access.  Re-enter using a valid SSN",
                Severity = "Error"
            },
            new Missive()
            {
                Id = Missive.Constants.EmployeeMayBe100Percent,
                Message = "*** EMPLOYEE MAY BE 100% - CHECK DATES ***",
                Description = "The Employee's Zero Contribution Flag is set at 7",
                Severity = "Information"
            },
            new Missive()
            {
                Id = Missive.Constants.EmployeeUnder21WithBalance,
                Message = "Employee under 21 w/ balance > 0",
                Description = "Employee is currently under 21 and has a current or vested balance greater than zero.",
                Severity = "Information"
            }
        );
    }
}
