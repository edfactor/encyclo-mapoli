
namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

internal sealed record DemographicMatchDto
{
    public required int demographicId { get; set; }
    public required long MatchedDemographicId { get; set; }
}
