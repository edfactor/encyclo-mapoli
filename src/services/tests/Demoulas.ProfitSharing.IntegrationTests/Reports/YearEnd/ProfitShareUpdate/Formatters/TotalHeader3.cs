
using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters.FormatUtils;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class TotalHeader3
{
    public string? FILLER0 { get; set; } // PIC X(11)
    public string? FILLER1 { get; set; } = "BALANCE"; // PIC X(07)
    public string? FILLER2 { get; set; } // PIC X(09)
    public string? FILLER3 { get; set; } = "CONTRIB"; // PIC X(07)
    public string? FILLER4 { get; set; } // PIC X(06)
    public string? FILLER5 { get; set; } = "EARNINGS"; // PIC X(08)
    public string? FILLER6 { get; set; } // PIC X(08)
    public string? FILLER7 { get; set; } = "EARNINGS2"; // PIC X(09)
    public string? FILLER8 { get; set; } // PIC X(06)
    public string? FILLER9 { get; set; } = "FORFEITS"; // PIC X(08)
    public string? FILLER10 { get; set; } // PIC X(12)
    public string? FILLER11 { get; set; } = "DISTRIB"; // PIC X(07)
    public string? FILLER12 { get; set; } // PIC X(08)
    public string? FILLER13 { get; set; } = "PAID ALLOC"; // PIC X(10)
    public string? FILLER14 { get; set; } // PIC X(03)
    public string? FILLER15 { get; set; } = "BALANCE"; // PIC X(08)

    /*

 01  TOTAL-HEADER-3.
     05  FILLER                   PIC X(11)     VALUE SPACES.
     05  FILLER                   PIC X(07)     VALUE
         "BALANCE".
     05  FILLER                   PIC X(09)     VALUE SPACES.
     05  FILLER                   PIC X(07)     VALUE
         "CONTRIB".
     05  FILLER                   PIC X(06)     VALUE SPACES.
     05  FILLER                   PIC X(08)     VALUE
         "EARNINGS".
     05  FILLER                   PIC X(08)     VALUE SPACES.
     05  FILLER                   PIC X(09)     VALUE
         "EARNINGS2".
     05  FILLER                   PIC X(06)     VALUE SPACES.
     05  FILLER                   PIC X(08)     VALUE
         "FORFEITS".
     05  FILLER                   PIC X(12)     VALUE SPACES.
     05  FILLER                   PIC X(07)     VALUE
         "DISTRIB".
     05  FILLER                   PIC X(08)     VALUE SPACES.
     05  FILLER                   PIC X(10)     VALUE
         "PAID ALLOC".
     05  FILLER                   PIC X(03)     VALUE SPACES.
     05  FILLER                   PIC X(08)     VALUE
         "BALANCE".

*/


    public override string ToString()
    {
        return
            rformat(FILLER0, "string?", "X(11)")
            + rformat(FILLER1, "string?", "X(07)")
            + rformat(FILLER2, "string?", "X(09)")
            + rformat(FILLER3, "string?", "X(07)")
            + rformat(FILLER4, "string?", "X(06)")
            + rformat(FILLER5, "string?", "X(08)")
            + rformat(FILLER6, "string?", "X(08)")
            + rformat(FILLER7, "string?", "X(09)")
            + rformat(FILLER8, "string?", "X(06)")
            + rformat(FILLER9, "string?", "X(08)")
            + rformat(FILLER10, "string?", "X(12)")
            + rformat(FILLER11, "string?", "X(07)")
            + rformat(FILLER12, "string?", "X(08)")
            + rformat(FILLER13, "string?", "X(10)")
            + rformat(FILLER14, "string?", "X(03)")
            + rformat(FILLER15, "string?", "X(07)")
            ;
    }
}
/*

 01  TOTAL-HEADER-3.
     05  FILLER                   PIC X(11)     VALUE SPACES.
     05  FILLER                   PIC X(07)     VALUE
         "BALANCE".
     05  FILLER                   PIC X(09)     VALUE SPACES.
     05  FILLER                   PIC X(07)     VALUE
         "CONTRIB".
     05  FILLER                   PIC X(06)     VALUE SPACES.
     05  FILLER                   PIC X(08)     VALUE
         "EARNINGS".
     05  FILLER                   PIC X(08)     VALUE SPACES.
     05  FILLER                   PIC X(09)     VALUE
         "EARNINGS2".
     05  FILLER                   PIC X(06)     VALUE SPACES.
     05  FILLER                   PIC X(08)     VALUE
         "FORFEITS".
     05  FILLER                   PIC X(12)     VALUE SPACES.
     05  FILLER                   PIC X(07)     VALUE
         "DISTRIB".
     05  FILLER                   PIC X(08)     VALUE SPACES.
     05  FILLER                   PIC X(10)     VALUE
         "PAID ALLOC".
     05  FILLER                   PIC X(03)     VALUE SPACES.
     05  FILLER                   PIC X(08)     VALUE
         "BALANCE ".

*/
