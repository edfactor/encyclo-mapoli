namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

using static Utils;

public class XEROX_HEADER {
    public string? XEROX_REC { get; set; } =  "DJDE JDE=PAY426,JDL=PAYROL,END,;"; // PIC X(32)
    public string? FILLER0 { get; set; } // PIC X(104)

    public override string ToString(){
            return
            rformat(XEROX_REC,"string?","X(32)")
            + rformat(FILLER0,"string?","X(104)")
                ;
     }
}
/*

 01  XEROX-HEADER.
     03 XEROX-REC    PIC X(32)     VALUE
          "DJDE JDE=PAY426,JDL=PAYROL,END,;".
     03 FILLER       PIC X(104)    VALUE SPACES.

* =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
* PRINT TOTALS HEADER.
* =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

*/
