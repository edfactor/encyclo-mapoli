namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class INTERMEDIATE_VALUES
{
    public long WS_FD_BADGE { get; set; } // PIC 9(7).
    public string? WS_FD_NAME { get; set; } // PIC X(25).
    public long WS_FD_SSN { get; set; } // PIC 9(9)
    public long WS_FD_PSN { get; set; } // PIC 9(11)
    public decimal WS_FD_AMT { get; set; } // PIC S9(8)V99
    public decimal WS_FD_DIST1 { get; set; } // PIC S9(8)V99
    public decimal WS_FD_MIL { get; set; } // PIC S9(8)V99
    public decimal WS_FD_XFER { get; set; } // PIC S9(8)V99
    public decimal WS_FD_PXFER { get; set; } // PIC S9(8)V99
    public long WS_FD_NEWEMP { get; set; } // PIC 99
    public long WS_FD_POINTS { get; set; } // PIC S9(5)
    public long WS_FD_POINTS_EARN { get; set; } // PIC S9(5)
    public decimal WS_FD_CONT { get; set; } // PIC S9(8)V99
    public decimal WS_FD_FORF { get; set; } // PIC S9(8)V99
    public decimal WS_FD_EARN { get; set; } // PIC S9(8)V99
    public decimal WS_FD_EARN2 { get; set; } // PIC S9(8)V99
    public decimal WS_FD_ETVA { get; set; } // PIC S9(8)V99
    public decimal WS_FD_ETVA2 { get; set; } // PIC S9(8)V99
    public decimal WS_FD_CAF { get; set; } // PIC S9(8)V99
}

/*

 01 INTERMEDIATE-VALUES.
    02 WS-FD-BADGE          PIC 9(7).
    02 WS-FD-NAME           PIC X(25).
    02 WS-FD-SSN            PIC 9(9)     COMP-6.
    02 ws-FD-PSN            PIC 9(11) COMP-6.
    02 WS-FD-AMT            PIC S9(8)V99 COMP-6.
    02 WS-FD-DIST1          PIC S9(8)V99 COMP-6.
    02 WS-FD-MIL            PIC S9(8)V99 COMP-6.
    02 WS-FD-XFER           PIC S9(8)V99 COMP-6.
    02 WS-FD-PXFER          PIC S9(8)V99 COMP-6.
    02 WS-FD-NEWEMP         PIC 99       COMP-6.
    02 WS-FD-POINTS         PIC S9(5)    COMP-6.
    02 WS-FD-POINTS-EARN    PIC S9(5)    COMP-6.
    02 WS-FD-CONT           PIC S9(8)V99 COMP-6.
    02 WS-FD-FORF           PIC S9(8)V99 COMP-6.
    02 WS-FD-EARN           PIC S9(8)V99 COMP-6.
    02 WS-FD-EARN2          PIC S9(8)V99 COMP-6.
    02 WS-FD-ETVA           PIC S9(8)V99 COMP-6.
    02 WS-FD-ETVA2          PIC S9(8)V99 COMP-6.
    02 WS-FD-CAF            PIC S9(8)V99 COMP-6.
*/
