namespace Demoulas.ProfitSharing.Services.Reports.YearEnd.Update.ReportFormatters;

public class GRAND_TOT
{
    public string? FILLER0 { get; set; } // PIC X(5)
    public decimal BEG_BAL_GRTOT { get; set; } // PIC Z,ZZZ,ZZZ,ZZZ.99-.
    public string? FILLER1 { get; set; } // PIC X
    public decimal CONT_GRTOT { get; set; } // PIC ZZ,ZZZ,ZZZ.99-.
    public string? FILLER2 { get; set; } // PIC X
    public decimal EARN_GRTOT { get; set; } // PIC ZZZ,ZZZ,ZZZ.99-.
    public string? FILLER3 { get; set; } // PIC X
    public decimal EARN2_GRTOT { get; set; } // PIC ZZ,ZZZ,ZZZ.99-.
    public string? FILLER4 { get; set; } // PIC X
    public decimal FORF_GRTOT { get; set; } // PIC ZZ,ZZZ,ZZZ.99-.
    public string? FILLER5 { get; set; } // PIC X
    public string? FILLER6 { get; set; } // PIC X(4)
    public decimal DIST1_GRTOT { get; set; } // PIC ZZZ,ZZZ,ZZZ.99-.
    public string? FILLER7 { get; set; } // PIC X
    public decimal MIL_GRTOT { get; set; } // PIC ZZ,ZZZ,ZZZ.99-.
    public string? FILLER8 { get; set; } // PIC X
    public decimal END_BAL_GRTOT { get; set; } // PIC Z,ZZZ,ZZZ,ZZZ.99-.
    public string? FILLER9 { get; set; } // PIC X(13)
}

/*

 01  GRAND-TOT.
     05  FILLER                   PIC X(5)     VALUE
         "G TOT".
     05  BEG-BAL-GRTOT              PIC Z,ZZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X        VALUE SPACE.
     05  CONT-GRTOT                 PIC ZZ,ZZZ,ZZZ.99-.
     05  CONT-PTS-GRTOT REDEFINES CONT-GRTOT PIC ZZZZZZ,ZZZ,ZZZ.
     05  FILLER                   PIC X        VALUE SPACE.
     05  EARN-GRTOT                 PIC ZZZ,ZZZ,ZZZ.99-.
     05  EARN-PTS-GRTOT REDEFINES EARN-GRTOT PIC ZZZZZZ,ZZZ,ZZZ.
     05  FILLER                   PIC X        VALUE SPACE.
     05  EARN2-GRTOT                PIC ZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X        VALUE SPACE.
     05  FORF-GRTOT                 PIC ZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X        VALUE SPACE.
     05  FILLER                   PIC X(4)     VALUE SPACE.
     05  DIST1-GRTOT                PIC ZZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X        VALUE SPACE.
     05  MIL-GRTOT                  PIC ZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X        VALUE SPACE.
     05  END-BAL-GRTOT              PIC Z,ZZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X(13)     VALUE SPACE.


* =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
* PRINT REPORT DETAILS SECTION.
* NOTE : The detail data exceeds 135 characters so we need to
*        reduce the size of Earnings2 and Military. These should
*        be this large, where the others can be.
* =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
*MAIN-1488  ADJUSTED FILLER AND BADGE DUE TO PSN EXPANSION.
*           PAYBEN INFO DID NOT LINE UP WITH EMP INFO AND SKEWED COLUMN
*           ALIGNMENT WHEN DON DOWNLOADED THE REPORT TO EXCEL.

*/
