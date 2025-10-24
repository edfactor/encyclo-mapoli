using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PayServices;

/// <summary>
/// Response object for PayServices endpoint operations.
/// Contains demographic information retrieved from pay services.
/// </summary>
[NoMemberDataExposed]
public sealed record PayServicesResponse
{
    /// <summary>
    /// The demographic value retrieved from the service.
    /// </summary>
    public PaginatedResponseDto<PayServicesDto>? PayServicesForYear { get; set; }
    public required short ProfitYear { get; init; }
    public string? Description { get; set; } = String.Empty;
    public int TotalEmployeeNumber { get; set; }
    public decimal TotalEmployeesWages { get; set; }

    /// <summary>
    /// Example response for API documentation and testing.
    /// </summary>
    public static PayServicesResponse ResponseExample() => new()
    {
        ProfitYear = 2025,
        PayServicesForYear = new PaginatedResponseDto<PayServicesDto>(),
        Description = "Sample Pay Service data retrieved successfully",
        TotalEmployeeNumber = 150
    };
}
