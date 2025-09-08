using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
public record NavigationDto : IdRequest
{
    public int? ParentId { get; set; }
    public required string Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Url { get; set; }
    public byte? StatusId { get; set; }
    public string? StatusName { get; set; }
    public byte OrderNumber { get; set; }
    public string? Icon { get; set; }
    public List<string>? RequiredRoles { get; set; }
    public bool? Disabled { get; set; }
    // Prerequisite navigation elements that are currently in a Completed state.
    // No business logic is applied here; this is populated upstream or remains empty for now.
    public List<NavigationDto>? PrerequisiteNavigations { get; set; }
    public List<NavigationDto>? Items { get; set; }
}
