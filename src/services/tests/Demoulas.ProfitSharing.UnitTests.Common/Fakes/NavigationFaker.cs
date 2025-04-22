using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Newtonsoft.Json.Linq;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
public class NavigationFaker
{
    public List<Navigation> DummyNavigationData()
    {
        return new List<Navigation>()
        {
            new Navigation(){ Id=49, ParentId =null, Title= "INQUIRIES", SubTitle=null, Url = null, StatusId = 1, OrderNumber = 10, Icon = null },
            new Navigation(){ Id=50, ParentId = 49, Title= "MASTER INQUIRY", SubTitle=null, Url = "master-inquiry", StatusId = 1, OrderNumber = 1, Icon = null },
            new Navigation(){ Id=51, ParentId =null, Title= "BENEFICIARIES", SubTitle=null, Url = null, StatusId = 1, OrderNumber = 10, Icon = null },
            new Navigation(){ Id=52, ParentId =null, Title= "DISTRIBUTIONS", SubTitle=null, Url = null, StatusId = 1, OrderNumber = 3, Icon = null },
            new Navigation(){ Id=53, ParentId =null, Title= "RECONCILIATION", SubTitle=null, Url = null, StatusId = 1, OrderNumber = 4, Icon = null },
            new Navigation(){ Id=54, ParentId =null, Title= "YEAR END", SubTitle=null, Url = null, StatusId = 1, OrderNumber = 5, Icon = null },
            new Navigation(){ Id=1, ParentId =54, Title= "December Activities", SubTitle=null, Url = null, StatusId = 1, OrderNumber = 1, Icon = null },
            new Navigation(){ Id=2, ParentId =1, Title= "Clean up Reports", SubTitle=null, Url = null, StatusId = 1, OrderNumber = 1, Icon = null },
            new Navigation(){ Id=3, ParentId =2, Title= "Demographic Badges Not In PayProfit", SubTitle=null, Url = "demographic-badges-not-in-payprofit", StatusId = 1, OrderNumber = 1, Icon = null },
            new Navigation(){ Id=4, ParentId =2, Title= "Duplicate SSNs in Demographics", SubTitle=null, Url = "duplicate-ssns-demographics", StatusId = 1, OrderNumber = 2, Icon = null },
            new Navigation(){ Id=5, ParentId =2, Title= "Negative ETVA", SubTitle=null, Url = "negative-etva-for-ssns-on-payprofit", StatusId = 1, OrderNumber = 3, Icon = null },
            new Navigation(){ Id=6, ParentId =2, Title= "Duplicate Names and Birthdays", SubTitle=null, Url = "duplicate-names-and-birthdays", StatusId = 1, OrderNumber = 4, Icon = null },
            new Navigation(){ Id=7, ParentId =1, Title= "Military Contributions", SubTitle="TPR008-13", Url = "military-entry-and-modification", StatusId = 1, OrderNumber = 2, Icon = null },
            new Navigation(){ Id=8, ParentId =1, Title= "Rehire Forfeitures", SubTitle="QPREV-PROF", Url = "rehire-forfeitures", StatusId = 1, OrderNumber = 3, Icon = null },
            new Navigation(){ Id=10, ParentId =1, Title= "Distributions and Forfeitures", SubTitle="QPAY129", Url = "distributions-and-forfeitures", StatusId = 1, OrderNumber = 6, Icon = null },
            new Navigation(){ Id=12, ParentId =1, Title= "Terminations", SubTitle="QPAY066", Url = "prof-term", StatusId = 1, OrderNumber = 8, Icon = null },
            new Navigation(){ Id=13, ParentId =1, Title= "Profit Share Report", SubTitle="PAY426", Url = "profit-share-report", StatusId = 1, OrderNumber = 9, Icon = null },
            new Navigation(){ Id=14, ParentId =54, Title= "Fiscal Close", SubTitle=null, Url = null, StatusId = 1, OrderNumber = 2, Icon = null },
            new Navigation(){ Id=15, ParentId =14, Title= "Manage Executive Hours and Dollars", SubTitle="TPR008-09", Url = "manage-executive-hours-and-dollars", StatusId = 1, OrderNumber = 1, Icon = null },
            new Navigation(){ Id=16, ParentId =14, Title= "YTD Wages Extract", SubTitle="PROF-DOLLAR-EXTRACT", Url = "ytd-wages-extract", StatusId = 1, OrderNumber = 2, Icon = null },
            new Navigation(){ Id=17, ParentId =14, Title= "Profit Share Report", SubTitle="PAY426", Url = "profit-share-report", StatusId = 1, OrderNumber = 3, Icon = null },
            new Navigation(){ Id=18, ParentId =14, Title= "Profit Share Report Edit Run", SubTitle="PAY426N", Url = null, StatusId = 1, OrderNumber = 4, Icon = null },
            new Navigation(){ Id=19, ParentId =18, Title= "Active/inactive employees age 18 - 20 with 1000 hours or more", SubTitle=null, Url = "pay426-1", StatusId = 1, OrderNumber = 1, Icon = null },
            new Navigation(){ Id=21, ParentId =18, Title= "Active/inactive employees age 21 & with 1000 hours or more", SubTitle=null, Url = "pay426-2", StatusId = 1, OrderNumber = 2, Icon = null },
            new Navigation(){ Id=22, ParentId =18, Title= "Active/inactive employees under 18", SubTitle=null, Url = "pay426-3", StatusId = 1, OrderNumber = 3, Icon = null },
            new Navigation(){ Id=23, ParentId =18, Title= "Active/inactive employees 18 and older with prior profit sharing amounts and <1000 hours", SubTitle=null, Url = "pay426-4", StatusId = 1, OrderNumber = 4, Icon = null },
            new Navigation(){ Id=24, ParentId =18, Title= "Active/inactive employees 18 and older with no prior profit sharing amounts and <1000 hours", SubTitle=null, Url = "pay426-5", StatusId = 1, OrderNumber = 5, Icon = null },
            new Navigation(){ Id=25, ParentId =18, Title= "Terminated employees 18 and older with 1000 hours or more", SubTitle=null, Url = "pay426-6", StatusId = 1, OrderNumber = 6, Icon = null },
            new Navigation(){ Id=26, ParentId =18, Title= "Terminated employees 18 and older with no prior profit sharing amounts and < 1000 hours", SubTitle=null, Url = "pay426-7", StatusId = 1, OrderNumber = 7, Icon = null },
            new Navigation(){ Id=27, ParentId =18, Title= "Terminated employees 18 and older with prior profit sharing amounts and < 1000 hours", SubTitle=null, Url = "pay426-8", StatusId = 1, OrderNumber = 8, Icon = null },
            new Navigation(){ Id=28, ParentId =18, Title= "Profit sharing summary page", SubTitle=null, Url = "pay426-9", StatusId = 1, OrderNumber = 9, Icon = null },
            new Navigation(){ Id=29, ParentId =18, Title= "All non-employee beneficiaries", SubTitle=null, Url = "pay426-10", StatusId = 1, OrderNumber = 10, Icon = null },
            new Navigation(){ Id=30, ParentId =14, Title= "Get Eligible Employees", SubTitle="GET-ELIGIBLE-EMPS", Url = "eligible-employees", StatusId = 1, OrderNumber = 5, Icon = null },
            new Navigation(){ Id=31, ParentId =14, Title= "Forfeit", SubTitle="PAY443", Url = "forfeit", StatusId = 1, OrderNumber = 6, Icon = null },
            new Navigation(){ Id=32, ParentId =14, Title= "Profit Share Updates", SubTitle="PAY444|PAY447", Url = "profit-share-update", StatusId = 1, OrderNumber = 7, Icon = null },
            new Navigation(){ Id=33, ParentId =14, Title= "Reports by Age", SubTitle=null, Url = null, StatusId = 1, OrderNumber = 8, Icon = null },
            new Navigation(){ Id=34, ParentId =33, Title= "Get Contributions by Age", SubTitle="PROF130", Url = "contributions-by-age", StatusId = 1, OrderNumber = 1, Icon = null },
            new Navigation(){ Id=35, ParentId =33, Title= "Get Distributions by Age", SubTitle="PROF130", Url = "distributions-by-age", StatusId = 1, OrderNumber = 2, Icon = null },
            new Navigation(){ Id=36, ParentId =33, Title= "Get Forfeiture by Age", SubTitle="PROF130", Url = "forfeitures-by-age", StatusId = 1, OrderNumber = 3, Icon = null },
            new Navigation(){ Id=37, ParentId =33, Title= "Get Balance by Age", SubTitle="PROF130B", Url = "balance-by-age", StatusId = 1, OrderNumber = 4, Icon = null },
            new Navigation(){ Id=38, ParentId =33, Title= "Get Vested Amounts by Age", SubTitle="PROF130V", Url = "vested-amounts-by-age", StatusId = 1, OrderNumber = 5, Icon = null },
            new Navigation(){ Id=39, ParentId =33, Title= "Get Balance by Years", SubTitle="PROF130Y", Url = "balance-by-years", StatusId = 1, OrderNumber = 6, Icon = null },
            new Navigation(){ Id=40, ParentId =14, Title= "Profit Share Gross Report", SubTitle="QPAY501", Url = "profit-share-gross-report", StatusId = 1, OrderNumber = 9, Icon = null },
            new Navigation(){ Id=41, ParentId =14, Title= "Profit Share by Store", SubTitle="QPAY066TA", Url = null, StatusId = 1, OrderNumber = 9, Icon = null },
            new Navigation(){ Id=42, ParentId =41, Title= "QPAY066-UNDR21", SubTitle=null, Url = "qpay066-under21", StatusId = 1, OrderNumber = 1, Icon = null },
            new Navigation(){ Id=43, ParentId =41, Title= "QPAY066TA-UNDR21", SubTitle=null, Url = "qpay066ta-under21", StatusId = 1, OrderNumber = 2, Icon = null },
            new Navigation(){ Id=44, ParentId =41, Title= "QPAY066TA", SubTitle=null, Url = "qpay066ta", StatusId = 1, OrderNumber = 3, Icon = null },
            new Navigation(){ Id=45, ParentId =41, Title= "PROFALL Report", SubTitle=null, Url = "profall", StatusId = 1, OrderNumber = 4, Icon = null },
            new Navigation(){ Id=46, ParentId =41, Title= "New Labels", SubTitle="QNEWPROFLBL", Url = "new-ps-labels", StatusId = 1, OrderNumber = 5, Icon = null },
            new Navigation(){ Id=47, ParentId =41, Title= "PROFNEW", SubTitle=null, Url = null, StatusId = 1, OrderNumber = 6, Icon = null },
            new Navigation(){ Id=48, ParentId =14, Title= "Print Profit Certs", SubTitle="PAYCERT", Url = null, StatusId = 1, OrderNumber = 10, Icon = null }
        };
    }
}
