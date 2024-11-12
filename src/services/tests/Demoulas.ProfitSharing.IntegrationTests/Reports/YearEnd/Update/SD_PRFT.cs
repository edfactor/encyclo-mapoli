namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class SD_PRFT {
    public long SD_BADGE { get; set; } // PIC 9(7).
    public string? SD_NAME { get; set; } // PIC X(25).
    public long SD_SSN { get; set; } // PIC 9(9)
    public decimal SD_AMT { get; set; } // PIC S9(8)V99
    public decimal SD_DIST1 { get; set; } // PIC S9(8)V99
    public decimal SD_MIL { get; set; } // PIC S9(8)V99
    public decimal SD_XFER { get; set; } // PIC S9(8)V99
    public decimal SD_PXFER { get; set; } // PIC S9(8)V99
    public long SD_NEWEMP { get; set; } // PIC 99
    public long SD_POINTS { get; set; } // PIC S9(5)
    public long SD_POINTS_EARN { get; set; } // PIC S9(5)
    public decimal SD_CONT { get; set; } // PIC S9(8)V99
    public decimal SD_FORF { get; set; } // PIC S9(8)V99
    public decimal SD_EARN { get; set; } // PIC S9(8)V99
    public decimal SD_EARN2 { get; set; } // PIC S9(8)V99
    public decimal SD_MAXOVER { get; set; } // PIC S9(8)V99
    public long SD_MAXPOINTS { get; set; } // PIC S9(5)
    public long SD_PSN { get; set; } // PIC 9(11)
    public decimal SD_CAF { get; set; } // PIC S9(8)V99

public SD_PRFT(){
  
}


public SD_PRFT(PRFT prft)
    {
        SD_BADGE = prft.FD_BADGE;
        SD_NAME = prft.FD_NAME;
        SD_SSN = prft.FD_SSN;
        SD_AMT = prft.FD_AMT;
        SD_DIST1 = prft.FD_DIST1;
        SD_MIL = prft.FD_MIL;
        SD_XFER = prft.FD_XFER;
        SD_PXFER = prft.FD_PXFER;
        SD_NEWEMP = prft.FD_NEWEMP;
        SD_POINTS = prft.FD_POINTS;
        SD_POINTS_EARN = prft.FD_POINTS_EARN;
        SD_CONT = prft.FD_CONT;
        SD_FORF = prft.FD_FORF;
        SD_EARN = prft.FD_EARN;
        SD_EARN2 = prft.FD_EARN2;
        SD_MAXOVER = prft.FD_MAXOVER;
        SD_MAXPOINTS = prft.FD_MAXPOINTS;
        SD_PSN = prft.FD_PSN;
        SD_CAF = prft.FD_CAF;
    }
}
/*

 01  SD-PRFT.
   03  SD-BADGE                   PIC 9(7).
   03  SD-NAME                    PIC X(25).
   03  SD-SSN                     PIC 9(9) COMP-6.
   03  SD-AMT                     PIC S9(8)V99 COMP-6.
   03  SD-DIST1                   PIC S9(8)V99 COMP-6.
   03  SD-MIL                     PIC S9(8)V99 COMP-6.
   03  SD-XFER                    PIC S9(8)V99 COMP-6.
   03  SD-PXFER                   PIC S9(8)V99 COMP-6.
   03  SD-NEWEMP                  PIC 99 COMP-6.
   03  SD-POINTS                  PIC S9(5) COMP-6.
   03  SD-POINTS-EARN             PIC S9(5) COMP-6.
   03  SD-CONT                    PIC S9(8)V99 COMP-6.
   03  SD-FORF                    PIC S9(8)V99 COMP-6.
   03  SD-EARN                    PIC S9(8)V99 COMP-6.
   03  SD-EARN2                   PIC S9(8)V99 COMP-6.
   03  SD-MAXOVER                 PIC S9(8)V99 COMP-6.
   03  SD-MAXPOINTS               PIC S9(5) COMP-6.
   03  SD-PSN                     PIC 9(11) COMP-6.
   03  SD-CAF                     PIC S9(8)V99 COMP-6.

*/
