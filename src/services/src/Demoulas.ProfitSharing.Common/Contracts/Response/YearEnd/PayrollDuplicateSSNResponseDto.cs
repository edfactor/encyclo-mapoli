using System.Data.SqlTypes;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record PayrollDuplicateSsnResponseDto
{
    public required int BadgeNumber { get; set; }
    public required string Ssn { get; set; }
    public string? Name { get; set; }
    public required AddressResponseDto Address { get; set; }
    public required DateOnly HireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public DateOnly? RehireDate { get; set; }
    public required char Status { get; set; }
    public required short StoreNumber { get; set; }
    public required int ProfitSharingRecords { get; set; }
    public required decimal HoursCurrentYear { get; set; }
    public decimal HoursLastYear { get; set; }
    public required decimal IncomeCurrentYear { get; set; }

    public static PayrollDuplicateSsnResponseDto ResponseExample()
    {
        return new PayrollDuplicateSsnResponseDto
        {
            BadgeNumber = 123,
            Ssn = 123_45_6789,
            Name = "John Doe",
            Address = AddressResponseDto.ResponseExample(),
            HireDate = SqlDateTime.MinValue.Value.ToDateOnly(),
            TerminationDate = DateTime.Today.ToDateOnly(),
            Status = 't',
            StoreNumber = 6,
            ProfitSharingRecords = 17,
            HoursCurrentYear = 1024,
            HoursLastYear = 2048,
            IncomeCurrentYear = ushort.MaxValue
        };
    }
}
