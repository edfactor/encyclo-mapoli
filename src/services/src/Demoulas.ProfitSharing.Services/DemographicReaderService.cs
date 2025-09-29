using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Headers;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Services;

public sealed class DemographicReaderService : IDemographicReaderService
{
    private readonly IFrozenService _frozenService;
    private readonly IHttpContextAccessor _http;

    public const string ItemKey = "__demographic_data_window";

    private FrozenStateResponse? _frozenState = null;

    public DemographicReaderService(
        IFrozenService frozenService,
        IHttpContextAccessor http)
    {
        _frozenService = frozenService;
        _http = http;
    }

    public async Task<IQueryable<Demographic>> BuildDemographicQuery(IProfitSharingDbContext ctx, bool useFrozenData = false)
    {
        if (useFrozenData)
        {
            // ---- FROZEN ------------------------------------------------------
            _frozenState ??= await _frozenService.GetActiveFrozenDemographic();

            if (_http.HttpContext != null)
            {
                var meta = new DataWindowMetadata(
                    IsFrozen: true,
                    ProfitYear: _frozenState.ProfitYear,
                    WindowEnd: _frozenState.AsOfDateTime); // snapshot → start == end

                _http.HttpContext!.Items[ItemKey] = meta;
            }

#pragma warning disable DSMPS001
            return FrozenService.GetDemographicSnapshot(ctx, _frozenState.ProfitYear);
#pragma warning restore DSMPS001
        }

        // ---- LIVE -----------------------------------------------------------
        if (_http.HttpContext != null)
        {
            var now = DateTimeOffset.UtcNow;
            var liveMeta = new DataWindowMetadata(
                IsFrozen: false,
                ProfitYear: null,
                WindowEnd: now);

            _http.HttpContext.Items[ItemKey] = liveMeta;
        }

#pragma warning disable DSMPS001
        return ctx.Demographics;
#pragma warning restore DSMPS001
    }

    public IQueryable<Demographic> BuildDemographicQueryAsOf(IProfitSharingDbContext ctx, DateTimeOffset asOf)
    {
        // Build a temporal snapshot based on an explicit as-of timestamp. This bypasses the active frozen state.
        if (_http.HttpContext != null)
        {
            var meta = new DataWindowMetadata(
                IsFrozen: true,
                ProfitYear: (short)asOf.Year,
                WindowEnd: asOf);
            _http.HttpContext.Items[ItemKey] = meta;
        }

#pragma warning disable DSMPS001
        return FrozenService.GetDemographicSnapshotAsOf(ctx, asOf);
#pragma warning restore DSMPS001
    }
}
