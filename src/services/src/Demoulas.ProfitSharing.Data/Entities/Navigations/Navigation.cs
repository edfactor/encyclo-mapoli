namespace Demoulas.ProfitSharing.Data.Entities.Navigations;

public sealed class Navigation
{
    public static class Constants
    {
        public const short Unknown = short.MaxValue;

        // Main menu items (ids from 1 to 99)
        public const short Inquiries = 2;
        public const short Beneficiaries = 3;
        public const short Distributions = 4;
        public const short Reconciliation = 5;
        public const short YearEnd = 6;
        public const short ItDevOps = 7;
        public const short Administrative = 8;

        // Secondary drawer top-level menus
        public const short FiscalClose = 9;
        public const short DecemberActivities = 10;

        // Third-level menus
        public const short CleanupReports = 11;
        public const short ProfShareReportByAge = 12;
        public const short ProfShareByStore = 13;

        // Groups under INQUIRIES
        public const short InquiriesGroup = 14;
        public const short AdjustmentsGroup = 15;
        public const short AdhocGroup = 16;

        // Available pages (ids starting at 100)
        public const short MasterInquiry = 100;
        public const short Adjustments = 101;
        public const short DemographicFreeze = 102;
        public const short DistributionInquiry = 103;
        public const short ManageExecutiveHours = 104;
        public const short YTDWagesExtract = 105;
        public const short Forfeitures = 106;
        public const short DistributionsAndForfeitures = 107;
        public const short ProfitShareReport = 108;
        public const short GetEligibleEmployees = 109;
        public const short ProfitShareForfeit = 110;
        public const short MasterUpdate = 111;
        public const short ProfitMasterUpdate = 112;
        public const short PaymasterUpdate = 113;
        public const short ProfControlSheet = 114;
        public const short DistributionsByAge = 115;
        public const short ContributionsByAge = 116;
        public const short ForfeituresByAge = 117;
        public const short BalanceByAge = 118;
        public const short VestedAmountsByAge = 119;
        public const short BalanceByYears = 120;
        public const short ProfShareGrossRpt = 121;
        public const short Under21Report = 122;
        public const short QPAY066Under21 = 123;
        public const short QPAY066TAUnder21 = 124;
        public const short QPAY066B = 125;
        public const short QPAY066TA = 126;
        public const short QNEWPROFLBL = 127;
        public const short PROFNEW = 128;
        public const short PROFALL = 129;
        public const short ReprintCertificates = 130;
        public const short SaveProfPaymstr = 131;
        public const short QPAY066AdHocReports = 132;
        public const short RecentlyTerminated = 133;
        public const short PayBeneficiaryReport = 134;
        public const short AdhocBeneficiariesReport = 135;
        public const short TerminatedLetters = 136;
        public const short PAY426N = 138;
        public const short ProfitSummary = 139;
        public const short PAY426_2 = 140;
        public const short PAY426_3 = 141;
        public const short DemographicBadgesNotInPayProfit = 142;
        public const short DuplicateSSNsInDemographics = 143;
        public const short NegativeETVA = 144;
        public const short Terminations = 145;
        public const short DuplicateNamesAndBirthdays = 146;
        public const short MilitaryContributions = 147;
        public const short Unforfeit = 148;

        public const short ProfitShareReportFinalRun = 149;
        public const short PrintProfitCerts = 150;

        public const short ProfitShareReportEditRun = 151;

        public const short PayBenReport = 152;
        public const short CheckRun = 153;
        public const short ProfitDetailReversals = 154;
        public const short ViewDistribution = 156;
        public const short AddDistribution = 157;

        public const short DistributionEditRunReport = 160;
        public const short AccountHistoryReport = 161;
        public const short AuditSearch = 166;
        public const short OracleHcmDiagnostics = 167;
        public const short ManageStateTaxRates = 173;
        public const short ManageAnnuityRates = 174;
        public const short ProfitSharingAdjustments = 175;

        public const short AdhocProfLetter73 = 176;
        public const short ManageCommentTypes = 177;
        public const short ManageRmdFactors = 178;
        public const short FakeTimeManagement = 179;
        public const short ManageBanks = 180;
    }

    public short Id { get; set; }
    public short? ParentId { get; set; }
    public required string Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Url { get; set; }
    public byte? StatusId { get; set; }
    public byte OrderNumber { get; set; }
    public bool? Disabled { get; set; }
    // When false the page exists in the system but should not be shown in navigation menus/drawers.
    // Backed by database column IS_NAVIGABLE (NUMBER(1)).
    public bool? IsNavigable { get; set; }
    public string? Icon { get; set; }

    public List<NavigationRole>? RequiredRoles { get; set; } = [];
    public NavigationStatus? NavigationStatus { get; set; }
    public List<Navigation>? Items { get; set; }
    public Navigation? Parent { get; set; }
    public List<NavigationTracking>? NavigationTrackings { get; set; }
    public List<NavigationCustomSetting>? CustomSettings { get; set; }

    // Self-referencing many-to-many to represent prerequisite navigation elements.
    // PrerequisiteNavigations: the items this navigation depends on.
    // DependentNavigations: the items that depend on this navigation (inverse).
    public List<Navigation>? PrerequisiteNavigations { get; set; }
    public List<Navigation>? DependentNavigations { get; set; }
}


