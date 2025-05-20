namespace Demoulas.ProfitSharing.Common.Contracts.Response.Headers;

/// <summary>Convenience constants for the response headers we’ll emit.</summary>
public static class DemographicHeaders
{
    public const string Source = "X-Demographic-Data-Source"; // Live | Frozen
    public const string End = "X-Demographic-Window-End";
}
