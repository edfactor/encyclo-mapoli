using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
public class NavigationStatusFaker
{
    public List<NavigationStatus> DummyNavigationStatus()
    {
        return new List<NavigationStatus>() {
            new NavigationStatus() { Id = NavigationStatus.Constants.NotStarted, Name = "Not Started" },
            new NavigationStatus() { Id = NavigationStatus.Constants.InProgress, Name = "In Progress" },
            new NavigationStatus() { Id = NavigationStatus.Constants.Blocked, Name = "Blocked" },
            new NavigationStatus() { Id = NavigationStatus.Constants.Successful, Name = "Successful" }
        };
    }
}
