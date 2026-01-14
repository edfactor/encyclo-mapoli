using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request for Current Year Wages report with optional frozen data support.
/// </summary>
public record WagesCurrentYearRequest : SortedPaginationRequestDto, IProfitYearRequest
{
    /// <summary>
    /// The profit year for which to retrieve wages data.
    /// </summary>
    [DefaultValue(2024)]
    public short ProfitYear { get; set; }

    /// <summary>
    /// When true, uses frozen/archived demographic data instead of live data.
    /// This will set the X-Demographic-Data-Source header to "Frozen" and append " - Archive" to the report name.
    /// </summary>
    [DefaultValue(false)]
    public bool UseFrozenData { get; set; }

    /// <summary>
    /// Creates an example request for API documentation.
    /// </summary>
    public static WagesCurrentYearRequest RequestExample()
    {
        return new WagesCurrentYearRequest
        {
            ProfitYear = 2024,
            UseFrozenData = false,
            Skip = 0,
            Take = 25
        };
    }
}
