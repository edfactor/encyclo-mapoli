namespace Demoulas.ProfitSharing.OracleHcm.Atom;

public class AtomFeedRecord
{
    public required long PersonId { get; init; }
    public required int EmployeeId { get; init; }
    public required string FirstName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public required string LastName { get; init; } = string.Empty;
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? ZipCode { get; init; }
    public string? DepartmentCode { get; init; }
    public char EmploymentType { get; init; }
    public required DateOnly HireDate { get; init; }
    public DateOnly? TerminationDate { get; init; }
}
