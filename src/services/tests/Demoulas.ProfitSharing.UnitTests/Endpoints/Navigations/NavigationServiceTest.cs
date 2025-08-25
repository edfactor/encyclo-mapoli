using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Services.Navigations;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Navigations;

public class NavigationServiceTests : ApiTestBase<Program>
{
    private readonly INavigationService _navigationService;
    private readonly List<Navigation> _navigationListObj;
    private readonly List<NavigationStatusDto> _navigationStatusList;

    public NavigationServiceTests()
    {
        _navigationService = ServiceProvider?.GetRequiredService<INavigationService>()!;
        _navigationListObj = new List<Navigation>()
        {
            new Navigation { Id = 12, ParentId = 1, Title = "Terminations", SubTitle = "QPAY066", Url = "prof-term", StatusId = 1, OrderNumber = 8, Icon = "", Disabled = false },
            new Navigation { Id = 13, ParentId = 1, Title = "Profit Share Report", SubTitle = "PAY426", Url = "profit-share-report", StatusId = 1, OrderNumber = 9, Icon = "", Disabled = false },
            new Navigation { Id = 14, ParentId = 54, Title = "Fiscal Close", SubTitle = "", Url = "fiscal-close", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false },
            new Navigation { Id = 15, ParentId = 14, Title = "Manage Executive Hours and Dollars", SubTitle = "TPR008-09", Url = "manage-executive-hours-and-dollars", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false },
            new Navigation { Id = 16, ParentId = 14, Title = "YTD Wages Extract", SubTitle = "PROF-DOLLAR-EXTRACT", Url = "ytd-wages-extract", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false },
            new Navigation { Id = 17, ParentId = 14, Title = "Profit Share Report", SubTitle = "PAY426", Url = "profit-share-report", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false },
            new Navigation { Id = 18, ParentId = 14, Title = "Profit Share Report Edit Run", SubTitle = "PAY426N", Url = "", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false },
            new Navigation { Id = 19, ParentId = 18, Title = "Active/inactive employees age 18 - 20 with 1000 hours or more", SubTitle = "", Url = "pay426-1", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false },
            new Navigation { Id = 21, ParentId = 18, Title = "Active/inactive employees age 21 & with 1000 hours or more", SubTitle = "", Url = "pay426-2", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false },
            new Navigation { Id = 22, ParentId = 18, Title = "Active/inactive employees under 18", SubTitle = "", Url = "pay426-3", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false },
            new Navigation { Id = 23, ParentId = 18, Title = "Active/inactive employees 18 and older with prior profit sharing amounts and <1000 hours", SubTitle = "", Url = "pay426-4", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false },
            new Navigation { Id = 24, ParentId = 18, Title = "Active/inactive employees 18 and older with no prior profit sharing amounts and <1000 hours", SubTitle = "", Url = "pay426-5", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false },
            new Navigation { Id = 25, ParentId = 18, Title = "Terminated employees 18 and older with 1000 hours or more", SubTitle = "", Url = "pay426-6", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false },
            new Navigation { Id = 26, ParentId = 18, Title = "Terminated employees 18 and older with no prior profit sharing amounts and < 1000 hours", SubTitle = "", Url = "pay426-7", StatusId = 1, OrderNumber = 7, Icon = "", Disabled = false },
            new Navigation { Id = 27, ParentId = 18, Title = "Terminated employees 18 and older with prior profit sharing amounts and < 1000 hours", SubTitle = "", Url = "pay426-8", StatusId = 1, OrderNumber = 8, Icon = "", Disabled = false },
            new Navigation { Id = 28, ParentId = 18, Title = "Profit sharing summary page", SubTitle = "", Url = "pay426-9", StatusId = 1, OrderNumber = 9, Icon = "", Disabled = false },
            new Navigation { Id = 29, ParentId = 18, Title = "All non-employee beneficiaries", SubTitle = "", Url = "pay426-10", StatusId = 1, OrderNumber = 10, Icon = "", Disabled = false },
            new Navigation { Id = 30, ParentId = 14, Title = "Get Eligible Employees", SubTitle = "GET-ELIGIBLE-EMPS", Url = "eligible-employees", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false },
            new Navigation { Id = 31, ParentId = 14, Title = "Forfeit", SubTitle = "PAY443", Url = "forfeit", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false },
            new Navigation { Id = 32, ParentId = 14, Title = "Profit Share Updates", SubTitle = "PAY444|PAY447", Url = "profit-share-update", StatusId = 1, OrderNumber = 7, Icon = "", Disabled = false },
            new Navigation { Id = 33, ParentId = 14, Title = "Reports by Age", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 8, Icon = "", Disabled = false },
            new Navigation { Id = 34, ParentId = 33, Title = "Get Contributions by Age", SubTitle = "PROF130", Url = "contributions-by-age", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false },
            new Navigation { Id = 35, ParentId = 33, Title = "Get Distributions by Age", SubTitle = "PROF130", Url = "distributions-by-age", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false },
            new Navigation { Id = 36, ParentId = 33, Title = "Get Forfeiture by Age", SubTitle = "PROF130", Url = "forfeitures-by-age", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false },
            new Navigation { Id = 37, ParentId = 33, Title = "Get Balance by Age", SubTitle = "PROF130B", Url = "balance-by-age", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false },
            new Navigation { Id = 38, ParentId = 33, Title = "Get Vested Amounts by Age", SubTitle = "PROF130V", Url = "vested-amounts-by-age", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false },
            new Navigation { Id = 39, ParentId = 33, Title = "Get Balance by Years", SubTitle = "PROF130Y", Url = "balance-by-years", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false },
            new Navigation { Id = 40, ParentId = 14, Title = "Profit Share Gross Report", SubTitle = "QPAY501", Url = "profit-share-gross-report", StatusId = 1, OrderNumber = 9, Icon = "", Disabled = false },
            new Navigation { Id = 41, ParentId = 14, Title = "Profit Share by Store", SubTitle = "QPAY066TA", Url = "", StatusId = 1, OrderNumber = 9, Icon = "", Disabled = false },
            new Navigation { Id = 42, ParentId = 41, Title = "QPAY066-UNDR21", SubTitle = "", Url = "qpay066-under21", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false },
            new Navigation { Id = 43, ParentId = 41, Title = "QPAY066TA-UNDR21", SubTitle = "", Url = "qpay066ta-under21", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false },
            new Navigation { Id = 44, ParentId = 41, Title = "QPAY066TA", SubTitle = "", Url = "qpay066ta", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false },
            new Navigation { Id = 45, ParentId = 41, Title = "PROFALL Report", SubTitle = "", Url = "profall", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false },
            new Navigation { Id = 46, ParentId = 41, Title = "New Labels", SubTitle = "QNEWPROFLBL", Url = "new-ps-labels", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false },
            new Navigation { Id = 47, ParentId = 41, Title = "PROFNEW", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false },
            new Navigation { Id = 48, ParentId = 14, Title = "Print Profit Certs", SubTitle = "PAYCERT", Url = "", StatusId = 1, OrderNumber = 10, Icon = "", Disabled = false },
            new Navigation { Id = 49, ParentId = null, Title = "INQUIRIES", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false },
            new Navigation { Id = 50, ParentId = 49, Title = "MASTER INQUIRY", SubTitle = "", Url = "master-inquiry", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false },
            new Navigation { Id = 51, ParentId = null, Title = "BENEFICIARIES", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = true },
            new Navigation { Id = 52, ParentId = null, Title = "DISTRIBUTIONS", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = true },
            new Navigation { Id = 53, ParentId = null, Title = "RECONCILIATION", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = true },
            new Navigation { Id = 54, ParentId = null, Title = "YEAR END", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 5, Icon = "", Disabled = false },
            new Navigation { Id = 55, ParentId = null, Title = "IT OPERATIONS", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false },
            new Navigation { Id = 56, ParentId = 55, Title = "Demographic Freeze", SubTitle = "", Url = "demographic-freeze", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false },
            new Navigation { Id = 1, ParentId = 54, Title = "December Activities", SubTitle = "", Url = "december-process-accordion", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false },
            new Navigation { Id = 2, ParentId = 1, Title = "Clean up Reports", SubTitle = "", Url = "", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false },
            new Navigation { Id = 3, ParentId = 2, Title = "Demographic Badges Not In PayProfit", SubTitle = "", Url = "demographic-badges-not-in-payprofit", StatusId = 1, OrderNumber = 1, Icon = "", Disabled = false },
            new Navigation { Id = 4, ParentId = 2, Title = "Duplicate SSNs in Demographics", SubTitle = "", Url = "duplicate-ssns-demographics", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false },
            new Navigation { Id = 5, ParentId = 2, Title = "Negative ETVA", SubTitle = "", Url = "negative-etva-for-ssns-on-payprofit", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false },
            new Navigation { Id = 6, ParentId = 2, Title = "Duplicate Names and Birthdays", SubTitle = "", Url = "duplicate-names-and-birthdays", StatusId = 1, OrderNumber = 4, Icon = "", Disabled = false },
            new Navigation { Id = 7, ParentId = 1, Title = "Military Contributions", SubTitle = "TPR008-13", Url = "military-entry-and-modification", StatusId = 1, OrderNumber = 2, Icon = "", Disabled = false },
            new Navigation { Id = 8, ParentId = 1, Title = "Rehire Forfeitures", SubTitle = "QPREV-PROF", Url = "rehire-forfeitures", StatusId = 1, OrderNumber = 3, Icon = "", Disabled = false },
            new Navigation { Id = 10, ParentId = 1, Title = "Distributions and Forfeitures", SubTitle = "QPAY129", Url = "distributions-and-forfeitures", StatusId = 1, OrderNumber = 6, Icon = "", Disabled = false },
        };


        _navigationStatusList = new List<NavigationStatusDto>()
        {
            new NavigationStatusDto() { Id = NavigationStatus.Constants.NotStarted, Name = "Not Started" },
            new NavigationStatusDto() { Id = NavigationStatus.Constants.InProgress, Name = "In Progress" },
            new NavigationStatusDto() { Id = NavigationStatus.Constants.OnHold, Name = "On Hold" },
            new NavigationStatusDto() { Id = NavigationStatus.Constants.Complete, Name = "Complete" }
        };
    }
    [Fact(DisplayName = "PS-1009: Navigation")]
    public async Task GetNavigations()
    {
        var navigation = await _navigationService.GetNavigation(CancellationToken.None);
        Assert.NotNull(navigation);
        ComparisonExtensions.ShouldBeEquivalentTo(navigation, DummyNavigationData());
    }

