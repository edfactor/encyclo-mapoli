using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record GrossWagesReportRequest : ProfitYearRequest
{
    const int DefaultGrossAmount = 50000;

    [DefaultValue(DefaultGrossAmount)]
    public decimal MinGrossAmount { get; set; } 

    public static new GrossWagesReportRequest RequestExample()
    {
        return new GrossWagesReportRequest()
        {
            ProfitYear = 2023,
            MinGrossAmount = DefaultGrossAmount
        };
        
    }
}
