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
        public const int FiscalClose = 14;
        
        // Clean up Reports sub-items
        public const int DemographicBadgesNotInPayProfit = 3;
        public const int DuplicateSSNsInDemographics = 4;
        public const int NegativeETVA = 5;
        public const int DuplicateNamesAndBirthdays = 6;
        
        // December Activities sub-items
        public const int MilitaryContributions = 7;
        public const int Unforfeit = 8;
        public const int Terminations = 9;
        public const int Forfeitures = 10;
        public const int DistributionsAndForfeitures = 11;
        public const int ProfitShareReport = 13;
        
        // Fiscal Close sub-items
        public const int ManageExecutiveHours = 15;
        public const int YTDWagesExtract = 16;
        public const int ProfitShareReportFinalRun = 17;
        public const int ProfitShareReportEditRun = 18;
        public const int GetEligibleEmployees = 30;
        public const int ProfitShareForfeit = 31;
        public const int ProfPayMasterUpdate = 33;
        public const int ProfShareReportByAge = 34;
        public const int ProfShareGrossRpt = 41;
        public const int ProfShareByStore = 42;
        public const int PrintProfitCerts = 49;
        public const int MasterUpdate = 60;
        public const int ProfitMasterUpdate = 62;
        public const int SaveProfPaymstr = 63;
        public const int ProfControlSheet = 64;
        public const int QPAY066AdHocReports = 65;
        
        // Prof Share Report By Age sub-items
        public const int ContributionsByAge = 35;
        public const int DistributionsByAge = 36;
        public const int ForfeituresByAge = 37;
        public const int BalanceByAge = 38;
        public const int VestedAmountsByAge = 39;
        public const int BalanceByYears = 40;
        
        // Prof Share by Store sub-items
        public const int QPAY066Under21 = 43;
        public const int QPAY066TAUnder21 = 44;
        public const int QPAY066TA = 45;
        public const int PROFALL = 46;
        public const int QNEWPROFLBL = 47;
        public const int PROFNEW = 48;
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
