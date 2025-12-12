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
    [MaskSensitive]
    public required int BadgeNumber { get; init; }

    /// <summary>
    /// Employee full name.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Employee full name.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Employee full name.
    /// </summary>
    public string? MiddleInitial { get; init; }

    /// <summary>
    /// Employee full name.
    /// </summary>
    public required string Name { get; init; }

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
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Employee's current age.
    /// </summary>
    public required int Age { get; init; }

    /// <summary>
    /// Masked SSN (XXX-XX-1234 format).
    /// </summary>
    [MaskSensitive]
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
    /// minimum required distribution based on age from the 
    /// </summary>
    public required decimal RequiredMinimumDistributions { get; init; }
}
