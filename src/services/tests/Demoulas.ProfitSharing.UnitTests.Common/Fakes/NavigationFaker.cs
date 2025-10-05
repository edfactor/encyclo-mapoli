using System.Linq;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

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
            new Navigation { Id = 2, ParentId = null, Title = "INQUIRIES", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesInquiries), NavigationStatus = notStarted },
            new Navigation { Id = 3, ParentId = null, Title = "BENEFICIARIES", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = true, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesBeneficiaries), NavigationStatus = notStarted },
            new Navigation { Id = 4, ParentId = null, Title = "DISTRIBUTIONS", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = true, IsNavigable = true, RequiredRoles = new List<NavigationRole>(), NavigationStatus = notStarted },
            new Navigation { Id = 5, ParentId = null, Title = "RECONCILIATION", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = true, IsNavigable = true, RequiredRoles = new List<NavigationRole>(), NavigationStatus = notStarted },
            new Navigation { Id = 6, ParentId = null, Title = "YEAR END", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 7, ParentId = null, Title = "IT DEVOPS", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesItDevOpsTop), NavigationStatus = notStarted },

            // Sub values for INQUIRIES
            new Navigation { Id = 100, ParentId = 2, Title = "MASTER INQUIRY", SubTitle = "", Url = "master-inquiry", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesMasterInquiry), NavigationStatus = notStarted },
            new Navigation { Id = 101, ParentId = 2, Title = "ADJUSTMENTS", SubTitle = "", Url = "adjustments", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesInquiries), NavigationStatus = notStarted },

            // Distribution items
            new Navigation { Id = 103, ParentId = 4, Title = "DISTRIBUTIONS INQUIRY", SubTitle = "", Url = "distributions-inquiry", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(), NavigationStatus = notStarted },

            // IT Operations
            new Navigation { Id = 102, ParentId = 7, Title = "Demographic Freeze", SubTitle = "", Url = "demographic-freeze", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesFreeze), NavigationStatus = notStarted },

            // December Activities
            new Navigation { Id = 9, ParentId = 6, Title = "December Activities", SubTitle = "", Url = "december-process-accordion", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 10, ParentId = 9, Title = "Clean up Reports", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },

            // Sub values for Clean up Reports
            new Navigation { Id = 142, ParentId = 10, Title = "Demographic Badges Not In PayProfit", SubTitle = "", Url = "demographic-badges-not-in-payprofit", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 143, ParentId = 10, Title = "Duplicate SSNs in Demographics", SubTitle = "", Url = "duplicate-ssns-demographics", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 144, ParentId = 10, Title = "Negative ETVA", SubTitle = "", Url = "negative-etva-for-ssns-on-payprofit", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 146, ParentId = 10, Title = "Duplicate Names and Birthdays", SubTitle = "", Url = "duplicate-names-and-birthdays", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },

            // Sub values for December Activities
            new Navigation { Id = 147, ParentId = 9, Title = "Military Contributions", SubTitle = "008-13", Url = "military-contribution", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 148, ParentId = 9, Title = "Unforfeit", SubTitle = "QPREV-PROF", Url = "unforfeitures", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 145, ParentId = 9, Title = "Terminations", SubTitle = "QPAY066", Url = "prof-term", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 106, ParentId = 9, Title = "Forfeitures", SubTitle = "008-12", Url = "forfeitures-adjustment", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 107, ParentId = 9, Title = "Distributions and Forfeitures", SubTitle = "QPAY129", Url = "distributions-and-forfeitures", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 108, ParentId = 9, Title = "Profit Share Report", SubTitle = "PAY426", Url = "profit-share-report", StatusId = 1, OrderNumber = 9, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },

            // Profit Share Totals (Year End)
            new Navigation { Id = 8, ParentId = 6, Title = "Fiscal Close", SubTitle = "", Url = "fiscal-close", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },

            // Fiscal Close menu items
            new Navigation { Id = 104, ParentId = 8, Title = "Manage Executive Hours", SubTitle = "PROF-DOLLAR-EXEC-EXTRACT, TPR008-09", Url = "manage-executive-hours-and-dollars", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 105, ParentId = 8, Title = "YTD Wages Extract", SubTitle = "PROF-DOLLAR-EXTRACT", Url = "ytd-wages-extract", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 132, ParentId = 8, Title = "QPAY066* Ad Hoc Reports", SubTitle = "QPAY066*", Url = "qpay066-adhoc", StatusId = 1, OrderNumber = 17, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 133, ParentId = 8, Title = "Recently Terminated", SubTitle = "PROF-VESTED|PAY508", Url = "recently-terminated", StatusId = 1, OrderNumber = 18, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 136, ParentId = 8, Title = "Terminated Letters", SubTitle = "QPROF003-1", Url = "terminated-letters", StatusId = 1, OrderNumber = 19, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 109, ParentId = 8, Title = "Get Eligible Employees", SubTitle = "GET-ELIGIBLE-EMPS", Url = "eligible-employees", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 110, ParentId = 8, Title = "Profit Share Forfeit", SubTitle = "PAY443", Url = "forfeit", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 111, ParentId = 8, Title = "Master Update", SubTitle = "PAY444|PAY447", Url = "profit-share-update", StatusId = 1, OrderNumber = 7, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 112, ParentId = 8, Title = "Profit Master Update", SubTitle = "PAY460, PROFTLD", Url = "profit-master-update", StatusId = 1, OrderNumber = 8, Icon = "", Disabled = true, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 113, ParentId = 8, Title = "Paymaster Update", SubTitle = "PAY450", Url = "pay450-summary", StatusId = 1, OrderNumber = 9, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 114, ParentId = 8, Title = "Prof Control Sheet", SubTitle = "PROF-CNTRL-SHEET", Url = "prof-control-sheet", StatusId = 1, OrderNumber = 11, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 11, ParentId = 8, Title = "Prof Share Report By Age", SubTitle = "Prof130", Url = "", StatusId = 1, OrderNumber = 12, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 121, ParentId = 8, Title = "Prof Share Gross Rpt", SubTitle = "QPAY501", Url = "profit-share-gross-report", StatusId = 1, OrderNumber = 13, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 12, ParentId = 8, Title = "Prof Share by Store", SubTitle = "QPAY066TA", Url = "profit-share-by-store", StatusId = 1, OrderNumber = 14, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 130, ParentId = 8, Title = "Reprint Certificates / Print Profit Certs", SubTitle = "PAYCERT", Url = "reprint-certificates", StatusId = 1, OrderNumber = 15, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 131, ParentId = 8, Title = "Save Prof Paymstr", SubTitle = "", Url = "save-prof-paymstr", StatusId = 1, OrderNumber = 16, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },

            new Navigation { Id = 149, ParentId = 8, Title = "Profit Share Report Final Run", SubTitle = "", Url = "profit-share-report-final-run", StatusId = 1, OrderNumber = 20, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },



            // Sub values for Report by Age
            new Navigation { Id = 115, ParentId = 11, Title = "DISTRIBUTIONS BY AGE", SubTitle = "PROF130", Url = "distributions-by-age", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 116, ParentId = 11, Title = "CONTRIBUTIONS BY AGE", SubTitle = "PROF130", Url = "contributions-by-age", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 117, ParentId = 11, Title = "FORFEITURES BY AGE", SubTitle = "PROF130", Url = "forfeitures-by-age", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 118, ParentId = 11, Title = "BALANCE BY AGE", SubTitle = "PROF130B", Url = "balance-by-age", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 119, ParentId = 11, Title = "VESTED AMOUNTS BY AGE", SubTitle = "PROF130V", Url = "vested-amounts-by-age", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 120, ParentId = 11, Title = "BALANCE BY YEARS", SubTitle = "PROF130Y", Url = "balance-by-years", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },

            // Sub values of Profit Share by Store
            new Navigation { Id = 122, ParentId = 12, Title = "Under-21 Report", SubTitle = "", Url = "under-21-report", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 123, ParentId = 122, Title = "QPAY066-UNDR21", SubTitle = "", Url = "qpay066-under21", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 124, ParentId = 122, Title = "QPAY066TA-UNDR21", SubTitle = "", Url = "qpay066ta-under21", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 125, ParentId = 12, Title = "QPAY066B", SubTitle = "", Url = "qpay066b", StatusId = 1, OrderNumber = 7, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 126, ParentId = 12, Title = "QPAY066TA", SubTitle = "", Url = "qpay066ta", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 127, ParentId = 12, Title = "QNEWPROFLBL", SubTitle = "", Url = "new-ps-labels", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 128, ParentId = 12, Title = "PROFNEW", SubTitle = "", Url = "profnew", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
            new Navigation { Id = 129, ParentId = 12, Title = "PROFALL", SubTitle = "", Url = "profall", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false, IsNavigable = true, RequiredRoles = new List<NavigationRole>(rolesYearEnd), NavigationStatus = notStarted },
        };

        // Establish a simple prerequisite relationship for testing:
        // Make "Master Update" (Id=111) depend on "Manage Executive Hours" (Id=104) being complete.
        var manageExecHours = list.FirstOrDefault(n => n.Id == 104);
        var masterUpdate = list.FirstOrDefault(n => n.Id == 111);
        if (manageExecHours != null)
        {
            manageExecHours.StatusId = NavigationStatus.Constants.Complete;
            manageExecHours.NavigationStatus = complete;
        }
        if (masterUpdate != null && manageExecHours != null)
        {
            masterUpdate.PrerequisiteNavigations = new List<Navigation> { manageExecHours };
        }

        return list;
    }

    public List<NavigationRole> GetAllNavigationRoles()
    {
        // Create all the navigation roles used in the system with their IsReadOnly flags
        return new List<NavigationRole>
        {
            new NavigationRole { Id = 1, Name = "System-Administrator", IsReadOnly = false },
            new NavigationRole { Id = 2, Name = "Finance-Manager", IsReadOnly = false },
            new NavigationRole { Id = 3, Name = "Distributions-Clerk", IsReadOnly = false },
            new NavigationRole { Id = 4, Name = "Hardship-Administrator", IsReadOnly = false },
            new NavigationRole { Id = 6, Name = "IT-DevOps", IsReadOnly = true },
            new NavigationRole { Id = 9, Name = "Auditor", IsReadOnly = true }
        };
    }
}
