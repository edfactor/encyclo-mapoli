using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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
