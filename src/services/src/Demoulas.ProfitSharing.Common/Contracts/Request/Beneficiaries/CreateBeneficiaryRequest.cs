using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
public sealed record CreateBeneficiaryRequest
{
    public int EmployeeBadgeNumber { get; set; }
    public int BeneficiarySsn { get; set; }
    public byte? FirstLevelBeneficiaryNumber { get; set; }
    public byte? SecondLevelBeneficiaryNumber { get; set; }
    public byte? ThirdLevelBeneficiaryNumber { get; set; }
    public required string Relationship { get; set; }
    public required char KindId { get; set; }
    public byte Percentage { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public required string Street { get; set; }
    public string? Street2 { get; set; }
    public string? Street3 { get; set; }
    public string? Street4 { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public string? CountryIso { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    public string? EmailAddress { get; set; }
}
