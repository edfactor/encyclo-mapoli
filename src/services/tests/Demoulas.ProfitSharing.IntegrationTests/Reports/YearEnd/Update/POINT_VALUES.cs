namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class POINT_VALUES
{
    public decimal PV_CONT_01 { get; set; }

    public decimal PV_FORF_01 { get; set; }
    public decimal PV_EARN_01 { get; set; }
    public decimal PV_EARN2_01 { get; set; }
    public long PV_ADJUST_BADGE { get; set; }
    public long PV_ADJUST_BADGE2 { get; set; }
    public decimal PV_ADJ_EARN { get; set; }
    public decimal PV_ADJ_EARN2 { get; set; }
    public decimal PV_ADJ_FORFEIT { get; set; }
    public decimal PV_ADJ_CONTRIB { get; set; }


}

public class AdjustmentReport {
   
    public decimal SV_FORF_AMT { get; set; } 
    public decimal SV_FORF_ADJUSTED { get; set; } 
    public decimal SV_EARN_AMT { get; set; } 
    public decimal SV_EARN_ADJUSTED { get; set; } 
    public decimal SV_EARN2_AMT { get; set; } 
    public decimal SV_EARN2_ADJUSTED { get; set; } 
    public decimal SV_CONT_AMT { get; set; } 
    public decimal SV_CONT_ADJUSTED { get; set; } 
}

