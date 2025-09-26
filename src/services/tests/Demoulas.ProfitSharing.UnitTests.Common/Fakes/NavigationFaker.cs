using Demoulas.ProfitSharing.Data.Entities.Navigations;
using System.Linq;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
public class NavigationFaker
{
    public List<Navigation> DummyNavigationData()
    {
    // Common statuses
    var notStarted = new NavigationStatus { Id = NavigationStatus.Constants.NotStarted, Name = "Not Started" };
    var complete = new NavigationStatus { Id = NavigationStatus.Constants.Complete, Name = "Complete" };

        // Common role sets (Ids follow NavigationRole.Contants; Names mirror Security.Role strings)
        var rolesInquiries = new List<NavigationRole>
        {
            new NavigationRole { Id = 1, Name = "System-Administrator", IsReadOnly = false },
            new NavigationRole { Id = 2, Name = "Finance-Manager", IsReadOnly = false },
            new NavigationRole { Id = 3, Name = "Distributions-Clerk", IsReadOnly = false },
            new NavigationRole { Id = 4, Name = "Hardship-Administrator", IsReadOnly = false },
            new NavigationRole { Id = 6, Name = "IT-DevOps", IsReadOnly = true }
        };

        var rolesMasterInquiry = new List<NavigationRole>(rolesInquiries);

        var rolesBeneficiaries = new List<NavigationRole>
        {
            new NavigationRole { Id = 1, Name = "System-Administrator", IsReadOnly = false },
            new NavigationRole { Id = 2, Name = "Finance-Manager", IsReadOnly = false },
            new NavigationRole { Id = 3, Name = "Distributions-Clerk", IsReadOnly = false },
            new NavigationRole { Id = 6, Name = "IT-DevOps", IsReadOnly = true }
        };

        var rolesYearEnd = new List<NavigationRole>
        {
            new NavigationRole { Id = 1, Name = "System-Administrator", IsReadOnly = false },
            new NavigationRole { Id = 2, Name = "Finance-Manager", IsReadOnly = false },
            new NavigationRole { Id = 4, Name = "Hardship-Administrator", IsReadOnly = false },
            new NavigationRole { Id = 6, Name = "IT-DevOps", IsReadOnly = true }
        };

        var rolesFreeze = new List<NavigationRole>
        {
            new NavigationRole { Id = 6, Name = "IT-DevOps", IsReadOnly = true }
        };

        var rolesItDevOpsTop = new List<NavigationRole>(rolesFreeze);

    var list = new List<Navigation>()
    {
            // Top-level menus
            new Navigation { Id = 50, ParentId = null, Title = "INQUIRIES", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesInquiries), NavigationStatus = notStarted },
            new Navigation { Id = 52, ParentId = null, Title = "BENEFICIARIES", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = true, RequiredRoles = new List<NavigationRole>(rolesBeneficiaries), NavigationStatus = notStarted },
            new Navigation { Id = 53, ParentId = null, Title = "DISTRIBUTIONS", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = true, RequiredRoles = new List<NavigationRole>(), NavigationStatus = notStarted },
            new Navigation { Id = 54, ParentId = null, Title = "RECONCILIATION", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = true, RequiredRoles = new List<NavigationRole>(), NavigationStatus = notStarted },
            new Navigation { Id = 55, ParentId = null, Title = "YEAR END", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 56, ParentId = null, Title = "IT DEVOPS", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesItDevOpsTop), NavigationStatus = notStarted },

