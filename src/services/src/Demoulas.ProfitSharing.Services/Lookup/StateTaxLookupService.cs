using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Lookup;
public sealed class StateTaxLookupService : IStateTaxLookupService
{
    private readonly IProfitSharingDataContextFactory _factory;
    public StateTaxLookupService(IProfitSharingDataContextFactory factory)
    {
        _factory = factory;
    }
    
    public async Task<StateTaxLookupResponse> LookupStateTaxRate(string state, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(state))
        {
            throw new InvalidOperationException("State tax");
        }

        var stateTax = await _factory.UseReadOnlyContext(async ctx =>
        {
            return await ctx.StateTaxes
                .Where(st => st.Abbreviation == state.ToUpperInvariant())
                .Select(st => new { st.Abbreviation, st.Rate })
                .FirstOrDefaultAsync(cancellationToken);
        });

        if (stateTax == null)
        {
            throw new InvalidOperationException("State tax");
        }

        var response = new StateTaxLookupResponse
        {
            State = stateTax.Abbreviation,
            StateTaxRate = stateTax.Rate
        };

        return response;
    }
}
