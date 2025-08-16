using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record PayrollDuplicateSsnResponseDto
{
    public required int BadgeNumber { get; set; }
    public required string Ssn { get; set; }
    [MaskSensitive] public string? Name { get; set; }
    public required AddressResponseDto Address { get; set; }
    public required DateOnly HireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public DateOnly? RehireDate { get; set; }
    public required char Status { get; set; }
    public required short StoreNumber { get; set; }
    public int ProfitSharingRecords { get; set; }
    public IEnumerable<PayProfitResponseDto> PayProfits { get; set; } = [];
    public required string EmploymentStatusName { get; set; }

    public static PayrollDuplicateSsnResponseDto ResponseExample()
    {
        return new PayrollDuplicateSsnResponseDto
        {
            BadgeNumber = 123,
            Ssn = "XXX-XX-6789",
            Name = "John Doe",
            Address = AddressResponseDto.ResponseExample(),
            HireDate = ReferenceData.DsmMinValue,
            TerminationDate = DateTime.Today.ToDateOnly(),
            Status = 't',
            EmploymentStatusName = "Terminated",
            StoreNumber = 6,
            ProfitSharingRecords = 17
        };
    }
}
