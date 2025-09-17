namespace Demoulas.ProfitSharing.Data.Entities.Navigations;
public static class Navigation
{
    public static class Constants
    {
        public const short Unknown = short.MaxValue;
        
        // Main menu items
        public const short Inquiries = 50;
        public const short Beneficiaries = 52;
        public const short Distributions = 53;
        public const short Reconciliation = 54;
        public const short YearEnd = 55;
        public const short ItDevOps = 56;
        
        // Inquiries sub-items
        public const short MasterInquiry = 51;
        
        // IT Operations sub-items
        public const short DemographicFreeze = 57;
        
        // Year End sub-items
        public const short DecemberActivities = 1;
        public const short CleanupReports = 2;
        public const short FiscalClose = 14;
        
        // Clean up Reports sub-items
        public const short DemographicBadgesNotInPayProfit = 3;
        public const short DuplicateSSNsInDemographics = 4;
        public const short NegativeETVA = 5;
        public const short DuplicateNamesAndBirthdays = 6;
        
        // December Activities sub-items
        public const short MilitaryContributions = 7;
        public const short Unforfeit = 8;
        public const short Terminations = 9;
        public const short Forfeitures = 10;
        public const short DistributionsAndForfeitures = 11;
        public const short ProfitShareReport = 13;
        
        // Fiscal Close sub-items
        public const short ManageExecutiveHours = 15;
        public const short YTDWagesExtract = 16;
        public const short ProfitShareReportFinalRun = 17;
        public const short ProfitShareReportEditRun = 18;
        public const short GetEligibleEmployees = 30;
        public const short ProfitShareForfeit = 31;
        public const short ProfPayMasterUpdate = 33;
        public const short ProfShareReportByAge = 34;
        public const short ProfShareGrossRpt = 41;
        public const short ProfShareByStore = 42;
        public const short PrintProfitCerts = 49;
        public const short MasterUpdate = 60;
        public const short ProfitMasterUpdate = 62;
        public const short SaveProfPaymstr = 63;
        public const short ProfControlSheet = 64;
        public const short QPAY066AdHocReports = 65;
        
        // Prof Share Report By Age sub-items
        public const short ContributionsByAge = 35;
        public const short DistributionsByAge = 36;
        public const short ForfeituresByAge = 37;
        public const short BalanceByAge = 38;
        public const short VestedAmountsByAge = 39;
        public const short BalanceByYears = 40;
        
        // Prof Share by Store sub-items
        public const short QPAY066Under21 = 43;
        public const short QPAY066TAUnder21 = 44;
        public const short QPAY066TA = 45;
        public const short PROFALL = 46;
        public const short QNEWPROFLBL = 47;
        public const short PROFNEW = 48;
    }


}
