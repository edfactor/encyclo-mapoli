namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class WORK_DATEX {
    public long WCCYY { get; set; } // PIC 9(4).
    public long WMMDD { get; set; } // PIC 9(4).
}

/*

 01  WORK-CYMDX REDEFINES WORK-DATEX.
     03  WCCYY                      PIC 9(4).
     03  WMMDD                      PIC 9(4).
*/
