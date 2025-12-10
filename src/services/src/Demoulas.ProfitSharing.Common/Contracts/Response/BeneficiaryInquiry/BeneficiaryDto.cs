using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;


public partial record BeneficiaryDto : IdRequest, INameParts, IFullNameProperty, IPhoneNumber, IEmailAddress, ICity, IIsExecutive
{
    public required short PsnSuffix { get; set; } // Suffix for hierarchy (1000, 2000, etc.)

    public required int BadgeNumber { get; set; }
    public required int DemographicId { get; set; }

    public required string Ssn { get; set; }

    [MaskSensitive] public DateOnly DateOfBirth { get; set; }

    public required string Street { get; init; }
    public string? Street2 { get; init; }
    public required string? City { get; init; }
    public required string? State { get; init; }
    public required string? PostalCode { get; init; }
    public required string CountryIso { get; init; }
    [MaskSensitive]
    public string? FullName { get; init; }
    [MaskSensitive]
    public required string LastName { get; init; }
    [MaskSensitive]
    public required string FirstName { get; init; }
    [MaskSensitive]
    public string? MiddleName { get; init; }
    public string? PhoneNumber { get; init; }
    [MaskSensitive]
    public string? MobileNumber { get; init; }
    [MaskSensitive]
    public string? EmailAddress { get; init; }

    public DateOnly CreatedDate { get; set; }
    public string? Relationship { get; set; }
    public char? KindId { get; set; }
    public BeneficiaryKindDto? Kind { get; set; }
    public required decimal Percent { get; set; }
    public decimal? CurrentBalance { get; set; }
    public required bool IsExecutive { get; set; }
}
