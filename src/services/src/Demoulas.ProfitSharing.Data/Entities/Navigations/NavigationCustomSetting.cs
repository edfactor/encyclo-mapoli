namespace Demoulas.ProfitSharing.Data.Entities.Navigations;

public sealed class NavigationCustomSetting
{
    public short NavigationId { get; set; }
    public required string Key { get; set; }
    public required string ValueJson { get; set; }

    public Navigation? Navigation { get; set; }
}
