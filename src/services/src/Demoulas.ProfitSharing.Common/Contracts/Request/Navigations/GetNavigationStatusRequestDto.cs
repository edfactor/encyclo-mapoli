namespace Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;

public record GetNavigationStatusRequestDto
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
