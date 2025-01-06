namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class Header5
{
    public string? FILLER0 { get; set; } // PIC X(04)
    public string? FILLER1 { get; set; } // PIC X(08)
    public string? FILLER2 { get; set; } // PIC X(12)
    public string? FILLER3 { get; set; } // PIC X(05)
    public string? FILLER4 { get; set; } // PIC X(11)
    public string? FILLER5 { get; set; } // PIC X(07)
    public string? FILLER6 { get; set; } // PIC X(09)
    public string? FILLER7 { get; set; } // PIC X(08)
    public string? FILLER8 { get; set; } // PIC X(05)
    public string? FILLER9 { get; set; } // PIC X(09)
    public string? FILLER10 { get; set; } // PIC X(09)
    public string? FILLER11 { get; set; } // PIC X(08)
}

/*

 01  HEADER-5.
     05  FILLER                   PIC X(04)     VALUE SPACES.
     05  FILLER                   PIC X(08)     VALUE

             "ADJ DESC".

     05  FILLER                   PIC X(12)     VALUE SPACES.

     05  FILLER                   PIC X(05)     VALUE
             "BADGE".
     05  FILLER                   PIC X(11)     VALUE SPACES.


     05  FILLER                   PIC X(07)     VALUE
             "CONTRIB".
     05  FILLER                   PIC X(09)     VALUE SPACES.
     05  FILLER                   PIC X(08)     VALUE
             "EARNINGS".
     05  FILLER                   PIC X(05)     VALUE SPACES.
     05  FILLER                   PIC X(09)     VALUE
             "EARNINGS2".
     05  FILLER                   PIC X(09)     VALUE SPACES.
     05  FILLER                   PIC X(08)     VALUE
             "FORFEITS".

*/
