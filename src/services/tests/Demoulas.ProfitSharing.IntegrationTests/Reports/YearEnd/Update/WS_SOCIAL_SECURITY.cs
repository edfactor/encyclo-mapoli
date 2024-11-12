namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class WS_SOCIAL_SECURITY {
    public long WS_SSN { get; set; } // PIC 9(9)
    public long WS_SSN1 { get; set; } // PIC 999.
    public long WS_SSN2 { get; set; } // PIC 99.
    public long WS_SSN3 { get; set; } // PIC 9(4).
}

/*

 01  WS-SOCIAL-SECURITY.
   03  WS-SSN                     PIC 9(9)      VALUE ZERO.
   03  DUMMY REDEFINES WS-SSN.
     05  WS-SSN1                  PIC 999.
     05  WS-SSN2                  PIC 99.
     05  WS-SSN3                  PIC 9(4).

*/
