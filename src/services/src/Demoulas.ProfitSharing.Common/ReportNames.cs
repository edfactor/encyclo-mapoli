namespace Demoulas.ProfitSharing.Common;

public class ReportNames
{
    public required string Name { get; init; }
    public required string ReportCode { get; init; }

    public static readonly ReportNames DistributionAndForfeitures = new ReportNames { Name = "Distributions and Forfeitures", ReportCode = "QPAY129" };
    public static readonly ReportNames ProfitSharingSummary = new ReportNames { Name = "Profit Sharing Summary", ReportCode = "PAY426N9" };
}
