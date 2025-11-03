using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
public sealed class DisbursementReportDetailResponse
{
    public char DistributionFrequencyId { get; set; }
    public required string DistributionFrequencyName { get; set; }
    public required string Ssn { get; set; }
    public int BadgeNumber { get; set; }
    public required string EmployeeName { get; set; }
    public decimal VestedBalance { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal RemainingBalance { get; set; }

    public static DisbursementReportDetailResponse SampleResponse() => new DisbursementReportDetailResponse
    {
        DistributionFrequencyId = 'A',
        DistributionFrequencyName = "Annual",
        Ssn = "XXX-XX-6789",
        BadgeNumber = 701001,
        EmployeeName = "John Doe",
        VestedBalance = 15000.00M,
        OriginalAmount = 5000.00M,
        RemainingBalance = 10000.00M
    };
}
