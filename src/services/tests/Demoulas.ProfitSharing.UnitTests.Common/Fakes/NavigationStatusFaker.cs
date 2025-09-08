using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
public class NavigationStatusFaker
{
    public List<NavigationStatus> DummyNavigationStatus()
    {
        return new List<NavigationStatus>() {
            new NavigationStatus() { Id = NavigationStatus.Constants.NotStarted, Name = "Not Started" },
            new NavigationStatus() { Id = NavigationStatus.Constants.InProgress, Name = "In Progress" },
            new NavigationStatus() { Id = NavigationStatus.Constants.OnHold, Name = "On Hold" },
            new NavigationStatus() { Id = NavigationStatus.Constants.Complete, Name = "Complete" }
        };
    }
}
