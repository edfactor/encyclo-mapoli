namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;

using Demoulas.ProfitSharing.Common.Attributes;
using Shared;
public sealed record CreateBeneficiaryContactRequest : INameParts, IPhoneNumber, IEmailAddress, ICity
{
    public required int ContactSsn { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public required string Street { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? Street4 { get; set; }
    public required string? City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public string? CountryIso { get; set; }

    [MaskSensitive]
    public required string FirstName { get; set; }

    [MaskSensitive]
    public required string LastName { get; set; }

    [MaskSensitive]
    public string? MiddleName { get; set; }
    public string? PhoneNumber { get; set; }

    [MaskSensitive]
    public string? MobileNumber { get; set; }

    [MaskSensitive]
    public string? EmailAddress { get; set; }

    public static CreateBeneficiaryContactRequest SampleRequest() => new CreateBeneficiaryContactRequest
    {
        ContactSsn = 700001423,
        DateOfBirth = new DateOnly(1990, 1, 1),
        Street = "123 Main St",
        City = "Anytown",
        State = "MA",
        PostalCode = "02139",
        FirstName = "John",
        LastName = "Doe",
        PhoneNumber = "508-555-1234",
    };
}
