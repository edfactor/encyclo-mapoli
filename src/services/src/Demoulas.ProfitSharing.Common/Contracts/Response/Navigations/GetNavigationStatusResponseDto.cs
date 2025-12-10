using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;

[NoMemberDataExposed]
public class GetNavigationStatusResponseDto
{
    public List<NavigationStatusDto>? NavigationStatusList { get; set; }
}
