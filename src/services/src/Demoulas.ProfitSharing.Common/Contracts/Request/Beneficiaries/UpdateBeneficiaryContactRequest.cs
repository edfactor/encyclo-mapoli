namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
public record UpdateBeneficiaryContactRequest:IdRequest
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
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    public string? EmailAddress { get; set; }

    public static UpdateBeneficiaryContactRequest SampleRequest()
    {
        return new UpdateBeneficiaryContactRequest() { Id = 55, LastName = "Johnson" };
    }
}
