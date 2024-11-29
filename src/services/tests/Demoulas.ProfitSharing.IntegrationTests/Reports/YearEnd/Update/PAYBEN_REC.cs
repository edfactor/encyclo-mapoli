namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PAYBEN_REC
{
    public long PYBEN_PSN { get; set; } // PIC 9(11).
    public int PYBEN_PAYSSN { get; set; } // PIC 9(9).
    public string? PYBEN_NAME { get; set; } // PIC X(25).

    public decimal PYBEN_PSAMT { get; set; } // PIC S9(7)V99.
    public decimal PYBEN_PROF_EARN { get; set; } // PIC S9(7)V99.
    public decimal PYBEN_PROF_EARN2 { get; set; } // PIC S9(7)V99.
}
