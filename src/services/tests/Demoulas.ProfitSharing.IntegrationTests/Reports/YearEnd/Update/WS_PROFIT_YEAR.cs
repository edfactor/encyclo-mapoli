namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class WS_PROFIT_YEAR {
    public long WS_PROFIT_YEAR_FIRST_4 { get; set; } // PIC 9999.
    public long WS_PROFIT_YEAR_EXTENSION { get; set; } // PIC 9.
}

/*

 01  WS-PROFIT-YEAR-SPLIT REDEFINES WS-PROFIT-YEAR.
     05 WS-PROFIT-YEAR-FIRST-4        PIC 9999.
     05 WS-PROFIT-YEAR-EXTENSION      PIC 9.

*/
