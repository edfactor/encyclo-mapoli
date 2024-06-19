using System.Diagnostics;

namespace Demoulas.ProfitSharing.Api.Middleware;

public sealed class ServerTimingMiddleware
{
    private readonly RequestDelegate _next;

    public ServerTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch watch = Stopwatch.StartNew();
        context.Response.OnStarting(() =>
        {
            watch.Stop();
            context.Response.Headers.Append("Server-Timing", $"total;dur={watch.ElapsedMilliseconds}");
            return Task.CompletedTask;
        });

        await _next(context).ConfigureAwait(false);
        watch.Stop();
    }
}
