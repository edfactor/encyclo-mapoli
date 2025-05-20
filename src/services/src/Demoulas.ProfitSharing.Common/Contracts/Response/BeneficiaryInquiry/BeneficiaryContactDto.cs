using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
public record BeneficiaryContactDto
{
    public int Id { get; set; }

    public  string? Ssn { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public AddressResponseDto? Address { get; set; }
    public ContactInfoResponseDto? ContactInfo { get; set; }

    public DateOnly CreatedDate { get; set; }
}
