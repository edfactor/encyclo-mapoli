namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public record PROFIT_DETAIL
{
    public decimal PROFIT_YEAR { get; set; } // PIC S9(4)V9(1)
    public byte PROFIT_CODE { get; set; } // PIC X(1).
    public decimal PROFIT_CONT { get; set; } // PIC S9(7)V9(2)
    public decimal PROFIT_EARN { get; set; } // PIC S9(7)V9(2)
    public decimal PROFIT_FORT { get; set; } // PIC S9(7)V9(2)
    public long PROFIT_MDTE { get; set; } // PIC S9(2)
    public long PROFIT_YDTE { get; set; } // PIC S9(2)
    public string? PROFIT_CMNT { get; set; } // PIC X(16).
    public string? PROFIT_ZEROCONT { get; set; } // PIC X(1).
    public decimal PROFIT_FED_TAXES { get; set; } // PIC S9(7)V9(2)
    public decimal PROFIT_STATE_TAXES { get; set; } // PIC S9(7)V9(2)
    public string? PROFIT_TAX_CODE { get; set; } // PIC X(1).
}

/*

       01 PROFIT-DETAIL IS EXTERNAL.
       02 PROFIT-YEAR              PIC S9(4)V9(1) COMP-6.

       02 PROFIT-CLIENT            PIC S9(3) COMP-6.

       02 PROFIT-CODE              PIC X(1).

       02 PROFIT-CONT              PIC S9(7)V9(2) COMP-6.

       02 PROFIT-EARN              PIC S9(7)V9(2) COMP-6.

       02 PROFIT-FORT              PIC S9(7)V9(2) COMP-6.

       02 PROFIT-DATE              .

       03 PROFIT-MDTE              PIC S9(2) COMP-6.

       03 PROFIT-YDTE              PIC S9(2) COMP-6.

       02 PROFIT-CMNT              PIC X(16).

       02 PROFIT-ZEROCONT          PIC X(1).

       02 PROFIT-FED-TAXES         PIC S9(7)V9(2) COMP-6.

       02 PROFIT-STATE-TAXES       PIC S9(7)V9(2) COMP-6.

       02 PROFIT-TAX-CODE          PIC X(1).

*/
