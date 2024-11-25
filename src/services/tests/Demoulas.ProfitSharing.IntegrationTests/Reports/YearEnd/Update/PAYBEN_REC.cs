namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PAYBEN_REC
{
    public long PYBEN_PSN { get; set; } // PIC 9(11).
    public long PYBEN_PAYSSN { get; set; } // PIC 9(9).
    public string? PYBEN_TYPE { get; set; } // PIC X.
    public long PYBEN_PERCENT { get; set; } // PIC 9(3).
    public string? PYBEN_NAME { get; set; } // PIC X(25).
    public string? PYBEN_ADD { get; set; } // PIC X(20).
    public string? PYBEN_CITY { get; set; } // PIC X(13).
    public string? PYBEN_STATE { get; set; } // PIC XX.
    public long PYBEN_ZIP { get; set; } // PIC 9(5).
    public string? PYBEN_RELATION { get; set; } // PIC X(10).
    public long PYBEN_DOBIRTH { get; set; } // PIC 9(8).
    public decimal PYBEN_PSDISB { get; set; } // PIC S9(7)V99.
    public decimal PYBEN_PSAMT { get; set; } // PIC S9(7)V99.
    public decimal PYBEN_PROF_EARN { get; set; } // PIC S9(7)V99.
    public decimal PYBEN_PROF_EARN2 { get; set; } // PIC S9(7)V99.
}

/*

 01  PAYBEN-REC.
     02  PSKEY.
         03  PYBEN-PSN               PIC  9(11).
     02  PYBEN-PAYSSN                PIC  9(9).
     02  PYBEN-TYPE                  PIC  X.
     02  PYBEN-PERCENT               PIC  9(3).
     02  PYBEN-NAME                  PIC  X(25).
     02  PYBEN-ADD                   PIC  X(20).
     02  PYBEN-CITY                  PIC  X(13).
     02  PYBEN-STATE                 PIC  XX.
     02  PYBEN-ZIP                   PIC  9(5).
     02  PYBEN-RELATION              PIC  X(10).
     02  PYBEN-DOBIRTH               PIC  9(8).
     02  PYBEN-PSDISB                PIC  S9(7)V99.
     02  PYBEN-PSAMT                 PIC  S9(7)V99.
     02  PYBEN-PROF-EARN             PIC  S9(7)V99.
     02  PYBEN-PROF-EARN2            PIC  S9(7)V99.
*/
