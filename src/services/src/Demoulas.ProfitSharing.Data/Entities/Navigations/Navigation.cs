namespace Demoulas.ProfitSharing.Data.Entities.Navigations;
public sealed class Navigation
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public required string Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Url { get; set; }
    public byte? StatusId { get; set; }
    public byte OrderNumber { get; set; }
    public bool? Disabled { get; set; }
    public string? Icon { get; set; }

    public List<NavigationRole>? RequiredRoles { get; set; } = [];
    public NavigationStatus? NavigationStatus { get; set; }
    public List<Navigation>? Items { get; set; }    
    public Navigation? Parent { get; set; }
    public List<NavigationTracking>? NavigationTrackings { get; set; }
}
