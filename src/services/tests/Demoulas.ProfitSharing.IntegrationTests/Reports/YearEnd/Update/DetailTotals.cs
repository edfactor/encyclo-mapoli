namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public record DetailTotals
{

    public decimal DIST_TOTAL { get; set; }
    public decimal FORFEIT_TOTAL { get; set; }
    public decimal ALLOCATION_TOTAL { get; set; }
    public decimal PALLOCATION_TOTAL { get; set; }

    public decimal WS_PROF_MIL { get; set; }
    public decimal WS_PROF_CAF { get; set; }


}
