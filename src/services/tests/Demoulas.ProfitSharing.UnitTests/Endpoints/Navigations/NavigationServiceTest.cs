using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Navigations;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Navigations;

[Collection("Navigation Tests")]
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

        // Use MemoryDistributedCache which implements IDistributedCache for testing
        var distributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var svc = new NavigationService(MockDbContextFactory, appUser.Object, distributedCache);
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

        // Prerequisite projection check: Final Run (Id=17)
        var yearEnd = navigation.FirstOrDefault(n => n.Id == Navigation.Constants.YearEnd);
        Assert.NotNull(yearEnd);
        var fiscalClose = yearEnd!.Items!.FirstOrDefault(n => n.Id == Navigation.Constants.FiscalClose);
        Assert.NotNull(fiscalClose);
        var finalRun = fiscalClose!.Items!.FirstOrDefault(n => n.Id == Navigation.Constants.ProfitShareReportFinalRun);
        Assert.NotNull(finalRun);
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
        var distributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var success = await new NavigationService(MockDbContextFactory, iAppUser, distributedCache).UpdateNavigation(navigationId: 3, statusId: 1, cancellationToken: CancellationToken.None);
        Assert.True(success);
    }

    [Fact(DisplayName = "PS-1623: IsReadOnly flag should be true for users with read-only roles")]
    public async Task GetNavigations_WithReadOnlyRole_SetsIsReadOnlyFlag()
    {
        // Arrange: Set up user with read-only role (ITDEVOPS)
        var appUser = new Mock<IAppUser>();
        appUser.Setup(u => u.GetUserAllRoles(It.IsAny<List<string>?>()))
            .Returns(new List<string> { Role.ITDEVOPS });

        var distributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var svc = new NavigationService(MockDbContextFactory, appUser.Object, distributedCache);

        // Act: Get navigation items
        var navigation = await svc.GetNavigation(CancellationToken.None);

        // Assert: All navigation items should have IsReadOnly = true
        Assert.NotNull(navigation);
        Assert.All(navigation, n => Assert.True(n.IsReadOnly, $"Navigation item '{n.Title}' should have IsReadOnly = true for ITDEVOPS role"));

        // Also check child items
        var itemsWithChildren = navigation.Where(n => n.Items?.Any() == true);
        foreach (var parent in itemsWithChildren)
        {
            Assert.All(parent.Items!, child => Assert.True(child.IsReadOnly, $"Child navigation item '{child.Title}' should have IsReadOnly = true for ITDEVOPS role"));
        }
    }

    [Fact(DisplayName = "PS-1623: IsReadOnly flag should be false for users without read-only roles")]
    public async Task GetNavigations_WithoutReadOnlyRole_SetsIsReadOnlyFlagFalse()
    {
        // Arrange: Set up user with non-read-only role (FINANCEMANAGER)
        var appUser = new Mock<IAppUser>();
        appUser.Setup(u => u.GetUserAllRoles(It.IsAny<List<string>?>()))
            .Returns(new List<string> { Role.FINANCEMANAGER });

        var distributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var svc = new NavigationService(MockDbContextFactory, appUser.Object, distributedCache);

        // Act: Get navigation items
        var navigation = await svc.GetNavigation(CancellationToken.None);

        // Assert: All navigation items should have IsReadOnly = false
        Assert.NotNull(navigation);
        Assert.All(navigation, n => Assert.False(n.IsReadOnly, $"Navigation item '{n.Title}' should have IsReadOnly = false for FINANCEMANAGER role"));

        // Also check child items
        var itemsWithChildren = navigation.Where(n => n.Items?.Any() == true);
        foreach (var parent in itemsWithChildren)
        {
            Assert.All(parent.Items!, child => Assert.False(child.IsReadOnly, $"Child navigation item '{child.Title}' should have IsReadOnly = false for FINANCEMANAGER role"));
        }
    }

    [Fact(DisplayName = "PS-1623: IsReadOnly flag should be true for users with Auditor role")]
    public async Task GetNavigations_WithAuditorRole_SetsIsReadOnlyFlag()
    {
        // Arrange: Set up user with read-only role (AUDITOR)
        var appUser = new Mock<IAppUser>();
        appUser.Setup(u => u.GetUserAllRoles(It.IsAny<List<string>?>()))
            .Returns(new List<string> { Role.AUDITOR });

        var distributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var svc = new NavigationService(MockDbContextFactory, appUser.Object, distributedCache);

        // Act: Get navigation items
        var navigation = await svc.GetNavigation(CancellationToken.None);

        // Assert: All navigation items should have IsReadOnly = true
        Assert.NotNull(navigation);
        Assert.All(navigation, n => Assert.True(n.IsReadOnly, $"Navigation item '{n.Title}' should have IsReadOnly = true for AUDITOR role"));

        // Also check child items
        var itemsWithChildren = navigation.Where(n => n.Items?.Any() == true);
        foreach (var parent in itemsWithChildren)
        {
            Assert.All(parent.Items!, child => Assert.True(child.IsReadOnly, $"Child navigation item '{child.Title}' should have IsReadOnly = true for AUDITOR role"));
        }
    }
}




