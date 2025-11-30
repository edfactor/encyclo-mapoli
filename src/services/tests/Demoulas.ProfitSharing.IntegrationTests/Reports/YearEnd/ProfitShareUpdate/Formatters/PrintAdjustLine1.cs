namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class PrintAdjustLine1
{
    public string? FILLER0 { get; set; } // PIC X(03)
    public string? PL_ADJ_DESC { get; set; } // PIC X(17)
    public string? FILLER1 { get; set; } // PIC X(03)
    public long PL_ADJUST_BADGE { get; set; } // PIC Z(07)
    public string? FILLER2 { get; set; } // PIC X(08)
    public decimal PL_CONT_AMT { get; set; } // PIC ZZZZ,ZZZ.99-
    public string? FILLER3 { get; set; } // PIC X(05)
    public decimal PL_EARN_AMT { get; set; } // PIC ZZZZ,ZZZ.99-
    public string? FILLER4 { get; set; } // PIC X(05)
    public decimal PL_EARN2_AMT { get; set; } // PIC ZZZZ,ZZZ.99-
    public string? FILLER5 { get; set; } // PIC X(05)
    public decimal PL_FORF_AMT { get; set; } // PIC ZZZZ,ZZZ.99-
}

/*

 01  PRINT-ADJ-LINE1.
     05  FILLER                   PIC X(03)        VALUE SPACES.
     05  PL-ADJ-DESC              PIC X(17)        VALUE SPACES.
     05  FILLER                   PIC X(03)        VALUE SPACES.
     05  PL-ADJUST-BADGE          PIC Z(07)        VALUE ZERO.
     05  FILLER                   PIC X(08)        VALUE SPACES.
     05  PL-CONT-AMT              PIC ZZZZ,ZZZ.99- VALUE ZERO.
     05  FILLER                   PIC X(05)        VALUE SPACES.
     05  PL-EARN-AMT              PIC ZZZZ,ZZZ.99- VALUE ZERO.
     05  FILLER                   PIC X(05)        VALUE SPACES.
     05  PL-EARN2-AMT             PIC ZZZZ,ZZZ.99- VALUE ZERO.
     05  FILLER                   PIC X(05)        VALUE SPACES.
     05  PL-FORF-AMT              PIC ZZZZ,ZZZ.99- VALUE SPACES.

*/
