using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.IntegrationTests.TotalSvc;

#pragma warning disable AsyncFixer01

// Pull out SMART payprofit data for comparision, limited at the moment to just Years In Service

public static class SmartPayProfitLoader
{
    public static async Task<Dictionary<int, PayProfitData>> GetSmartPayProfitDataByBadge(
        TotalService totalService,
        IProfitSharingDataContextFactory dbFactory,
        short profitYear,
        DateOnly asOfDate)
    {
        return await dbFactory.UseReadOnlyContext(async ctx =>
        {
            Dictionary<int, byte> yearsOfService = await totalService.GetYearsOfService(ctx, profitYear, asOfDate)
                .ToDictionaryAsync(y => y.Ssn, y => y.Years);

            var result = await ctx.PayProfits
                .Where(p => p.ProfitYear == profitYear && p.Demographic != null)
                .Select(p => new { p.Demographic!.BadgeNumber, p.Demographic.Ssn })
                .ToListAsync();

            Dictionary<int, PayProfitData> payProfitData = result
                .Where(r => yearsOfService.ContainsKey(r.Ssn)) // Filter out unmatched SSNs
                .ToDictionary(
                    r => r.BadgeNumber,
                    r => new PayProfitData { BadgeNumber = r.BadgeNumber, Years = yearsOfService[r.Ssn] });

            return payProfitData;
        });
    }
}
