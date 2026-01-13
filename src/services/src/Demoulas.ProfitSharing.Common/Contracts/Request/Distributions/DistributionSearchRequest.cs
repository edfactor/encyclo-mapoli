using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;

public sealed record DistributionSearchRequest : SortedPaginationRequestDto
{
    /// <summary>
    /// Property name to sort by (e.g., "BadgeNumber", "FullName", "GrossAmount").
    /// Use prefix '-' for descending (e.g., "-BadgeNumber"). Supports comma-separated multi-sort.
    /// </summary>
    /// <example>BadgeNumber</example>
    [DefaultValue(null)]
    public new string? SortBy { get; set; }

    /// <summary>
    /// When true, sorts in descending order. Ignored if SortBy uses '-' prefix notation.
    /// </summary>
    [DefaultValue(false)]
    public new bool? IsSortDescending { get; set; }

    public string? Ssn { get; set; }
    public long? BadgeNumber { get; set; }
    public short? PsnSuffix { get; set; }
    public byte? MemberType { get; set; } // 1 = employees, 2 = beneficiaries, null = all
    public char? DistributionFrequencyId { get; set; }
    public char? DistributionStatusId { get; set; }
    public List<string>? DistributionStatusIds { get; set; }
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
