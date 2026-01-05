namespace Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;

public record UpdateNavigationRequestDto
{
    public short NavigationId { get; set; }
    public byte StatusId { get; set; }

    public static UpdateNavigationRequestDto RequestExample()
    {
        return new UpdateNavigationRequestDto
        {
            NavigationId = 100,
            StatusId = 1
        };
    }
}
