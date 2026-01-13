using Demoulas.ProfitSharing.Common.Contracts.Response.Headers;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Services.Middleware;

public sealed class DemographicHeaderMiddleware
{
    private readonly RequestDelegate _next;
    public DemographicHeaderMiddleware(RequestDelegate next) => _next = next;

    public Task InvokeAsync(HttpContext context)
    {
        // Hook runs when the server is about to write the response,
        // but while headers are still mutable.
        context.Response.OnStarting(static state =>
        {
            var ctx = (HttpContext)state;

            if (ctx.Items.TryGetValue(
                    DemographicReaderService.ItemKey,
                    out object? obj) &&
                obj is DataWindowMetadata meta)
            {
                var headers = ctx.Response.Headers;
                headers[DemographicHeaders.Source] = meta.IsFrozen ? "Frozen" : "Live";
                headers[DemographicHeaders.End] = meta.WindowEnd.ToString("o");
            }

            return Task.CompletedTask;
        }, context);

        return _next(context);
    }
}
