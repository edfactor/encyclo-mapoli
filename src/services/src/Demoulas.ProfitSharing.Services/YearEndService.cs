using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;
public class YearEndService : IYearEndService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public YearEndService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }
    public async Task<IList<PayrollDuplicateSSNResponseDto>> GetDuplicateSSNs(CancellationToken ct)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var dupSsns = await ctx.Demographics.GroupBy(x=>x.SSN).Where(x=>x.Count() > 1).Select(x=>x.Key).ToListAsync();
            
            return await (from dem in ctx.Demographics
                          join pdJoin in ctx.ProfitDetails on dem.SSN equals pdJoin.SSN into demPdJoin
                          from pd in demPdJoin.DefaultIfEmpty()
                          join pp in ctx.PayProfits on dem.SSN equals pp.EmployeeSSN into DemPdPpJoin
                          from DemPdPp in DemPdPpJoin.DefaultIfEmpty()
                          where dupSsns.Contains(dem.SSN)
                          group new { dem, DemPdPp} 
                            by new { 
                                dem.BadgeNumber,
                                dem.SSN,
                                dem.FullName,
                                dem.Address.Street,
                                dem.Address.City,
                                dem.Address.State,
                                dem.Address.PostalCode,
                                dem.HireDate,
                                dem.TerminationDate,
                                dem.ReHireDate,
                                //Status,
                                dem.StoreNumber,
                                DemPdPp.HoursCurrentYear,
                                DemPdPp.EarningsCurrentYear
                            } into grp
                          select new PayrollDuplicateSSNResponseDto { 
                              BadgeNumber = grp.Key.BadgeNumber, 
                              SSN = grp.Key.SSN,
                              Name = grp.Key.FullName,
                              Address = grp.Key.Street,
                              City = grp.Key.City,
                              State = grp.Key.State,
                              PostalCode = grp.Key.PostalCode,
                              HireDate = grp.Key.HireDate,
                              TerminationDate = grp.Key.TerminationDate,
                              RehireDate = grp.Key.ReHireDate,
                              Status = 'a', //TODO: Where is this in demographics?
                              StoreNumber = grp.Key.StoreNumber,
                              ProfitSharingRecords = grp.Count(),
                              HoursCurrentYear = grp.Key.HoursCurrentYear,
                              EarningsCurrentYear = grp.Key.EarningsCurrentYear,
                          }
                          ).ToListAsync(ct);
        });
    }
}
