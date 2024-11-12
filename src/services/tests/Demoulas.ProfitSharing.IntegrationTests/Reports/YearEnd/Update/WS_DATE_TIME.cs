namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class WS_DATE_TIME {
    public long WS_DATEN { get; set; } // PIC 9(8)
    public long WS_CC { get; set; } // PIC 99.
    public long WS_YY { get; set; } // PIC 99.
    public long WS_MM { get; set; } // PIC 99.
    public long WS_DD { get; set; } // PIC 99.
    public long WS_CCYY { get; set; } // PIC 9(4).
    public string? FILLER0 { get; set; } // PIC X(4).
    public long WS_HR { get; set; } // PIC 99
    public long WS_MN { get; set; } // PIC 99
    public long WS_SC { get; set; } // PIC 99
    public long WS_YY1 { get; set; } // PIC 9
    public long WS_YY2 { get; set; } // PIC 9

    public DateOnly WS_DATE;
    public long WS_YEAR;  // should only be 2 digits
}

/*

 01  WS-DATE-TIME.
     03  WS-DATEN                              PIC 9(8)  VALUE 0.
     03  WS-DATENX REDEFINES WS-DATEN.
         05  WS-CC                             PIC 99.
         05  WS-DATE.
             07  WS-YY                         PIC 99.
             07  WS-MM                         PIC 99.
             07  WS-DD                         PIC 99.
     03  WS-DATEX REDEFINES WS-DATEN.
         05  WS-CCYY                            PIC 9(4).
         05  FILLER                             PIC X(4).
     03  WS-TIME.
         05  WS-HR                    PIC 99        VALUE ZERO.
         05  WS-MN                    PIC 99        VALUE ZERO.
         05  WS-SC                    PIC 99        VALUE ZERO.
     03  WS-YEAR.
         05  WS-YY1                   PIC 9         VALUE ZERO.
         05  WS-YY2                   PIC 9         VALUE ZERO.

*/
