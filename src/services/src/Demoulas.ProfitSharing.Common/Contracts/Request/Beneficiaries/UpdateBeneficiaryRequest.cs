namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
public sealed record UpdateBeneficiaryRequest : IdRequest
{
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public required string Street { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? Street4 { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public string? CountryIso { get; set; }
    public required char KindId { get; set; }
    public int? BeneficiarySsn { get; set; }
    public string? Relationship { get; set; }

    public static UpdateBeneficiaryRequest SampleRequest()
    {
        return new UpdateBeneficiaryRequest()
        {
            Id = 1,
            LastName = "Guerrero",
            FirstName = "Marcus",
            Street = "15 Main St",
            City = "Tewksbury",
            State = "MA",
            PostalCode = "01862",
            KindId = 'P',
            BeneficiarySsn = 111223333,
            Relationship = "Cousin"
        };
    }
}
