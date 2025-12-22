using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;

[NoMemberDataExposed]
public class UpdateNavigationStatusResponseDto
{
    public bool IsSuccessful { get; set; }

    public static UpdateNavigationStatusResponseDto ResponseExample()
    {
        return new UpdateNavigationStatusResponseDto
        {
            IsSuccessful = true
        };
    }
}
