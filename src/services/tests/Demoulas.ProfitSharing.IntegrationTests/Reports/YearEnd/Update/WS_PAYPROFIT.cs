namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class WS_PAYPROFIT
{
    public decimal WS_PS_AMT { get; set; } // PIC S9(8)V99
    public decimal WS_PYBEN_ETVA { get; set; } // PIC S9(8)V99
    public long WS_PROF_NEWEMP { get; set; } // PIC 99
    public long WS_PROF_POINTS { get; set; } // PIC S9(5)
    public decimal WS_PROF_CONT { get; set; } // PIC S9(8)V99
    public decimal WS_PROF_MIL { get; set; } // PIC S9(8)V99
    public decimal WS_PROF_XFER { get; set; } // PIC S9(8)V99
    public decimal WS_PROF_PXFER { get; set; } // PIC S9(8)V99
    public decimal WS_PROF_FORF { get; set; } // PIC S9(8)V99
    public decimal WS_PROF_EARN { get; set; } // PIC S9(8)V99
    public decimal WS_PROF_EARN2 { get; set; } // PIC S9(8)V99
    public decimal WS_PROF_CAF { get; set; } // PIC S9(8)V99
    public decimal WS_PS_DIST1_AMT { get; set; } // PIC S9(8)V99
    public decimal WS_PS_MIL_AMT { get; set; } // PIC S9(8)V99
}

/*

 01  WS-PAYPROFIT.
     05  WS-PS-AMT                PIC S9(8)V99  VALUE ZERO.
     05  WS-PYBEN-ETVA            PIC S9(8)V99  VALUE ZERO.
     05  WS-PROF-NEWEMP           PIC 99        VALUE ZERO.
     05  WS-PROF-POINTS           PIC S9(5)     VALUE ZERO.
     05  WS-PROF-CONT             PIC S9(8)V99  VALUE ZERO.
     05  WS-PROF-MIL              PIC S9(8)V99  VALUE ZERO.
     05  WS-PROF-XFER             PIC S9(8)V99  VALUE ZERO.
     05  WS-PROF-PXFER            PIC S9(8)V99  VALUE ZERO.
     05  WS-PROF-FORF             PIC S9(8)V99  VALUE ZERO.
     05  WS-PROF-EARN             PIC S9(8)V99  VALUE ZERO.
     05  WS-PROF-EARN2            PIC S9(8)V99  VALUE ZERO.
     05  WS-PROF-CAF              PIC S9(8)V99  VALUE ZERO.
     05  WS-PS-DIST1-AMT          PIC S9(8)V99  VALUE ZERO.
     05  WS-PS-MIL-AMT            PIC S9(8)V99  VALUE ZERO.
*/
