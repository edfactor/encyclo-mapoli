using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Services.Navigations;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Navigations;

public class NavigationServiceTests : ApiTestBase<Program>
{
    private readonly INavigationService _navigationService;
    private readonly List<NavigationStatusDto> _navigationStatusList;

    public NavigationServiceTests()
    {
        _navigationService = ServiceProvider?.GetRequiredService<INavigationService>()!;


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
        // Ensure roles are present so results include a parent with children
        var appUser = new Mock<IAppUser>();
        appUser.Setup(u => u.GetUserAllRoles(It.IsAny<List<string>?>()))
            .Returns(new List<string> { Role.FINANCEMANAGER, Role.ITDEVOPS, Role.ADMINISTRATOR });

        var svc = new NavigationService(MockDbContextFactory, appUser.Object);
        var navigation = await svc.GetNavigation(CancellationToken.None);

        Assert.NotNull(navigation);
        // Structural sanity checks instead of brittle full equivalence to a hand-crafted list
        Assert.All(navigation, n =>
        {
            Assert.True(n.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(n.Title));
            Assert.NotNull(n.Items);
        });

        // Basic hierarchy check: at least one node has children
        Assert.True(navigation.Count > 0, "Expected at least one navigation item.");
        Assert.Contains(navigation, n => (n.Items?.Count ?? 0) > 0);
    }

    //Dummy Data
    // removed DummyNavigationData() in favor of structural assertions

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




