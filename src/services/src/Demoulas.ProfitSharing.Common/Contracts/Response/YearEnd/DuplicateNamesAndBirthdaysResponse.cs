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

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static DuplicateNamesAndBirthdaysResponse ResponseExample()
    {
        return new DuplicateNamesAndBirthdaysResponse
        {
            DemographicId = 12345,
            BadgeNumber = 987654,
            Ssn = "***-**-2468",
            Name = "Johnson, Robert W",
            DateOfBirth = new DateOnly(1980, 6, 15),
            Address = "123 Main Street",
            Street2 = "Apt 4B",
            City = "Springfield",
            State = "MA",
            PostalCode = "01103",
            CountryIso = "US",
            Years = 15,
            HireDate = new DateOnly(2009, 3, 10),
            TerminationDate = null,
            Status = 'A',
            StoreNumber = 61,
            Count = 2,
            NetBalance = 125000.50m,
            HoursCurrentYear = 2080.00m,
            IncomeCurrentYear = 65000.00m,
            EmploymentStatusName = "Active",
            IsExecutive = false,
            IsFakeSsn = false
        };
    }
}
