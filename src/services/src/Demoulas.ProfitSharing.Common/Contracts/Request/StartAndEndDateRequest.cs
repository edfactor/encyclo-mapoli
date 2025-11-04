

using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record StartAndEndDateRequest : SortedPaginationRequestDto
{
    public DateOnly BeginningDate { get; set; }
    public DateOnly EndingDate { get; set; }

    public static StartAndEndDateRequest RequestExample()
    {
        return new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2019, 01, 01),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 1,
            Take = 10,
            SortBy = "BadgeNumber",
            IsSortDescending = false
        };
    }
}

/// <summary>
/// Fit-for-purpose DTO for endpoints that need to filter based on balance and vesting status.
/// Inherits from <see cref="StartAndEndDateRequest"/> to reuse common date range and pagination properties.
/// </summary>
public record FilterableStartAndEndDateRequest : StartAndEndDateRequest
{
    public bool ExcludeZeroBalance { get; set; } = false;
    public bool ExcludeZeroAndFullyVested { get; set; } = false;

    public static new FilterableStartAndEndDateRequest RequestExample()
    {
        return new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2019, 01, 01),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 1,
            Take = 10,
            SortBy = "BadgeNumber",
            IsSortDescending = false,
            ExcludeZeroBalance = false,
            ExcludeZeroAndFullyVested = false
        };
    }
}
