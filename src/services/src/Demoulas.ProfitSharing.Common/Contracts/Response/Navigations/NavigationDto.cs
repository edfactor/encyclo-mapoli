using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;

public record NavigationDto : IdRequest<int>
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
    // When false the page exists in the system but should not be shown in navigation menus/drawers.
    // If null, treated as navigable (true).
    public bool? IsNavigable { get; set; }
    // Prerequisite navigation elements that are currently in a Completed state.
    // No business logic is applied here; this is populated upstream or remains empty for now.
    public List<NavigationDto>? PrerequisiteNavigations { get; set; }
    public List<NavigationDto>? Items { get; set; }
    /// <summary>
    /// Indicates whether the current user has any read-only roles that would restrict editing capabilities.
    /// This is used by the UI to disable or hide action buttons for users in read-only roles.
    /// </summary>
    public bool IsReadOnly { get; set; }

    public static NavigationDto ResponseExample()
    {
        return new NavigationDto
        {
            Id = 1,
            ParentId = null,
            Title = "Year End",
            SubTitle = "Year-end processing",
            Url = "/year-end",
            StatusId = 1,
            StatusName = "In Progress",
            OrderNumber = 1,
            Icon = "calendar",
            RequiredRoles = new List<string> { "ADMINISTRATOR", "FINANCEMANAGER" },
            Disabled = false,
            IsNavigable = true,
            PrerequisiteNavigations = null,
            Items = null,
            IsReadOnly = false
        };
    }
}
