
using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters.FormatUtils;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class BeneficiaryCountTotal
{
    public string? FILLER0 { get; set; } = "TOTAL # OF BENEFICIARIES = "; // PIC X(27)
    public long PB_TOT_EMPLOYEE_COUNT { get; set; } // PIC ZZZ,ZZ9.

    public override string ToString()
    {
        return
            rformat(FILLER0, "string?", "X(27)")
            + rformat(PB_TOT_EMPLOYEE_COUNT, "long", "ZZZ,ZZ9.")
            ;
    }
}
/*

 01  EMPLOYEE-COUNT-TOT-PAYBEN.
     05  FILLER                   PIC X(27)     VALUE
         "TOTAL # OF BENEFICIARIES = ".
     05  PB-TOT-EMPLOYEE-COUNT    PIC ZZZ,ZZ9.

*/
