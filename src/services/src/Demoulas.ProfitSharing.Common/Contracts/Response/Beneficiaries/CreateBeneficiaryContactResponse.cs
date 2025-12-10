using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;

using Shared;

[NoMemberDataExposed] //If you created the beneficiary, you should be able to see it immediately afterwards.
public sealed record CreateBeneficiaryContactResponse : INameParts, IEmailAddress, IPhoneNumber, ICity
{
    public required int Id { get; set; }
    public required string Ssn { get; set; }

    [MaskSensitive]
    public required DateOnly DateOfBirth { get; set; }
    [MaskSensitive] public required string Street { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? Street4 { get; set; }
    [MaskSensitive] public string? City { get; init; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public string? CountryIso { get; set; }
    [MaskSensitive] public required string FirstName { get; init; }
    [MaskSensitive] public required string LastName { get; init; }
    [MaskSensitive] public string? MiddleName { get; init; }
    public string? PhoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    [MaskSensitive] public string? EmailAddress { get; set; }

    public static CreateBeneficiaryContactResponse SampleResponse() => new CreateBeneficiaryContactResponse
    {
        Id = 1,
        Ssn = "XXX-XX-1423",
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
