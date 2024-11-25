namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class WS_COUNTERS
{
    public long WS_PAGE_CTR { get; set; } // PIC 9(05)
    public long WS_LINE_CTR { get; set; } = 99; // PIC 99
    public long WS_FILE_CTR { get; set; } // PIC 9
    public long WS_EMPLOYEE_CTR { get; set; } // PIC 9(06)
    public long WS_EMPLOYEE_CTR_PAYBEN { get; set; } // PIC 9(06)
}

/*

 01  WS-COUNTERS.
* 02/15/02 PROJ. 90600 CHANGED FOLLOWING FIELD NAME FROM "WS-CTR".
     05  WS-PAGE-CTR              PIC 9(05)     VALUE ZERO.
     05  WS-LINE-CTR              PIC 99        VALUE 99.
     05  WS-FILE-CTR              PIC 9         VALUE ZERO.
* 02/15/02 PROJ. 90600 ADDED FOLLOWING FIELD.
     05  WS-EMPLOYEE-CTR          PIC 9(06)     VALUE ZERO.
     05  WS-EMPLOYEE-CTR-payben   PIC 9(06)     VALUE ZERO.
*/
