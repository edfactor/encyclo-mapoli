using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record GrossWagesReportRequest : ProfitYearRequest
{
    private const int DefaultGrossAmount = 50000;

    [DefaultValue(DefaultGrossAmount)]
    public decimal MinGrossAmount { get; set; }

    public static new GrossWagesReportRequest RequestExample()
    {
        return new GrossWagesReportRequest()
        {
            ProfitYear = 2024,
            MinGrossAmount = DefaultGrossAmount
        };
    }
}
