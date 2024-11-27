namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PAYBEN1_REC
{
    public long PYBEN_PSN1 { get; set; } // PIC 9(11).
    public int PYBEN_PAYSSN1 { get; set; } // PIC 9(9).
    public string? PYBEN_TYPE1 { get; set; } // PIC X.
    public long PYBEN_PERCENT1 { get; set; } // PIC 9(3).
    public string? PYBEN_NAME1 { get; set; } // PIC X(25).
    public string? PYBEN_ADD1 { get; set; } // PIC X(20).
    public string? PYBEN_CITY1 { get; set; } // PIC X(13).
    public string? PYBEN_STATE1 { get; set; } // PIC XX.
    public long PYBEN_ZIP1 { get; set; } // PIC 9(5).
    public string? PYBEN_RELATION1 { get; set; } // PIC X(10).
    public long PYBEN_DOBIRTH1 { get; set; } // PIC 9(8).
    public decimal PYBEN_PSDISB1 { get; set; } // PIC S9(7)V99.
    public decimal PYBEN_PSAMT1 { get; set; } // PIC S9(7)V99.
    public decimal PYBEN_PROF_EARN1 { get; set; } // PIC S9(7)V99.
    public decimal PYBEN_PROF_EARN21 { get; set; } // PIC S9(7)V99.
}

/*

 01  PAYBEN1-REC.
     02  PSKEY1.
         03  PYBEN-PSN1               PIC  9(11).
     02  PYBEN-PAYSSN1                PIC  9(9).
     02  PYBEN-TYPE1                  PIC  X.
     02  PYBEN-PERCENT1               PIC  9(3).
     02  PYBEN-NAME1                  PIC  X(25).
     02  PYBEN-ADD1                   PIC  X(20).
     02  PYBEN-CITY1                  PIC  X(13).
     02  PYBEN-STATE1                 PIC  XX.
     02  PYBEN-ZIP1                   PIC  9(5).
     02  PYBEN-RELATION1              PIC  X(10).
     02  PYBEN-DOBIRTH1               PIC  9(8).
     02  PYBEN-PSDISB1                PIC  S9(7)V99.
     02  PYBEN-PSAMT1                 PIC  S9(7)V99.
     02  PYBEN-PROF-EARN1             PIC  S9(7)V99.
     02  PYBEN-PROF-EARN21            PIC  S9(7)V99.

*/