            // Inquiries
            new Navigation { Id = 51, ParentId = 50, Title = "MASTER INQUIRY", SubTitle = "", Url = "master-inquiry", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesMasterInquiry), NavigationStatus = notStarted },

            // IT DevOps
            new Navigation { Id = 57, ParentId = 56, Title = "Demographic Freeze", SubTitle = "", Url = "demographic-freeze", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesFreeze), NavigationStatus = notStarted },

            // Year End -> December Activities
            new Navigation { Id = 1, ParentId = 55, Title = "December Activities", SubTitle = "", Url = "december-process-accordion", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 2, ParentId = 1, Title = "Clean up Reports", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 3, ParentId = 2, Title = "Demographic Badges Not In PayProfit", SubTitle = "", Url = "demographic-badges-not-in-payprofit", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 4, ParentId = 2, Title = "Duplicate SSNs in Demographics", SubTitle = "", Url = "duplicate-ssns-demographics", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 5, ParentId = 2, Title = "Negative ETVA", SubTitle = "", Url = "negative-etva-for-ssns-on-payprofit", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 6, ParentId = 2, Title = "Duplicate Names and Birthdays", SubTitle = "", Url = "duplicate-names-and-birthdays", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 7, ParentId = 1, Title = "Military Contributions", SubTitle = "008-13", Url = "military-entry-and-modification", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 8, ParentId = 1, Title = "Unforfeit", SubTitle = "QPREV-PROF", Url = "unforfeitures", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 9, ParentId = 1, Title = "Terminations", SubTitle = "QPAY066", Url = "prof-term", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 10, ParentId = 1, Title = "Forfeitures", SubTitle = "008-12", Url = "forfeitures-adjustment", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 11, ParentId = 1, Title = "Distributions and Forfeitures", SubTitle = "QPAY129", Url = "distributions-and-forfeitures", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 13, ParentId = 1, Title = "Profit Share Report", SubTitle = "PAY426", Url = "profit-share-report", StatusId = 1, OrderNumber = 9, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },

            // Year End -> Fiscal Close and children
            new Navigation { Id = 14, ParentId = 55, Title = "Fiscal Close", SubTitle = "", Url = "fiscal-close", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 15, ParentId = 14, Title = "Manage Executive Hours", SubTitle = "PROF-DOLLAR-EXEC-EXTRACT, TPR008-09", Url = "manage-executive-hours-and-dollars", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 16, ParentId = 14, Title = "YTD Wages Extract", SubTitle = "PROF-DOLLAR-EXTRACT", Url = "ytd-wages-extract", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 18, ParentId = 14, Title = "Profit Share Report (Edit Run)", SubTitle = "PAY426", Url = "pay426n", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 17, ParentId = 14, Title = "Profit Share Report (Final Run)", SubTitle = "PAY426", Url = "profit-share-report", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 65, ParentId = 14, Title = "QPAY066* Ad Hoc Reports", SubTitle = "QPAY066*", Url = "qpay066-adhoc", StatusId = 1, OrderNumber = 17, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 66, ParentId = 14, Title = "IT Operations", SubTitle = "Operations", Url = "", StatusId = 1, OrderNumber = 20, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 30, ParentId = 14, Title = "Get Eligible Employees", SubTitle = "GET-ELIGIBLE-EMPS", Url = "eligible-employees", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 31, ParentId = 14, Title = "Profit Share Forfeit", SubTitle = "PAY443", Url = "forfeit", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 60, ParentId = 14, Title = "Master Update", SubTitle = "PAY444|PAY447", Url = "profit-share-update", StatusId = 1, OrderNumber = 7, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 62, ParentId = 14, Title = "Profit Master Update", SubTitle = "PAY460, PROFTLD", Url = "profit-master-update", StatusId = 1, OrderNumber = 8, Icon = "", Disabled = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 33, ParentId = 14, Title = "Prof PayMaster Update", SubTitle = "PAY450", Url = "pay450-summary", StatusId = 1, OrderNumber = 10, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 64, ParentId = 14, Title = "Prof Control Sheet", SubTitle = "PROF-CNTRL-SHEET", Url = "prof-control-sheet", StatusId = 1, OrderNumber = 11, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 34, ParentId = 14, Title = "Prof Share Report By Age", SubTitle = "Prof130", Url = "", StatusId = 1, OrderNumber = 12, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 41, ParentId = 14, Title = "Prof Share Gross Rpt", SubTitle = "QPAY501", Url = "profit-share-gross-report", StatusId = 1, OrderNumber = 13, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 42, ParentId = 14, Title = "Prof Share by Store", SubTitle = "QPAY066TA", Url = "", StatusId = 1, OrderNumber = 14, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 49, ParentId = 14, Title = "Print Profit Certs", SubTitle = "PAYCERT", Url = "print-profit-certs", StatusId = 1, OrderNumber = 15, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 63, ParentId = 14, Title = "Save Prof Paymstr", SubTitle = "", Url = "save-prof-paymstr", StatusId = 1, OrderNumber = 16, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },

            // Prof Share Report By Age children
            new Navigation { Id = 36, ParentId = 34, Title = "DISTRIBUTIONS BY AGE", SubTitle = "PROF130", Url = "distributions-by-age", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 35, ParentId = 34, Title = "CONTRIBUTIONS BY AGE", SubTitle = "PROF130", Url = "contributions-by-age", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 37, ParentId = 34, Title = "FORFEITURES BY AGE", SubTitle = "PROF130", Url = "forfeitures-by-age", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 38, ParentId = 34, Title = "BALANCE BY AGE", SubTitle = "PROF130B", Url = "balance-by-age", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 39, ParentId = 34, Title = "VESTED AMOUNTS BY AGE", SubTitle = "PROF130V", Url = "vested-amounts-by-age", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 40, ParentId = 34, Title = "BALANCE BY YEARS", SubTitle = "PROF130Y", Url = "balance-by-years", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },

            // Prof Share by Store children
            new Navigation { Id = 43, ParentId = 42, Title = "QPAY066-UNDR21", SubTitle = "", Url = "qpay066-under21", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 44, ParentId = 42, Title = "QPAY066TA-UNDR21", SubTitle = "", Url = "qpay066ta-under21", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 45, ParentId = 42, Title = "QPAY066TA", SubTitle = "", Url = "qpay066ta", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 47, ParentId = 42, Title = "QNEWPROFLBL", SubTitle = "", Url = "new-ps-labels", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 48, ParentId = 42, Title = "PROFNEW", SubTitle = "", Url = "profnew", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 46, ParentId = 42, Title = "PROFALL", SubTitle = "", Url = "profall", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
        };

        // Establish a simple prerequisite relationship for testing:
        // Make "Profit Share Report (Final Run)" (Id=17) depend on
        // "Profit Share Report (Edit Run)" (Id=18) being complete.
        var editRun = list.FirstOrDefault(n => n.Id == 18);
        var finalRun = list.FirstOrDefault(n => n.Id == 17);
        if (editRun != null)
        {
            editRun.StatusId = NavigationStatus.Constants.Complete;
            editRun.NavigationStatus = complete;
        }
        if (finalRun != null && editRun != null)
        {
            finalRun.PrerequisiteNavigations = new List<Navigation> { editRun };
        }

        return list;
    }
}
