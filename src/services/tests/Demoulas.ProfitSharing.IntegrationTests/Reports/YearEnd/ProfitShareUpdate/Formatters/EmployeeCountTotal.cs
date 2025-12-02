namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

using static FormatUtils;

public class EmployeeCountTotal
{
    public string? FILLER0 { get; set; } = "TOTAL # OF EMPLOYEES =     "; // PIC X(27)
    public long PR_TOT_EMPLOYEE_COUNT { get; set; } // PIC ZZZ,ZZ9.

    public override string ToString()
    {
        return
            rformat(FILLER0, "string?", "X(27)")
            + rformat(PR_TOT_EMPLOYEE_COUNT, "long", "ZZZ,ZZ9.")
            ;
    }
}
/*

 01  EMPLOYEE-COUNT-TOT.
     05  FILLER                   PIC X(27)     VALUE
         "TOTAL # OF EMPLOYEES =     ".
     05  PR-TOT-EMPLOYEE-COUNT    PIC ZZZ,ZZ9.

*/
