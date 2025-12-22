namespace Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;

public class GetNavigationStatusRequestDto
{
    public byte? Id { get; set; }

    public static GetNavigationStatusRequestDto RequestExample()
    {
        return new GetNavigationStatusRequestDto
        {
            Id = 1
        };
    }
}
