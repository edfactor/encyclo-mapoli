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
}
