namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request for the PROF-LETTER73 adhoc report - Employees with profits over 73.
/// Inherits all properties from ProfitYearRequest (ProfitYear, Skip, Take, SortBy, etc.).
/// </summary>
public record EmployeesWithProfitsOver73Request : ProfitYearRequest
{
    /// <summary>
    /// Optional list of badge numbers to filter employees for form letter generation.
    /// If null or empty, all eligible employees over 73 will be included.
    /// </summary>
    public List<int>? BadgeNumbers { get; init; }

    /// <summary>
    /// De minimis threshold value for determining RMD payment logic.
    /// If an employee's balance is at or below this amount, the suggested RMD check amount
    /// will be the entire remaining balance (liquidate account).
    /// If above this amount, the suggested check will be RMD minus payments already received.
    /// Defaults to $5,000 if not specified.
    /// </summary>
    public decimal DeMinimusValue { get; init; } = 5000m;

    public static new EmployeesWithProfitsOver73Request RequestExample()
    {
        return new EmployeesWithProfitsOver73Request
        {
            ProfitYear = 2023,
            Skip = 0,
            Take = 100,
            BadgeNumbers = null, // Optional: Can include [12345, 67890] for specific employees
            DeMinimusValue = 5000m
        };
    }
}
