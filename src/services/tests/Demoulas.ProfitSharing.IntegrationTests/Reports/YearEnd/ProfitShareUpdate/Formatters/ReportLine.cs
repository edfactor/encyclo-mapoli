
using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters.FormatUtils;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class ReportLine
{
    public string? FILLER0 { get; set; } // PIC X(04)
    public long BADGE_NBR { get; set; } // PIC 9(07)
    public string? FILLER1 { get; set; } // PIC X(01)
    public string? EMP_NAME { get; set; } // PIC X(22)
    public string? FILLER2 { get; set; } // PIC X(01)
    public decimal BEG_BAL { get; set; } // PIC ZZZZ,ZZZ.99-
    public decimal PR_CONT { get; set; } // PIC ZZZZ,ZZZ.99-
    public decimal PR_EARN { get; set; } // PIC ZZZZ,ZZZ.99-
    public decimal PR_EARN2 { get; set; } // PIC ZZZ,ZZZ.99-
    public decimal PR_FORF { get; set; } // PIC ZZZZ,ZZZ.99-
    public decimal PR_DIST1 { get; set; } // PIC ZZZZ,ZZZ.99-
    public decimal PR_MIL { get; set; } // PIC ZZZ,ZZZ.99-
    public decimal END_BAL { get; set; } // PIC ZZZZ,ZZZ.99-
    public string? PR_NEWEMP { get; set; } // PIC X(3)

    public override string ToString()
    {
        return
            rformat(FILLER0, "string?", "X(04)")
            + rformat(BADGE_NBR, "long", "9(07)")
            + rformat(FILLER1, "string?", "X(01)")
            + rformat(EMP_NAME, "string?", "X(22)")
            + rformat(FILLER2, "string?", "X(01)")
            + rformat(BEG_BAL, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR_CONT, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR_EARN, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR_EARN2, "decimal", "ZZZ,ZZZ.99-")
            + rformat(PR_FORF, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR_DIST1, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR_MIL, "decimal", "ZZZ,ZZZ.99-")
            + rformat(END_BAL, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR_NEWEMP, "string?", "X(3)")
            ;
    }
}
/*

 01  REPORT-LINE.
**     05  FILLER                   PIC X(01)        VALUE " ".
     05  FILLER                   PIC X(04)        VALUE "    ".
     05  BADGE-NBR                PIC 9(07)        VALUE ZERO.
**     05  FILLER                   PIC X(04)        VALUE SPACE.
     05  FILLER                   PIC X(01)        VALUE SPACE.
     05  EMP-NAME                 PIC X(22)        VALUE SPACE.
     05  FILLER                   PIC X(01)        VALUE SPACE.
     05  BEG-BAL                  PIC ZZZZ,ZZZ.99- VALUE ZERO.
     05  PR-CONT                  PIC ZZZZ,ZZZ.99- VALUE ZERO.
     05  PR-EARN                  PIC ZZZZ,ZZZ.99- VALUE ZERO.
     05  PR-EARN2                 PIC  ZZZ,ZZZ.99- VALUE ZERO.
     05  PR-FORF                  PIC ZZZZ,ZZZ.99- VALUE ZERO.
     05  PR-DIST1                 PIC ZZZZ,ZZZ.99- VALUE ZERO.
     05  PR-MIL                   PIC  ZZZ,ZZZ.99- VALUE ZERO.
     05  END-BAL                  PIC ZZZZ,ZZZ.99- VALUE ZERO.
*     05  FILLER                   PIC X(01)        VALUE SPACE.
     05  PR-NEWEMP                PIC X(3)         VALUE SPACE.

* =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
* REPORT LINE 2 IS FOR PAYBEN RECORDS
* =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

*/
