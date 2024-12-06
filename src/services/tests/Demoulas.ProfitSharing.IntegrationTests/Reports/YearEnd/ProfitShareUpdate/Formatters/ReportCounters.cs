namespace Demoulas.ProfitSharing.Services.Reports.YearEnd.Update.Formatters;

public class ReportCounters
{
    public long PageCounter { get; set; } // PIC 9(05)
    public long LineCounter { get; set; } = 99; // PIC 99
    public long EmployeeCounter { get; set; } // PIC 9(06)
    public long BeneficiaryCounter { get; set; } // PIC 9(06)
}
