using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;

[NoMemberDataExposed]
public class NavigationResponseDto
{
    public List<NavigationDto>? Navigation { get; set; }

    public static NavigationResponseDto ResponseExample()
    {
        return new NavigationResponseDto
        {
            Navigation = new List<NavigationDto>
            {
                NavigationDto.ResponseExample()
            }
        };
    }
}
