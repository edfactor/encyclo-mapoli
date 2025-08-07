namespace Demoulas.ProfitSharing.Data.Entities.Navigations;
public sealed class Navigation
{
    public static class Constants
    {
        // Main menu items
        public const int Inquiries = 50;
        public const int Beneficiaries = 52;
        public const int Distributions = 53;
        public const int Reconciliation = 54;
        public const int YearEnd = 55;
        public const int ItOperations = 56;
        
        // Inquiries sub-items
        public const int MasterInquiry = 51;
        
        // IT Operations sub-items
        public const int DemographicFreeze = 57;
        
        // Year End sub-items
        public const int DecemberActivities = 1;
        public const int CleanupReports = 2;
    }

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
