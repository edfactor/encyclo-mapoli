
using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters.FormatUtils;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class TotalHeader1
{
    public string? FILLER0 { get; set; } = "PAY444"; // PIC X(14)
    public string? FILLER1 { get; set; } // PIC X(11)
    public string? FILLER2 { get; set; } = "         P R O F I T     S H A R I N G    F O R    "; // PIC X(52)
    public long TOT_HDR1_YEAR1 { get; set; } // PIC 9(4)
    public string? FILLER3 { get; set; } // PIC X(21)
    public long TOT_HDR1_MM { get; set; } // PIC 99
    public string? FILLER4 { get; set; } = "/"; // PIC X
    public long TOT_HDR1_DD { get; set; } // PIC 99
    public string? FILLER5 { get; set; } = "/"; // PIC X
    public long TOT_HDR1_YY { get; set; } // PIC 99
    public string? FILLER6 { get; set; } // PIC X(4)
    public long TOT_HDR1_HR { get; set; } // PIC 99
    public string? FILLER7 { get; set; } = ":"; // PIC X
    public long TOT_HDR1_MN { get; set; } // PIC 99
    public string? FILLER8 { get; set; } // PIC X(15)

    public override string ToString()
    {
        return
            rformat(FILLER0, "string?", "X(14)")
            + rformat(FILLER1, "string?", "X(11)")
            + rformat(FILLER2, "string?", "X(52)")
            + rformat(TOT_HDR1_YEAR1, "long", "9(4)")
            + rformat(FILLER3, "string?", "X(21)")
            + rformat(TOT_HDR1_MM, "long", "99")
            + rformat(FILLER4, "string?", "X")
            + rformat(TOT_HDR1_DD, "long", "99")
            + rformat(FILLER5, "string?", "X")
            + rformat(TOT_HDR1_YY, "long", "99")
            + rformat(FILLER6, "string?", "X(4)")
            + rformat(TOT_HDR1_HR, "long", "99")
            + rformat(FILLER7, "string?", "X")
            + rformat(TOT_HDR1_MN, "long", "99")
            + rformat(FILLER8, "string?", "X(15)")
            ;
    }
}
/*

 01  TOTAL-HEADER-1.
     05  FILLER                   PIC X(14)     VALUE "PAY444".
     05  FILLER                   PIC X(11)     VALUE SPACES.
     05  FILLER                   PIC X(52)     VALUE
         "         P R O F I T     S H A R I N G    F O R    ".
     05  TOT-HDR1-YEAR1           PIC 9(4)         VALUE ZERO.
     05  FILLER                   PIC X(21)     VALUE SPACE.
     05  TOT-HDR1-MM              PIC 99        VALUE ZERO.
     05  FILLER                   PIC X         VALUE "/".
     05  TOT-HDR1-DD              PIC 99        VALUE ZERO.
     05  FILLER                   PIC X         VALUE "/".
     05  TOT-HDR1-YY              PIC 99        VALUE ZERO.
     05  FILLER                   PIC X(4)      VALUE SPACE.
     05  TOT-HDR1-HR              PIC 99        VALUE ZERO.
     05  FILLER                   PIC X         VALUE ":".
     05  TOT-HDR1-MN              PIC 99        VALUE ZERO.
     05  FILLER                   PIC X(15)     VALUE SPACE.

*/
