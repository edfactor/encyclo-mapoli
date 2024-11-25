namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class WS_MAXCONT_TOTALS
{
    public decimal WS_MAX { get; set; } // PIC S9(8)V99
    public decimal WS_OVER { get; set; } // PIC S9(8)V99
    public decimal WS_SAVE_OVER { get; set; } // PIC S9(8)V99
    public decimal WS_TOT_OVER { get; set; } // PIC S9(8)V99
    public long WS_TOT_POINTS { get; set; } // PIC S9(8)
}

/*

 01  WS-MAXCONT-TOTALS.
     05  WS-MAX                   PIC S9(8)V99  VALUE ZERO.
     05  WS-OVER                  PIC S9(8)V99  VALUE ZERO.
     05  WS-SAVE-OVER             PIC S9(8)V99  VALUE ZERO.
     05  WS-TOT-OVER              PIC S9(8)V99  VALUE ZERO.
     05  WS-TOT-POINTS            PIC S9(8)     VALUE ZERO.

*/
