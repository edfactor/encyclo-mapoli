using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
public sealed record DistributionSearchResponse : IIsExecutive
{
    public required string Ssn { get; set; }
    public int? BadgeNumber { get; set; }
    public required string FullName { get; set; }
    public bool IsExecutive { get; set; }
    public char FrequencyId { get; set; }
    public required string FrequencyName { get; set; }
    public char StatusId { get; set; }
    public required string StatusName { get; set; }
    public char TaxCodeId { get; set; }
    public required string TaxCodeName { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal FederalTax { get; set; }
    public decimal StateTax { get; set; }
    public decimal CheckAmount { get; set; }

    public static DistributionSearchResponse SampleResponse()
    {
        var response = new DistributionSearchResponse
        {
            Ssn = "XXX-XX-1234",
            BadgeNumber = 701001,
            FullName = "John Doe",
            IsExecutive = false,
            FrequencyId = 'W',
            FrequencyName = "Weekly",
            StatusId = 'P',
            StatusName = "Processed",
            TaxCodeId = 'A',
            TaxCodeName = "Standard",
            GrossAmount = 1500.00M,
            FederalTax = 150.00M,
            StateTax = 75.00M,
            CheckAmount = 1275.00M
        };

        return response;
    }
}
