using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request object for PayServices endpoint operations.
/// Contains identification and description for processing pay service requests.
/// </summary>
public sealed record PayServicesRequest: SortedPaginationRequestDto
{
    public static class Constants
    {
        public const char PartTime = 'P';
        public const char FullTimeStraightSalary = 'H';
        public const char FullTimeAccruedPaidHolidays = 'G';
        public const char FullTimeEightPaidHolidays = 'F';
    }

    public short ProfitYear { get; init; }

    /// <summary>
    /// Example request for API documentation and testing.
    /// </summary>
    public static PayServicesRequest RequestExample() => new()
    {
        ProfitYear = 2024
    };
}
