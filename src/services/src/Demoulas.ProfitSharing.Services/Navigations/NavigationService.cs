using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;

namespace Demoulas.ProfitSharing.Services.Navigations;
public class NavigationService : INavigationService
{
    public readonly List<Navigation> _navigations;


    public NavigationService()
    {
        this._navigations = new List<Navigation>()
        {
            //Populating navigation object.
            new Navigation()
            { Id = 1, Title="December Activities", Url = "", OrderNumber = 1, Status = NavigationStatus.NotStarted
                , Children = new List<Navigation>()
                {
                    new Navigation()
                    {Id = 1, ParentId =1, Title ="Clean Up Reports", Url ="", OrderNumber =1, Status = NavigationStatus.NotStarted,
                        Children = new List<Navigation>(){
                            new Navigation() { Id = 1, ParentId = 1, OrderNumber = 1, Title = "Demographic Badges Not In PayProfit", Url = "demographic-badges-not-in-payprofit", Status = NavigationStatus.NotStarted } ,
                            new Navigation() { Id = 2, ParentId = 1, OrderNumber = 2, Title = "Duplicate SSNs in Demographics", Url = "duplicate-ssns-demographics", Status = NavigationStatus.NotStarted } ,
                            new Navigation() { Id = 3, ParentId = 1, OrderNumber = 3, Title = "Negative ETVA", Url = "negative-etva-for-ssns-on-payprofit", Status = NavigationStatus.NotStarted } ,
                            new Navigation() { Id = 4, ParentId = 1, OrderNumber = 4, Title = "Demographic Badges Not In PayProfit", Url = "demographic-badges-not-in-payprofit", Status = NavigationStatus.NotStarted } ,
                            new Navigation() { Id = 5, ParentId = 1, OrderNumber = 5, Title = "Duplicate Names and Birthdays", Url = "duplicate-names-and-birthdays", Status = NavigationStatus.NotStarted }
                        },
                    },
                    new Navigation(){Id = 2, ParentId =1, Title ="Military Contributions", SubTitle = "TPR008-13", Url ="military-entry-and-modification", OrderNumber =2, Status = NavigationStatus.NotStarted},
                    new Navigation(){Id = 3, ParentId =1, Title ="Rehire Forfeitures", SubTitle = "QPREV-PROF", Url ="rehire-forfeitures", OrderNumber =3, Status = NavigationStatus.NotStarted},
                    new Navigation(){Id = 4, ParentId =1, Title ="Distributions and Forfeitures", SubTitle = "QPAY129", Url ="distributions-and-forfeitures", OrderNumber =4, Status = NavigationStatus.NotStarted},
                    new Navigation(){Id = 5, ParentId =1, Title ="Terminations", SubTitle = "QPAY066", Url ="prof-term", OrderNumber =4, Status = NavigationStatus.NotStarted},
                    new Navigation(){Id = 6, ParentId =1, Title ="Profit Share Report", SubTitle = "PAY426", Url ="profit-share-report", OrderNumber =5, Status = NavigationStatus.NotStarted}
                }
            }
        };
    }
    public List<Navigation> GetNavigation()
    {
        return this._navigations;
    }

    public Navigation GetNavigation(int navigationId)
    {
        return this._navigations.First(m => m.Id == navigationId);
    }
}
