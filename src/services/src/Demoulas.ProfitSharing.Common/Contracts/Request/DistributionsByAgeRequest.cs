using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record DistributionsByAgeRequest : ProfitYearRequest
{
    public enum Report
    {
        Total = 0,
        FullTime = 1,
        PartTime = 2,
    }

    [DefaultValue(Report.Total)]
    public Report ReportType { get; set; }

    [DefaultValue(byte.MaxValue)]
    public override int? Take { get; init; }

    public static new DistributionsByAgeRequest RequestExample()
    {
        return new DistributionsByAgeRequest
        {
            ReportType = Report.Total,
            ProfitYear = 2023
        };
    }
}
