using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record DuplicateNamesAndBirthdaysResponse : IIsExecutive
{
    public required int DemographicId { get; set; }
    public required int BadgeNumber { get; set; }
    public required string Ssn { get; set; }
    [MaskSensitive] public string? Name { get; set; }
    [MaskSensitive] public required DateOnly DateOfBirth { get; set; }
    [MaskSensitive] public required string Address { get; init; }
    public string? Street2 { get; init; }
    [MaskSensitive] public required string? City { get; init; }
    public required string? State { get; init; }
    public required string? PostalCode { get; init; }
    public required string CountryIso { get; init; }
    public byte Years { get; set; }
    public DateOnly HireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public required char Status { get; set; }
    public required short StoreNumber { get; set; }
    public required int Count { get; set; }
    public decimal NetBalance { get; set; }
    public required decimal? HoursCurrentYear { get; set; }
    public required decimal? IncomeCurrentYear { get; set; }
    public required string EmploymentStatusName { get; set; }
    public required bool IsExecutive { get; set; }
    public bool IsFakeSsn { get; set; }
}
