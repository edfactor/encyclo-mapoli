
namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;

public class CollectTotals
{
    public decimal WS_TOT_BEGBAL { get; set; }
    public decimal WS_TOT_CONT { get; set; }
    public decimal WS_TOT_EARN { get; set; }
    public decimal WS_TOT_EARN2 { get; set; }
    public decimal WS_TOT_FORF { get; set; }
    public decimal WS_TOT_DIST1 { get; set; }
    public decimal WS_TOT_MIL { get; set; }
    public decimal WS_TOT_XFER { get; set; }
    public decimal WS_TOT_PXFER { get; set; }
    public decimal WS_TOT_ENDBAL { get; set; }
    public decimal WS_TOT_CAF { get; set; }
    public long WS_PROF_PTS_TOTAL { get; set; }
    public long WS_EARN_PTS_TOTAL { get; set; }

    public decimal MaxOverTotal { get; set; }
    public long MaxPointsTotal { get; set; }
}
