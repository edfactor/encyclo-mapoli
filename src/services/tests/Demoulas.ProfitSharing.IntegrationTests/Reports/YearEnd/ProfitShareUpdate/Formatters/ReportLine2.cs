
using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters.FormatUtils;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class ReportLine2
{
    public long PR2_PSN { get; set; } // PIC 9(11)
    public string? FILLER0 { get; set; } // PIC X
    public string? PR2_EMP_NAME { get; set; } // PIC X(22)
    public string? FILLER1 { get; set; } // PIC X
    public decimal PR2_BEG_BAL { get; set; } // PIC ZZZZ,ZZZ.99-
    public decimal PR2_CONT { get; set; } // PIC ZZZZ,ZZZ.99-
    public decimal PR2_EARN { get; set; } // PIC ZZZZ,ZZZ.99-
    public decimal PR2_EARN2 { get; set; } // PIC ZZZ,ZZZ.99-
    public decimal PR2_FORF { get; set; } // PIC ZZZZ,ZZZ.99-
    public decimal PR2_DIST1 { get; set; } // PIC ZZZZ,ZZZ.99-
    public decimal PR2_MIL { get; set; } // PIC ZZZ,ZZZ.99-
    public decimal PR2_END_BAL { get; set; } // PIC ZZZZ,ZZZ.99-
    public string? PR2_NEWEMP { get; set; } // PIC X(3)

    public override string ToString()
    {
        return
            rformat(PR2_PSN, "long", "9(11)")
            + rformat(FILLER0, "string?", "X")
            + rformat(PR2_EMP_NAME, "string?", "X(22)")
            + rformat(FILLER1, "string?", "X")
            + rformat(PR2_BEG_BAL, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR2_CONT, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR2_EARN, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR2_EARN2, "decimal", "ZZZ,ZZZ.99-")
            + rformat(PR2_FORF, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR2_DIST1, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR2_MIL, "decimal", "ZZZ,ZZZ.99-")
            + rformat(PR2_END_BAL, "decimal", "ZZZZ,ZZZ.99-")
            + rformat(PR2_NEWEMP, "string?", "X(3)")
            ;
    }
}
/*

 01  REPORT-LINE-2.
** 2021  restore to 9 for 2021 per Don
**     05  PR2-PSN                  PIC 9(09)         VALUE ZERO.
     05  PR2-PSN                  PIC 9(11)         VALUE ZERO.
     05  FILLER                   PIC X             VALUE SPACE.
     05  PR2-EMP-NAME             PIC X(22)         VALUE SPACE.
     05  FILLER                   PIC X             VALUE SPACE.
     05  PR2-BEG-BAL              PIC ZZZZ,ZZZ.99-  VALUE ZERO.
     05  PR2-CONT                 PIC ZZZZ,ZZZ.99-  VALUE ZERO.
     05  PR2-EARN                 PIC ZZZZ,ZZZ.99-  VALUE ZERO.
     05  PR2-EARN2                PIC ZZZ,ZZZ.99-   VALUE ZERO.
     05  PR2-FORF                 PIC ZZZZ,ZZZ.99-  VALUE ZERO.
     05  PR2-DIST1                PIC ZZZZ,ZZZ.99-  VALUE ZERO.
     05  PR2-MIL                  PIC ZZZ,ZZZ.99-   VALUE ZERO.
     05  PR2-END-BAL              PIC ZZZZ,ZZZ.99-  VALUE ZERO.
     05  PR2-NEWEMP               PIC X(3)          VALUE SPACE.

* =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

*/
