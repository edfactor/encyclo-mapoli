using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
public record UpdateBeneficiaryContactResponse
{
    public int Id { get; set; }
    public required string Ssn { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public required string Street1 { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? Street4 { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public string? CountryIso { get; set; }
    public required string FullName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    public string? EmailAddress { get; set; }

    public static UpdateBeneficiaryContactResponse SampleResponse()
    {
        return new UpdateBeneficiaryContactResponse
        {
            Id = 55,
            Ssn = "XXX-XX-6789",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Street1 = "123 Main St",
            City = "Anytown",
            State = "MA",
            PostalCode = "02111",
            FullName = "Doe, John",
            FirstName = "John",
            LastName = "Doe"
        };
    }
}
