namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class WS_COMPUTE_TOTALS {
    public decimal WS_POINTS_DOLLARS { get; set; } // PIC S9(8)V99
    public decimal WS_EARNINGS_BALANCE { get; set; } // PIC S9(8)V99
    public long WS_EARN_POINTS { get; set; } // PIC S9(5)
    public long WS_REMAINDER { get; set; } // PIC 99
    public decimal WS_FORF_AMT { get; set; } // PIC S9(8)V99
    public decimal WS_CONT_AMT { get; set; } // PIC S9(8)V99
    public decimal WS_MIL_AMT { get; set; } // PIC S9(8)V99
    public decimal WS_XFER_AMT { get; set; } // PIC S9(8)V99
    public decimal WS_PXFER_AMT { get; set; } // PIC S9(8)V99
    public decimal WS_EARN_AMT { get; set; } // PIC S9(8)V99
    public decimal WS_EARN2_AMT { get; set; } // PIC S9(8)V99
    public decimal WS_EARN_1_PLUS_2 { get; set; } // PIC S9(8)V99
    public decimal WS_CAF_AMT { get; set; } // PIC S9(8)V99
}

/*

 01  WS-COMPUTE-TOTALS.
     05  WS-POINTS-DOLLARS        PIC S9(8)V99  VALUE ZERO.
     05  WS-EARNINGS-BALANCE      PIC S9(8)V99  VALUE ZERO.
     05  WS-EARN-POINTS           PIC S9(5)     VALUE ZERO.
     05  WS-REMAINDER             PIC 99        VALUE ZERO.
     05  WS-FORF-AMT              PIC S9(8)V99  VALUE ZERO.
     05  WS-CONT-AMT              PIC S9(8)V99  VALUE ZERO.
     05  WS-MIL-AMT               PIC S9(8)V99  VALUE ZERO.
     05  WS-XFER-AMT              PIC S9(8)V99  VALUE ZERO.
     05  WS-PXFER-AMT             PIC S9(8)V99  VALUE ZERO.
     05  WS-EARN-AMT              PIC S9(8)V99  VALUE ZERO.
     05  WS-EARN2-AMT             PIC S9(8)V99  VALUE ZERO.
     05  WS-EARN-1-PLUS-2         PIC S9(8)V99  VALUE ZERO.
     05  WS-CAF-AMT               PIC S9(8)V99  VALUE ZERO.
*/
