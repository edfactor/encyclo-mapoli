using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Caching;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Lookup;

public sealed class StateTaxLookupService : IStateTaxLookupService
{
    private readonly StateTaxCache _cache;

    public StateTaxLookupService(StateTaxCache cache)
    {
        _cache = cache;
    }

    public async Task<StateTaxLookupResponse> LookupStateTaxRate(string state, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(state))
        {
            throw new InvalidOperationException("State tax");
        }

        var stateKey = state.ToUpperInvariant();
        var rate = await _cache.GetAsync(stateKey, cancellationToken);

        // If rate is null, state was not found in database
        if (rate == null)
        {
            throw new InvalidOperationException("State tax");
        }

        var response = new StateTaxLookupResponse
        {
            State = stateKey,
            StateTaxRate = rate.Value
        };

        return response;
    }
}
