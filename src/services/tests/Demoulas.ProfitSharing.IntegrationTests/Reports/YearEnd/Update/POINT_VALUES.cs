namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class POINT_VALUES
{
    public decimal PV_CONT_01 { get; set; } // PIC S999V999999
    public decimal PV_FORF_01 { get; set; } // PIC S999V999999
    public decimal PV_EARN_01 { get; set; } // PIC S999V999999
    public decimal PV_EARN2_01 { get; set; } // PIC S999V999999
    public long PV_ADJUST_BADGE { get; set; } // PIC 9(7)
    public long PV_ADJUST_BADGE2 { get; set; } // PIC 9(7)
    public decimal PV_ADJ_EARN { get; set; } // PIC S999V99
    public decimal PV_ADJ_EARN2 { get; set; } // PIC S999V99
    public decimal PV_ADJ_FORFEIT { get; set; } // PIC S999V99
    public decimal PV_ADJ_CONTRIB { get; set; } // PIC S9(8)V99


    public long SV_SSN { get; set; } // PIC 9(9)
    public decimal SV_FORF_AMT { get; set; } // PIC S9(8)V99
    public decimal SV_FORF_ADJUSTED { get; set; } // PIC S9(8)V99
    public decimal SV_EARN_AMT { get; set; } // PIC S9(8)V99
    public decimal SV_EARN_ADJUSTED { get; set; } // PIC S9(8)V99
    public decimal SV_EARN2_AMT { get; set; } // PIC S9(8)V99
    public decimal SV_EARN2_ADJUSTED { get; set; } // PIC S9(8)V99
    public decimal SV_CONT_AMT { get; set; } // PIC S9(8)V99
    public decimal SV_CONT_ADJUSTED { get; set; } // PIC S9(8)V99
}

/*

 01  POINT-VALUES.
   03  CONTRIBUTION-POINTS.
     05  PV-CONT-01               PIC S999V999999    VALUE ZERO.
   03  FORFEITURE-POINTS.
     05  PV-FORF-01               PIC S999V999999    VALUE ZERO.
   03  EARNINGS-POINTS.
     05  PV-EARN-01               PIC S999V999999    VALUE ZERO.
     05  PV-EARN2-01               PIC S999V999999    VALUE ZERO.

   03  ADJUSTMENTS.
     05  PV-ADJUST-BADGE          PIC 9(7)           VALUE ZERO.
     05  PV-ADJUST-BADGE2         PIC 9(7)           VALUE ZERO.
     05  PV-ADJ-EARN              PIC S999V99        VALUE ZERO.
     05  PV-ADJ-EARN2             PIC S999V99        VALUE ZERO.
     05  PV-ADJ-FORFEIT           PIC S999V99        VALUE ZERO.
     05  PV-ADJ-CONTRIB           PIC S9(8)V99       VALUE ZERO.
     05  SV-SSN                   PIC 9(9)           VALUE ZERO.
     05  SV-FORF-AMT              PIC S9(8)V99       VALUE ZERO.
     05  SV-FORF-ADJUSTED         PIC S9(8)V99       VALUE ZERO.
     05  SV-EARN-AMT              PIC S9(8)V99       VALUE ZERO.
     05  SV-EARN-ADJUSTED         PIC S9(8)V99       VALUE ZERO.
     05  SV-EARN2-AMT             PIC S9(8)V99       VALUE ZERO.
     05  SV-EARN2-ADJUSTED        PIC S9(8)V99       VALUE ZERO.
     05  SV-CONT-AMT              PIC S9(8)V99       VALUE ZERO.
     05  SV-CONT-ADJUSTED         PIC S9(8)V99       VALUE ZERO.


*/
