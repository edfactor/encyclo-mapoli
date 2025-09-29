using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
public sealed record DistributionSearchRequest : SortedPaginationRequestDto
{
    public string? Ssn { get; set; }
    public int? BadgeNumber { get; set; }
    public short? PsnSuffix { get; set; }
    public char? DistributionFrequencyId { get; set; } 
    public char? DistributionStatusId { get; set; }
    public char? TaxCodeId { get; set; }
    public decimal? MinGrossAmount { get; set; }
    public decimal? MaxGrossAmount { get; set; }
    public decimal? MinCheckAmount { get; set; }
    public decimal? MaxCheckAmount { get; set; }

    public static DistributionSearchRequest RequestExample()
    {
        return new DistributionSearchRequest
        {
            MinGrossAmount = 1000.00M,
            MaxGrossAmount = 2000.00M,
            Skip = 0,
            Take = 25,
            SortBy = nameof(DistributionSearchResponse.BadgeNumber),
            IsSortDescending = false,
        };
    }
}
