namespace Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;

public record NavigationRequestDto
{
    public int? NavigationId { get; set; }

    public static NavigationRequestDto RequestExample()
    {
        return new NavigationRequestDto
        {
            NavigationId = 100
        };
    }
}
