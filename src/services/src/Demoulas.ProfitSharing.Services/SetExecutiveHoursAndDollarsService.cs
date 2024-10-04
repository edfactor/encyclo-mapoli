using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;
public class SetExecutiveHoursAndDollarsService : ISetExecutiveHoursAndDollarsService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    public SetExecutiveHoursAndDollarsService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    public async Task SetExecutiveHoursAndDollars(short profitYear, List<SetExecutiveHoursAndDollarsDto> executiveHoursAndDollarsDtos)
    {
        await _dataContextFactory.UseWritableContext(async ctx =>
        {
            int ppCountForYear =  await ctx.PayProfits.CountAsync(pp => pp.ProfitYear == profitYear);
            if (ppCountForYear == 0)
            {
                throw new InvalidOperationException("The provided year is invalid.");
            }

            var badges = executiveHoursAndDollarsDtos.Select(dto => dto.BadgeNumber).ToList();
            if (badges.Count != badges.Distinct().Count())
            {
                throw new InvalidOperationException("Badge numbers must be distinct.");
            }

            var demographicsWithPayProfits = await ctx.Demographics
                .Where(d => badges.Contains(d.BadgeNumber))
                .Join(ctx.PayProfits,
                    d => d.OracleHcmId,
                    pp => pp.OracleHcmId,
                    (d, pp) => new { Demographic = d, PayProfit = pp })
                .Where(joined => joined.PayProfit.ProfitYear == profitYear)
                .ToListAsync();

            if (executiveHoursAndDollarsDtos.Count != demographicsWithPayProfits.Count)
            {
                throw new InvalidOperationException("One or more badge numbers were not found.");
            }

            foreach (var demoWithPP in demographicsWithPayProfits)
            {
                var dto = executiveHoursAndDollarsDtos.First(x => x.BadgeNumber == demoWithPP.Demographic.BadgeNumber);

                demoWithPP.PayProfit.HoursExecutive = dto.ExecutiveHours;
                demoWithPP.PayProfit.IncomeExecutive = dto.ExecutiveDollars;
            }

            return ctx.SaveChangesAsync();
        });
    }


}
