using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request for grand totals breakdown by store with optional under-21 participant filtering.
/// </summary>
public record GrandTotalsByStoreRequest : YearRequest
{
    /// <summary>
    /// When true, filters to only include participants under 21 years of age.
    /// Age is calculated from DateOfBirth relative to the fiscal end date for the profit year.
    /// </summary>
    [DefaultValue(false)]
    public bool Under21Participants { get; set; } = false;

    public static new GrandTotalsByStoreRequest RequestExample()
    {
        return new GrandTotalsByStoreRequest
        {
            ProfitYear = 2024,
            Under21Participants = false
        };
    }
}
