namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PRINT_ADJ_LINE2 {
    public string? FILLER0 { get; set; } // PIC X(5)
    public string? FILLER1 { get; set; } // PIC X(40)
}

/*

 01  PRINT-ADJ-LINE2.
     05  FILLER                   PIC X(5)      VALUE SPACES.
     05  FILLER                   PIC X(40)     VALUE
               "** NO ADJUSTMENT - EMPLOYEE NOT FOUND **".
*/
