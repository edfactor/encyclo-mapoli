using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;

[NoMemberDataExposed]
public class NavigationResponseDto
{
    public List<NavigationDto>? Navigation { get; set; }

    public Dictionary<string, object?>? CustomSettings { get; set; }

    public static NavigationResponseDto ResponseExample()
    {
        return new NavigationResponseDto
        {
            CustomSettings = new Dictionary<string, object?>
            {
                [NavigationCustomSettingsKeys.TrackPageStatus] = true,
                [NavigationCustomSettingsKeys.UseFrozenYear] = true
            },
            Navigation = new List<NavigationDto>
            {
                NavigationDto.ResponseExample()
            }
        };
    }
}
