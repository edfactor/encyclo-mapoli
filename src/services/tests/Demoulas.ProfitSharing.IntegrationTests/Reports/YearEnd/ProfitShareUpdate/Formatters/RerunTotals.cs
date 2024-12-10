
using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters.FormatUtils;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class RerunTotals
{
    public string? FILLER0 { get; set; } = " TOTAL FORFEITURES ="; // PIC X(20)
    public decimal RERUN_OVER { get; set; } // PIC ZZ,ZZZ,ZZZ.99-.
    public string? FILLER1 { get; set; } = ", TOTAL POINTS ="; // PIC X(17)
    public long RERUN_POINTS { get; set; } // PIC ZZ,ZZZ,ZZZ-.
    public string? FILLER2 { get; set; } = " FOR EMPLOYEES WITH CONTRIBUTIONS OVER "; // PIC X(40)
    public long RERUN_MAX { get; set; } // PIC Z,ZZZ,ZZ9.

    public override string ToString()
    {
        return
            rformat(FILLER0, "string?", "X(20)")
            + rformat(RERUN_OVER, "decimal", "ZZ,ZZZ,ZZZ.99-")
            + rformat(FILLER1, "string?", "X(17)")
            + (RERUN_POINTS == 0 ? "".PadLeft("ZZ,ZZZ,ZZZ-".Length) : rformat(RERUN_POINTS, "long", "ZZ,ZZZ,ZZZ-"))
            + rformat(FILLER2, "string?", "X(41)")
            + rformat(RERUN_MAX, "long", "Z,ZZZ,ZZ9")
            ;
    }
}
/*

 01  RERUN-TOT.
     05  FILLER                   PIC X(20)     VALUE
         " TOTAL FORFEITURES =".
     05  RERUN-OVER               PIC ZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X(17)     VALUE
         ", TOTAL POINTS =".
     05  RERUN-POINTS             PIC ZZ,ZZZ,ZZZ-.
     05  FILLER                   PIC X(40)     VALUE
         " FOR EMPLOYEES WITH CONTRIBUTIONS OVER ".
     05  RERUN-MAX                PIC Z,ZZZ,ZZ9.

* =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

*/
