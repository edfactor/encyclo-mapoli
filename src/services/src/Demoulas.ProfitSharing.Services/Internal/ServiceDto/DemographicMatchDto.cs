
namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

internal sealed record DemographicMatchDto
{
    public required int DemographicId { get; set; }
    public required long MatchedDemographicId { get; set; }
}
