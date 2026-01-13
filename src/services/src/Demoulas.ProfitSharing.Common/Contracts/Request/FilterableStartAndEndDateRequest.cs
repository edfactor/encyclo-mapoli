using Demoulas.ProfitSharing.Common.Enums;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Fit-for-purpose DTO for endpoints that need to filter based on balance and vesting status.
/// Inherits from <see cref="StartAndEndDateRequest"/> to reuse common date range and pagination properties.
/// </summary>
public record FilterableStartAndEndDateRequest : StartAndEndDateRequest
{
    public bool ExcludeZeroBalance { get; set; } = false;
    public bool ExcludeZeroAndFullyVested { get; set; } = false;

    /// <summary>
    /// Optional vested balance value to filter against. Requires VestedBalanceOperator to be set.
    /// </summary>
    public decimal? VestedBalanceValue { get; set; }

    /// <summary>
    /// Optional comparison operator for vested balance filter (Equals, LessThan, LessThanOrEqual, GreaterThan, GreaterThanOrEqual).
    /// Only applied if VestedBalanceValue is provided.
    /// </summary>
    public ComparisonOperator? VestedBalanceOperator { get; set; }

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
            ExcludeZeroAndFullyVested = false,
            VestedBalanceValue = 5000.00m,
            VestedBalanceOperator = ComparisonOperator.GreaterThanOrEqual
        };
    }
}
