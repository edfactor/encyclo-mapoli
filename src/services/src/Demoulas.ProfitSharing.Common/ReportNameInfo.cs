namespace Demoulas.ProfitSharing.Common;

public class ReportNameInfo
{
    public required string Name { get; init; }
    public required string ReportCode { get; init; }

    public static readonly ReportNameInfo DistributionAndForfeitures = new ReportNameInfo { Name = "Distributions and Forfeitures", ReportCode = "QPAY129" };
}
