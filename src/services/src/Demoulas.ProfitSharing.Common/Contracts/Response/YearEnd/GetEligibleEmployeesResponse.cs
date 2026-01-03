using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record GetEligibleEmployeesResponse : ReportResponseBase<EligibleEmployee>
{
    [YearEndArchiveProperty]
    public required int NumberReadOnFrozen { get; set; }
    [YearEndArchiveProperty]
    public required int NumberNotSelected { get; set; }
    [YearEndArchiveProperty]
    public required int NumberWritten { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static GetEligibleEmployeesResponse ResponseExample()
    {
        return new GetEligibleEmployeesResponse
        {
            ReportName = "eligible-employees",
            ReportDate = DateTimeOffset.UtcNow,
            NumberReadOnFrozen = 2050,
            NumberNotSelected = 10,
            NumberWritten = 2040,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<EligibleEmployee>
            {
                Results = new List<EligibleEmployee>
                {
                    new EligibleEmployee { OracleHcmId = 123456, BadgeNumber = 100001, FullName = "John Smith", IsExecutive = false }
                }
            }
        };
    }
}
