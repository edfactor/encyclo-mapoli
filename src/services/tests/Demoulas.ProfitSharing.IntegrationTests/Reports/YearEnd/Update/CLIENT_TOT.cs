
namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

using static Utils;

public class CLIENT_TOT {
    public string? TOT_FILLER { get; set; } =  "TOT "; // PIC X(5)
    public decimal BEG_BAL_TOT { get; set; } // PIC Z,ZZZ,ZZZ,ZZZ.99-.
    public string? FILLER0 { get; set; } // PIC X
    public decimal CONT_TOT { get; set; } // PIC ZZ,ZZZ,ZZZ.99-
    public string? FILLER1 { get; set; } // PIC X
    public decimal EARN_TOT { get; set; } // PIC ZZZ,ZZZ,ZZZ.99-.
    public string? FILLER2 { get; set; } // PIC X
    public decimal EARN2_TOT { get; set; } // PIC ZZ,ZZZ,ZZZ.99-.
    public string? FILLER3 { get; set; } // PIC X
    public decimal FORF_TOT { get; set; } // PIC ZZ,ZZZ,ZZZ.99-.
    public string? FILLER4 { get; set; } // PIC X
    public string? FILLER5 { get; set; } // PIC X(4)
    public decimal DIST1_TOT { get; set; } // PIC ZZZ,ZZZ,ZZZ.99-.
    public string? FILLER6 { get; set; } // PIC X
    public decimal MIL_TOT { get; set; } // PIC ZZ,ZZZ,ZZZ.99-.
    public string? FILLER7 { get; set; } // PIC X
    public decimal END_BAL_TOT { get; set; } // PIC Z,ZZZ,ZZZ,ZZZ.99-.
    public string? FILLER8 { get; set; } // PIC X(13)

    public override string ToString(){
            return
            rformat(TOT_FILLER,"string?","X(5)")
            + rformat(BEG_BAL_TOT,"decimal","Z,ZZZ,ZZZ,ZZZ.99-")
            + rformat(FILLER0,"string?","X")
            + rformat(CONT_TOT,"decimal","ZZ,ZZZ,ZZZ.99-")
            + rformat(FILLER1,"string?","X")
            + rformat(EARN_TOT,"decimal","ZZZ,ZZZ,ZZZ.99-")
            + rformat(FILLER2,"string?","X")
            + rformat(EARN2_TOT,"decimal","ZZ,ZZZ,ZZZ.99-")
            + rformat(FILLER3,"string?","X")
            + rformat(FORF_TOT,"decimal","ZZ,ZZZ,ZZZ.99-")
            + rformat(FILLER4,"string?","X")
            + rformat(FILLER5,"string?","X(4)")
            + rformat(DIST1_TOT,"decimal","ZZZ,ZZZ,ZZZ.99-")
            + rformat(FILLER6,"string?","X")
            + rformat(MIL_TOT,"decimal","ZZ,ZZZ,ZZZ.99-")
            + rformat(FILLER7,"string?","X")
            + rformat(END_BAL_TOT,"decimal","Z,ZZZ,ZZZ,ZZZ.99-")
            + rformat(FILLER8,"string?","X(13)")
                ;
     }
}
/*

 01  CLIENT-TOT.
     05  TOT-FILLER                   PIC X(5)     VALUE
         "TOT  ".
     05  BEG-BAL-TOT              PIC Z,ZZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X        VALUE SPACE.
     05  CONT-TOT                 PIC ZZ,ZZZ,ZZZ.99-.
     05  CONT-PTS-TOT REDEFINES CONT-TOT PIC ZZZZZ,ZZZ,ZZZ.
     05  FILLER                   PIC X        VALUE SPACE.
     05  EARN-TOT                 PIC ZZZ,ZZZ,ZZZ.99-.
     05  EARN-PTS-TOT REDEFINES EARN-TOT PIC ZZZZZZ,ZZZ,ZZZ.
     05  FILLER                   PIC X        VALUE SPACE.
     05  EARN2-TOT                PIC ZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X        VALUE SPACE.
     05  FORF-TOT                 PIC ZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X        VALUE SPACE.
     05  FILLER                   PIC X(4)     VALUE SPACE.
     05  DIST1-TOT                PIC ZZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X        VALUE SPACE.
     05  MIL-TOT                  PIC ZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X        VALUE SPACE.
     05  END-BAL-TOT              PIC Z,ZZZ,ZZZ,ZZZ.99-.
     05  FILLER                   PIC X(13)     VALUE SPACE.

*/
