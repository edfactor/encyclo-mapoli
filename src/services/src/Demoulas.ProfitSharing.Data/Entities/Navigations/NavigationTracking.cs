namespace Demoulas.ProfitSharing.Data.Entities.Navigations;
public class NavigationTracking
{
    public int Id { get; set; }
    public short NavigationId { get; set; }
    public byte? StatusId { get; set; }
    public string? Username { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public Navigation? Navigation { get; set; }
    public NavigationStatus? NavigationStatus { get; set; }

}
