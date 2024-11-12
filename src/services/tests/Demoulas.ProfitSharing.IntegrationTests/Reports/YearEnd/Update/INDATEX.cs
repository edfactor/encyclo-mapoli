namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class INDATEX {
    public long INDATE_CCYY { get; set; } // PIC 9(4).
    public long INDATE_MMDD { get; set; } // PIC 9(4).
}

/*

 01  INDATE-CYMDX REDEFINES INDATEX.
     03  INDATE-CCYY          PIC 9(4).
     03  INDATE-MMDD          PIC 9(4).
*/
