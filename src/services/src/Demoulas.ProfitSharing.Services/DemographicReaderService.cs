using System.Collections.Concurrent;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.Headers;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.ItOperations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

/// <summary>
/// Chooses live or frozen data **per request** and records the choice in <see cref="HttpContext.Items"/>.
/// </summary>
public interface IDemographicReaderService
{
    Task<IQueryable<Demographic>> BuildQuery(IProfitSharingDbContext ctx,
        FrozenProfitYearRequest request);
}

public sealed class DemographicReaderService : IDemographicReaderService
{
    private readonly IFrozenService _frozenService;
    private readonly IHttpContextAccessor _http;

    public const string ItemKey = "__demographic_data_window";

    public DemographicReaderService(
        IFrozenService frozenService,
        IHttpContextAccessor http)
    {
        _frozenService = frozenService;
        _http = http;
    }

    public async Task<IQueryable<Demographic>> BuildQuery(IProfitSharingDbContext ctx,
        FrozenProfitYearRequest request)
    {
        if (request.UseFrozenData)
        {
            // ---- FROZEN ------------------------------------------------------
            var freeze = await _frozenService
                         .GetActiveFrozenDemographic();

            var meta = new DataWindowMetadata(
                IsFrozen: true,
                ProfitYear: freeze.ProfitYear,
                WindowStart: freeze.AsOfDateTime,
                WindowEnd: freeze.AsOfDateTime); // snapshot → start == end

            _http.HttpContext!.Items[ItemKey] = meta;

            return FrozenService.GetDemographicSnapshot(ctx, freeze.ProfitYear); // :contentReference[oaicite:0]{index=0}:contentReference[oaicite:1]{index=1}
        }

        // ---- LIVE -----------------------------------------------------------
        var now = DateTimeOffset.UtcNow;
        var liveMeta = new DataWindowMetadata(
            IsFrozen: false,
            ProfitYear: null,
            WindowStart: now,
            WindowEnd: now);

        _http.HttpContext!.Items[ItemKey] = liveMeta;

        return ctx.Demographics
                  .Include(d => d.ContactInfo)
                  .Include(d => d.Address);                                        // :contentReference[oaicite:2]{index=2}:contentReference[oaicite:3]{index=3}
    }
}
