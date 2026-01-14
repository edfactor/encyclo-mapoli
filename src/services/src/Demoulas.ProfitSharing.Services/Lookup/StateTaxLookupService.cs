using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Lookup;

public sealed class StateTaxLookupService : IStateTaxLookupService
{
    private readonly StateTaxCache _cache;
    private readonly ILogger<StateTaxLookupService> _logger;

    public StateTaxLookupService(StateTaxCache cache, ILogger<StateTaxLookupService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<StateTaxLookupResponse> LookupStateTaxRate(string state, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(state))
        {
            throw new InvalidOperationException("State is required");
        }

        var stateKey = state.ToUpperInvariant();
        _logger.LogDebug("Looking up state tax rate for state: {StateKey}", stateKey);

        var rate = await _cache.GetAsync(stateKey, cancellationToken);

        _logger.LogDebug("Cache lookup for state {StateKey} returned: {Rate}", stateKey, rate?.ToString() ?? "null");

        // If rate is null, state was not found in database
        if (rate == null)
        {
            _logger.LogWarning("State tax rate not found for state {StateKey}", stateKey);
            throw new InvalidOperationException($"State tax rate not found for state {stateKey}");
        }

        _logger.LogInformation("State tax lookup successful for state {StateKey}: {Rate}", stateKey, rate.Value);

        var response = new StateTaxLookupResponse
        {
            State = stateKey,
            StateTaxRate = rate.Value
        };

        return response;
    }
}
