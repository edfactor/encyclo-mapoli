namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class Header4
{
    public string? FILLER0 { get; set; } // PIC X(34)
    public string? FILLER1 { get; set; } = "  A D J U S T M E N T S   "; // PIC X(26)
    public string? FILLER2 { get; set; } = " E N T E R E D"; // PIC X(40)
}

/*

 01  HEADER-4.
     05  FILLER                   PIC X(34)     VALUE SPACES.
     05  FILLER                   PIC X(26)     VALUE
               "  A D J U S T M E N T S   ".
     05  FILLER                   PIC X(40)     VALUE
               " E N T E R E D".

*/
