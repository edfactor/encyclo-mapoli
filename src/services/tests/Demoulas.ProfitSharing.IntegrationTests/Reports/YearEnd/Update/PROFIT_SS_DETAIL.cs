namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PROFIT_SS_DETAIL {
    public decimal PROFIT_SS_YEAR { get; set; } // PIC S9(4)V9(1)
    public long PROFIT_SS_CLIENT { get; set; } // PIC S9(3)
    public string? PROFIT_SS_CODE { get; set; } // PIC X(1).
    public decimal PROFIT_SS_CONT { get; set; } // PIC S9(7)V9(2)
    public decimal PROFIT_SS_EARN { get; set; } // PIC S9(7)V9(2)
    public decimal PROFIT_SS_FORT { get; set; } // PIC S9(7)V9(2)
    public long PROFIT_SS_MDTE { get; set; } // PIC S9(2)
    public long PROFIT_SS_YDTE { get; set; } // PIC S9(2)
    public string? PROFIT_SS_CMNT { get; set; } // PIC X(16).
    public string? PROFIT_SS_ZEROCONT { get; set; } // PIC X(1).
    public decimal PROFIT_SS_FED_TAXES { get; set; } // PIC S9(7)V9(2)
    public decimal PROFIT_SS_STATE_TAXES { get; set; } // PIC S9(7)V9(2)
    public string? PROFIT_SS_TAX_CODE { get; set; } // PIC X(1).
    public long PROFIT_SS_SSNO { get; set; } // PIC S9(9)
    public string? PROFIT_SS_NAME { get; set; } // PIC X(25).
    public string? PROFIT_SS_ADDRESS { get; set; } // PIC X(20).
    public string? PROFIT_SS_CITY { get; set; } // PIC X(13).
    public string? PROFIT_SS_STATE { get; set; } // PIC X(2).
    public long PROFIT_SS_ZIP { get; set; } // PIC 9(5)
}

/*

       01 PROFIT-SS-DETAIL IS EXTERNAL.
       02 PROFIT-SS-YEAR           PIC S9(4)V9(1) COMP-6.

       02 PROFIT-SS-CLIENT         PIC S9(3) COMP-6.

       02 PROFIT-SS-CODE           PIC X(1).

       02 PROFIT-SS-CONT           PIC S9(7)V9(2) COMP-6.

       02 PROFIT-SS-EARN           PIC S9(7)V9(2) COMP-6.

       02 PROFIT-SS-FORT           PIC S9(7)V9(2) COMP-6.

       02 PROFIT-SS-DATE           .

       03 PROFIT-SS-MDTE           PIC S9(2) COMP-6.

       03 PROFIT-SS-YDTE           PIC S9(2) COMP-6.

       02 PROFIT-SS-CMNT           PIC X(16).

       02 PROFIT-SS-ZEROCONT       PIC X(1).

       02 PROFIT-SS-FED-TAXES      PIC S9(7)V9(2) COMP-6.

       02 PROFIT-SS-STATE-TAXES    PIC S9(7)V9(2) COMP-6.

       02 PROFIT-SS-TAX-CODE       PIC X(1).

       02 PROFIT-SS-SSNO           PIC S9(9) COMP-6.

       02 PROFIT-SS-NAME           PIC X(25).

       02 PROFIT-SS-ADDRESS        PIC X(20).

       02 PROFIT-SS-CITY           PIC X(13).

       02 PROFIT-SS-STATE          PIC X(2).

       02 PROFIT-SS-ZIP            PIC 9(5) COMP-6.

*/
