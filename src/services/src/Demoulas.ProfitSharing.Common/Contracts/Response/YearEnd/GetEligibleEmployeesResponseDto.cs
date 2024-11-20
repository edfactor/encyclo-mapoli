
namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record  GetEligibleEmployeesResponseDto {
    public required long OracleHcmId { get; set; }
    public required int BadgeNumber { get; set; }
    public required string FullName { get; set; }
    public static GetEligibleEmployeesResponseDto Example()
    {
        return new GetEligibleEmployeesResponseDto
        {
            OracleHcmId = 42,
            BadgeNumber = 721,
            FullName = "John, Null E"
        };
    }
}
