namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public record EmployeeFinancials
{
    public string Name { get; set; }
    public long EmployeeId { get; set; } // PIC 9(7).
    public int Ssn { get; set; } // PIC 9(9).
    public long EnrolledId { get; set; } // PIC 99.
    public long YearsInPlan { get; set; } // PIC 99.
    public decimal CurrentAmount { get; set; } // PIC S9(7)V99.
    public long EmployeeTypeId { get; set; } // PIC 99.
    public long PointsEarned { get; set; } // PIC S9(5).
    public decimal Contributions { get; set; } // PIC S9(7)V99.
    public decimal IncomeForfeiture { get; set; } // PIC S9(7)V99.
    public decimal Earnings { get; set; } // PIC S9(7)V99.
    public decimal EtvaAfterVestingRules { get; set; } // PAYPROFIT.PY_PS_ETVA
    public decimal EarningsOnEtva { get; set; } // PIC S9(7)V99.
    public decimal SecondaryEarnings { get; set; } // PIC S9(7)V99.
    public decimal SecondaryEtvaEarnings { get; set; } // PIC S9(7)V99.
}
