
using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters.FormatUtils;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class Header2
{
    public string? FILLER0 { get; set; } = "--------- E M P L O Y E E --------- "; // PIC X(36)
    public string? FILLER1 { get; set; } = "BEGINNING"; // PIC X(09)
    public string? FILLER2 { get; set; } // PIC X(61)
    public string? FILLER3 { get; set; } = "MILITARY/"; // PIC X(09)
    public string? FILLER4 { get; set; } // PIC X(04)
    public string? FILLER5 { get; set; } = "ENDING"; // PIC X(06)

    public override string ToString()
    {
        return
            rformat(FILLER0, "string?", "X(36)")
            + rformat(FILLER1, "string?", "X(09)")
            + rformat(FILLER2, "string?", "X(61)")
            + rformat(FILLER3, "string?", "X(09)")
            + rformat(FILLER4, "string?", "X(04)")
            + rformat(FILLER5, "string?", "X(06)")
            ;
    }
}

/*

 01  HEADER-2.
     05  FILLER                   PIC X(36)     VALUE
         "--------- E M P L O Y E E --------- ".
     05  FILLER                   PIC X(09)     VALUE "BEGINNING".
     05  FILLER                   PIC X(61)     VALUE SPACES.
     05  FILLER                   PIC X(09)     VALUE "MILITARY/".
     05  FILLER                   PIC X(04)     VALUE SPACES.
     05  FILLER                   PIC X(06)     VALUE "ENDING".

*/
