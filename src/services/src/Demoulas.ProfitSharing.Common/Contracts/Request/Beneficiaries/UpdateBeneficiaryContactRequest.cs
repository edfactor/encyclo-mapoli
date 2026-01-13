namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;

using Demoulas.ProfitSharing.Common.Attributes;
using Shared;
public record UpdateBeneficiaryContactRequest : IdRequest<int>, INameParts, IPhoneNumber, IEmailAddress, ICity
{
    public int? ContactSsn { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Street1 { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? Street4 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryIso { get; set; }

    [MaskSensitive]
    public string FirstName { get; set; } = null!;

    [MaskSensitive]
    public string LastName { get; set; } = null!;

    [MaskSensitive]
    public string? MiddleName { get; set; }
    public string? PhoneNumber { get; set; }

    [MaskSensitive]
    public string? MobileNumber { get; set; }

    [MaskSensitive]
    public string? EmailAddress { get; set; }

    public static UpdateBeneficiaryContactRequest SampleRequest()
    {
        return new UpdateBeneficiaryContactRequest() { Id = 55, LastName = "Johnson" };
    }
}
