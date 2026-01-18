using Demoulas.Common.Data.Services.Entities.Entities.Navigation;
using Demoulas.ProfitSharing.Common.Constants;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

public class NavigationStatusFaker
{
    public List<NavigationStatus> DummyNavigationStatus()
    {
        return new List<NavigationStatus>() {
            new NavigationStatus() { Id = NavigationStatusIds.NotStarted, Name = "Not Started" },
            new NavigationStatus() { Id = NavigationStatusIds.InProgress, Name = "In Progress" },
            new NavigationStatus() { Id = NavigationStatusIds.OnHold, Name = "On Hold" },
            new NavigationStatus() { Id = NavigationStatusIds.Complete, Name = "Complete" }
        };
    }
}
