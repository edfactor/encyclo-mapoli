
using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters.FormatUtils;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class TotalHeader2
{
    public string? FILLER0 { get; set; } // PIC X(10)
    public string? FILLER1 { get; set; } = "BEGINNING"; // PIC X(19)
    public string? FILLER2 { get; set; } // PIC X(77)
    public string? FILLER3 { get; set; } = "MILITARY/"; // PIC X(09)
    public string? FILLER4 { get; set; } // PIC X(04)
    public string? FILLER5 { get; set; } = "ENDING"; // PIC X(6)

    public override string ToString()
    {
        return
            rformat(FILLER0, "string?", "X(10)")
            + rformat(FILLER1, "string?", "X(19)")
            + rformat(FILLER2, "string?", "X(77)")
            + rformat(FILLER3, "string?", "X(09)")
            + rformat(FILLER4, "string?", "X(04)")
            + rformat(FILLER5, "string?", "X(6)")
            ;
    }
}
/*

 01  TOTAL-HEADER-2.
     05  FILLER                   PIC X(10)    VALUE SPACES.
     05  FILLER                   PIC X(19)    VALUE
         "BEGINNING".
     05  FILLER                   PIC X(77)     VALUE SPACES.
     05  FILLER                   PIC X(09)     VALUE "MILITARY/".
     05  FILLER                   PIC X(04)     VALUE SPACES.
     05  FILLER                   PIC X(6)     VALUE
         "ENDING".


*/
