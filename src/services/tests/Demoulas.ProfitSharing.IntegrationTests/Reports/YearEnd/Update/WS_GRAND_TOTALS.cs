namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class WS_GRAND_TOTALS
{
    public decimal WS_GRTOT_BEGBAL { get; set; } // PIC S9(10)V99
    public decimal WS_GRTOT_CONT { get; set; } // PIC S9(8)V99
    public decimal WS_GRTOT_EARN { get; set; } // PIC S9(9)V99
    public decimal WS_GRTOT_EARN2 { get; set; } // PIC S9(8)V99
    public decimal WS_GRTOT_FORF { get; set; } // PIC S9(8)V99
    public decimal WS_GRTOT_DIST1 { get; set; } // PIC S9(9)V99
    public decimal WS_GRTOT_MIL { get; set; } // PIC S9(8)V99
    public decimal WS_GRTOT_XFER { get; set; } // PIC S9(8)V99
    public decimal WS_GRTOT_PXFER { get; set; } // PIC S9(8)V99
    public decimal WS_GRTOT_ENDBAL { get; set; } // PIC S9(10)V99
    public decimal WS_GRTOT_CAF { get; set; } // PIC S9(8)V99
}

/*

 01  WS-GRAND-TOTALS.
     05  WS-GRTOT-BEGBAL            PIC S9(10)V99  VALUE ZERO.
     05  WS-GRTOT-CONT              PIC S9(8)V99  VALUE ZERO.
     05  WS-GRTOT-EARN              PIC S9(9)V99  VALUE ZERO.
     05  WS-GRTOT-EARN2             PIC S9(8)V99  VALUE ZERO.
     05  WS-GRTOT-FORF              PIC S9(8)V99  VALUE ZERO.
     05  WS-GRTOT-DIST1             PIC S9(9)V99  VALUE ZERO.
     05  WS-GRTOT-MIL               PIC S9(8)V99  VALUE ZERO.
     05  WS-GRTOT-XFER              PIC S9(8)V99  VALUE ZERO.
     05  WS-GRTOT-PXFER             PIC S9(8)V99  VALUE ZERO.
     05  WS-GRTOT-ENDBAL            PIC S9(10)V99  VALUE ZERO.
     05  WS-GRTOT-CAF               PIC S9(8)V99  VALUE ZERO.

*/
