using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

/// <summary>
/// Individual employee detail record for the PROF-LETTER73 report.
/// Represents an employee over age 73 with profit sharing balance.
/// Badge,Name,Address,City,State,Zip,Status,DOB,Age,Masked SSN, Term Date
/// </summary>
public sealed record EmployeesWithProfitsOver73DetailDto
{
    /// <summary>
    /// Employee badge number.
    /// </summary>
    public required int BadgeNumber { get; init; }

    /// <summary>
    /// Employee full name.
    /// </summary>
    [MaskSensitive]
    public required string FullName { get; init; }

    /// <summary>
    /// Employee address.
    /// </summary>
    [MaskSensitive]
    public required string Address { get; init; }

    /// <summary>
    /// Employee city.
    /// </summary>
    public required string City { get; init; }

    /// <summary>
    /// Employee state.
    /// </summary>
    public required string State { get; init; }

    /// <summary>
    /// Employee zip code.
    /// </summary>
    public required string Zip { get; init; }

    /// <summary>
    /// Employee status (Active, Terminated, etc.).
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Employee date of birth.
    /// </summary>
    [MaskSensitive]
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Employee's current age.
    /// </summary>
    [MaskSensitive]
    public required int Age { get; init; }

    /// <summary>
    /// Masked SSN (XXX-XX-1234 format).
    /// </summary>
    public required string Ssn { get; init; }

    /// <summary>
    /// Employee termination date (if applicable).
    /// </summary>
    public required DateOnly? TerminationDate { get; init; }

    /// <summary>
    /// Current profit sharing balance.
    /// </summary>
    public required decimal Balance { get; init; }

    /// <summary>
    /// IRS life expectancy divisor for this age from RmdsFactorByAge table.
    /// Used to calculate required minimum distribution (e.g., 26.5 years for age 73).
    /// </summary>
    public required decimal Factor { get; init; }

    /// <summary>
    /// Required minimum distribution amount calculated as Balance ÷ Factor.
    /// Based on IRS Publication 590-B Uniform Lifetime Table.
    /// </summary>
    public required decimal Rmd { get; init; }

    /// <summary>
    /// Total payments (distributions) made to this employee during the fiscal year.
    /// Calculated by summing Forfeiture amounts from PROFIT_DETAIL where ProfitCodeId 
    /// is a payment/distribution code and the payment date falls within the fiscal year.
    /// </summary>
    public required decimal PaymentsInProfitYear { get; init; }

    /// <summary>
    /// Suggested RMD check amount based on de minimis threshold and payments received.
    /// Logic:
    /// - If Balance &lt;= DeMinimusValue: Returns entire remaining balance (liquidate account)
    /// - If Balance &gt; DeMinimusValue: Returns (RMD - PaymentsInProfitYear), minimum of 0
    /// This represents the recommended distribution amount to satisfy IRS RMD requirements.
    /// </summary>
    public required decimal SuggestRmdCheckAmount { get; init; }
    public static EmployeesWithProfitsOver73DetailDto ResponseExample() => new()
    {
        BadgeNumber = 45678,
        FullName = "Robert James Wilson",
        Address = "456 Oak Avenue",
        City = "Worcester",
        State = "MA",
        Zip = "01608",
        Status = "Active",
        DateOfBirth = new DateOnly(1950, 3, 15),
        Age = 74,
        Ssn = "***-**-9876",
        TerminationDate = null,
        Balance = 275000.00m,
        Factor = 25.5m,
        Rmd = 10784.31m,
        PaymentsInProfitYear = 0.00m,
        SuggestRmdCheckAmount = 10784.31m
    };
}
