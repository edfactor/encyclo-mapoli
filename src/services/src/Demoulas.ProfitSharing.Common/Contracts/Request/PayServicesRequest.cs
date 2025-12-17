namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request object for PayServices endpoint operations.
/// Contains identification and description for processing pay service requests.
/// </summary>
public sealed record PayServicesRequest : ProfitYearRequest
{
    /// <summary>
    /// Example request for API documentation and testing.
    /// </summary>
    public static new PayServicesRequest RequestExample() => new()
    {
        ProfitYear = 2024
    };
}