    //Dummy Data
    private List<NavigationDto> DummyNavigationData()
    {
        var lookup = _navigationListObj.ToLookup(x => x.ParentId);
        List<NavigationDto> BuildTree(int? parentId)
        {
            return lookup[parentId]
                .Select(x => new NavigationDto
                {
                    Id = x.Id,
                    Icon = x.Icon,
                    OrderNumber = x.OrderNumber,
                    ParentId = x.ParentId,
                    StatusId = x.StatusId,
                    Title = x.Title,
                    Url = x.Url,
                    SubTitle = x.SubTitle,
                    Items = BuildTree(x.Id),
                    Disabled = x.Disabled,
                    RequiredRoles = x.RequiredRoles?.Select(m => m.Name).ToList()
                })
                .ToList();
        }

        return BuildTree(null); // root level
        
    }

    [Fact(DisplayName = "PS-1059: GetNavigationStatus")]
    public async Task GetNavigationStatus()
    {
        var navigationStatus = await _navigationService.GetNavigationStatus(CancellationToken.None);
        Assert.NotNull(navigationStatus);
        navigationStatus.ShouldBeEquivalentTo(_navigationStatusList);
    }

    [Fact(DisplayName = "PS-1059: Update navigation status")]
    public async Task UpdateNavigationStatus()
    {
        IAppUser iAppUser = new Mock<IAppUser>().Object;
        var success = await new NavigationService(MockDbContextFactory, iAppUser).UpdateNavigation(navigationId: 3, statusId: 1, cancellationToken: CancellationToken.None);
        Assert.True(success);
    }



}




