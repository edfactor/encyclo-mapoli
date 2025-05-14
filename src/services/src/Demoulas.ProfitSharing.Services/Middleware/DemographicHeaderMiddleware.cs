using Demoulas.ProfitSharing.Common.Contracts.Response.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Services.Middleware;

public sealed class DemographicHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public DemographicHeaderMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Items.TryGetValue(
                DemographicReaderService.ItemKey,
                out var obj) &&
            obj is DataWindowMetadata meta)
        {
            context.Response.Headers[DemographicHeaders.Source] =
                meta.IsFrozen ? "Frozen" : "Live";

            context.Response.Headers[DemographicHeaders.Start] =
                meta.WindowStart.ToString("o");

            context.Response.Headers[DemographicHeaders.End] =
                meta.WindowEnd.ToString("o");
        }
    }
}
