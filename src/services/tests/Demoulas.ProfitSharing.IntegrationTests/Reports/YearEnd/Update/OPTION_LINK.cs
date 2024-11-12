namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class OPTION_LINK {
    public long LENGTH_OF { get; set; } // PIC S9(9)
    public string? OPT_CHAR { get; set; } // PIC X
}

/*

 01  OPTION-LINK.
     02  LENGTH-OF PIC S9(9) COMP-5.
     02  TEXTE.
         03  OPT-CHAR  PIC X OCCURS 1 TO 80
*/
