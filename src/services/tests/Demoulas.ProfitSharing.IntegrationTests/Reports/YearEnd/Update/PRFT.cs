namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PRFT {
    public long FD_BADGE { get; set; } // PIC 9(7).
    public string? FD_NAME { get; set; } // PIC X(25).
    public long FD_SSN { get; set; } // PIC 9(9)
    public decimal FD_AMT { get; set; } // PIC S9(8)V99
    public decimal FD_DIST1 { get; set; } // PIC S9(8)V99
    public decimal FD_MIL { get; set; } // PIC S9(8)V99
    public decimal FD_XFER { get; set; } // PIC S9(8)V99
    public decimal FD_PXFER { get; set; } // PIC S9(8)V99
    public long FD_NEWEMP { get; set; } // PIC 99
    public long FD_POINTS { get; set; } // PIC S9(5)
    public long FD_POINTS_EARN { get; set; } // PIC S9(5)
    public decimal FD_CONT { get; set; } // PIC S9(8)V99
    public decimal FD_FORF { get; set; } // PIC S9(8)V99
    public decimal FD_EARN { get; set; } // PIC S9(8)V99
    public decimal FD_EARN2 { get; set; } // PIC S9(8)V99
    public decimal FD_MAXOVER { get; set; } // PIC S9(8)V99
    public long FD_MAXPOINTS { get; set; } // PIC S9(5)
    public long FD_PSN { get; set; } // PIC 9(11)
    public decimal FD_CAF { get; set; } // PIC S9(8)V99
}

/*

 01  PRFT.
   03  FD-BADGE                   PIC 9(7).
   03  FD-NAME                    PIC X(25).
   03  FD-SSN                     PIC 9(9) COMP-6.
   03  FD-AMT                     PIC S9(8)V99 COMP-6.
   03  FD-DIST1                   PIC S9(8)V99 COMP-6.
   03  FD-MIL                     PIC S9(8)V99 COMP-6.
   03  FD-XFER                    PIC S9(8)V99 COMP-6.
   03  FD-PXFER                   PIC S9(8)V99 COMP-6.
   03  FD-NEWEMP                  PIC 99 COMP-6.
   03  FD-POINTS                  PIC S9(5) COMP-6.
   03  FD-POINTS-EARN             PIC S9(5) COMP-6.
   03  FD-CONT                    PIC S9(8)V99 COMP-6.
   03  FD-FORF                    PIC S9(8)V99 COMP-6.
   03  FD-EARN                    PIC S9(8)V99 COMP-6.
   03  FD-EARN2                   PIC S9(8)V99 COMP-6.
   03  FD-MAXOVER                 PIC S9(8)V99 COMP-6.
   03  FD-MAXPOINTS               PIC S9(5) COMP-6.
   03  FD-PSN                     PIC 9(11) COMP-6.
   03  FD-CAF                     PIC S9(8)V99 COMP-6.

*/
