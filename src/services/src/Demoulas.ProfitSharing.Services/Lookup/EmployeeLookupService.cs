using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Lookup;

public sealed class EmployeeLookupService(IProfitSharingDataContextFactory factory) : IEmployeeLookupService
{
    public async Task<bool> BadgeExistsAsync(int badgeNumber, CancellationToken cancellationToken = default)
    {
        return await factory.UseReadOnlyContext(async ctx =>
        {
#pragma warning disable DSMPS001
            return await ctx.Demographics.AnyAsync(d => d.BadgeNumber == badgeNumber, cancellationToken);
#pragma warning restore DSMPS001
        });
    }

    public async Task<DateOnly?> GetEarliestHireDateAsync(int badgeNumber, CancellationToken cancellationToken = default)
    {
        return await factory.UseReadOnlyContext(async ctx =>
        {
#pragma warning disable DSMPS001
            var demo = await ctx.Demographics
                .Where(d => d.BadgeNumber == badgeNumber)
                .OrderBy(d => d.HireDate)
                .Select(d => new { d.HireDate, d.ReHireDate })
                .FirstOrDefaultAsync(cancellationToken);
#pragma warning restore DSMPS001
            DateOnly? earliest = null;
            if (demo != null)
            {
                if (demo.ReHireDate.HasValue && demo.ReHireDate.Value < demo.HireDate)
                {
                    earliest = demo.ReHireDate.Value;
                }
                else
                {
                    earliest = demo.HireDate;
                }
            }

            return earliest;
        });
    }
}
