using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
public class NavigationStatusDto
{
    public byte Id { get; set; }
    [UnmaskSensitive]public string? Name { get; set; }
}
