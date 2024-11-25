namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class WS_CLIENT_TOTALS
{
    public decimal WS_TOT_BEGBAL { get; set; } // PIC S9(10)V99
    public decimal WS_TOT_CONT { get; set; } // PIC S9(8)V99
    public decimal WS_TOT_EARN { get; set; } // PIC S9(9)V99
    public decimal WS_TOT_EARN2 { get; set; } // PIC S9(8)V99
    public decimal WS_TOT_FORF { get; set; } // PIC S9(8)V99
    public decimal WS_TOT_DIST1 { get; set; } // PIC S9(9)V99
    public decimal WS_TOT_MIL { get; set; } // PIC S9(8)V99
    public decimal WS_TOT_XFER { get; set; } // PIC S9(8)V99
    public decimal WS_TOT_PXFER { get; set; } // PIC S9(8)V99
    public decimal WS_TOT_ENDBAL { get; set; } // PIC S9(10)V99
    public decimal WS_TOT_CAF { get; set; } // PIC S9(8)V99
    public long WS_PROF_PTS_TOTAL { get; set; } // PIC S9(8)
    public long WS_EARN_PTS_TOTAL { get; set; } // PIC S9(8)
}

/*

 01  WS-CLIENT-TOTALS.
     05  WS-TOT-BEGBAL            PIC S9(10)V99  VALUE ZERO.
     05  WS-TOT-CONT              PIC S9(8)V99  VALUE ZERO.
     05  WS-TOT-EARN              PIC S9(9)V99  VALUE ZERO.
     05  WS-TOT-EARN2             PIC S9(8)V99  VALUE ZERO.
     05  WS-TOT-FORF              PIC S9(8)V99  VALUE ZERO.
     05  WS-TOT-DIST1             PIC S9(9)V99  VALUE ZERO.
     05  WS-TOT-MIL               PIC S9(8)V99  VALUE ZERO.
     05  WS-TOT-XFER              PIC S9(8)V99  VALUE ZERO.
     05  WS-TOT-PXFER             PIC S9(8)V99  VALUE ZERO.
     05  WS-TOT-ENDBAL            PIC S9(10)V99  VALUE ZERO.
     05  WS-TOT-CAF               PIC S9(8)V99  VALUE ZERO.
     05  WS-PROF-PTS-TOTAL        PIC S9(8)     VALUE ZERO.
     05  WS-EARN-PTS-TOTAL        PIC S9(8)     VALUE ZERO.

*/
