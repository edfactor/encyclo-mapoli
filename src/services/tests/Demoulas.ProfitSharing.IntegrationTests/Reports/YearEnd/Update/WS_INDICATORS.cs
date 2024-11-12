namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class WS_INDICATORS {
    public long WS_END_IND { get; set; } // PIC 9
    public long WS_RERUN_IND { get; set; } // PIC 9
}

/*

 01  WS-INDICATORS.
     05  WS-END-IND               PIC 9         VALUE ZERO.
     05  WS-RERUN-IND             PIC 9         VALUE ZERO.
*/
