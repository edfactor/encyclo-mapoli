namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public record PAYBEN1_REC
{
    public long PYBEN_PSN1 { get; set; } // PIC 9(11).
    public int PYBEN_PAYSSN1 { get; set; } // PIC 9(9).
    public string? PYBEN_NAME1 { get; set; } // PIC X(25).
    public decimal PYBEN_PSDISB1 { get; set; } // PIC S9(7)V99.
    public decimal PYBEN_PSAMT1 { get; set; } // PIC S9(7)V99.
    public decimal PYBEN_PROF_EARN1 { get; set; } // PIC S9(7)V99.
    public decimal PYBEN_PROF_EARN21 { get; set; } // PIC S9(7)V99.
}
