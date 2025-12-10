using System.Diagnostics;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Base;

file static class EndpointActivity
{
    public static readonly ActivitySource Source = new("Demoulas.ProfitSharing.Endpoints");
}

/// <summary>
/// Endpoints deriving from this class will automatically be able to return a CSV when the accept headers contain text/csv.
/// The developer needs to override the GetResponse member to provide a response via a DTO.  The developer also needs to override the report filename property.
/// Configuration is still the responsibility of the developer.
/// </summary>
/// <typeparam name="ReqType">Request type of the endpoint.  Can be EmptyRequest</typeparam>
/// <typeparam name="RespType">Response type of the endpoint.</typeparam>
/// <typeparam name="MapType">A mapping class that converts from a dto to a CSV format</typeparam>
public abstract class EndpointWithCsvBase<ReqType, RespType, MapType> : FastEndpoints.Endpoint<ReqType, ReportResponseBase<RespType>>, IHasNavigationId
    where ReqType : SortedPaginationRequestDto
    where RespType : class
    where MapType : ClassMap<RespType>
{
    // Activity source is held in non-generic static to avoid per-closed-generic duplication
    protected EndpointWithCsvBase(short navigationId)
    {
        NavigationId = navigationId;
    }

    public short NavigationId { get; protected set; }

    public override void Configure()
    {
        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }

        Description(b =>
            b.Produces<ReportResponseBase<RespType>>(200, "application/json", "text/csv"));
    }

    /// <summary>
    /// Use to provide a simple example request when no more complex than a simple Pagination Request is needed
    /// </summary>
    protected SortedPaginationRequestDto SimpleExampleRequest => new SortedPaginationRequestDto
    {
        Skip = 0,
        Take = byte.MaxValue,
        SortBy = "columnName",
        IsSortDescending = true
    };

    /// <summary>
    /// Asynchronously retrieves a response for the given request.
    /// </summary>
    /// <param name="req">The request object containing the necessary parameters.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response object.</returns>
    public abstract Task<ReportResponseBase<RespType>> GetResponse(ReqType req, CancellationToken ct);

    /// <summary>
    /// Returns the base portion of the filename downloaded to the browser.
    /// </summary>
    public abstract string ReportFileName { get; }

    public sealed override async Task HandleAsync(ReqType req, CancellationToken ct)
    {
        string endpointName = GetType().Name;
        using Activity? activity = Activity.Current is null
            ? EndpointActivity.Source.StartActivity(endpointName, ActivityKind.Server)
            : null;
        if (activity is not null)
        {
            activity.DisplayName = endpointName;
            activity.SetTag("endpoint.name", endpointName);
            activity.SetTag("endpoint.class", GetType().FullName);
            activity.SetTag("navigation.id", NavigationId);
            if (HttpContext is not null)
            {
                activity.SetTag("http.route", HttpContext.Request?.Path.ToString());
                activity.SetTag("http.method", HttpContext.Request?.Method);
            }
        }

        var httpContext = HttpContext ?? throw new InvalidOperationException("HttpContext is not available during endpoint execution.");
        var services = httpContext.RequestServices;
        var loggerFactory = services.GetService<ILoggerFactory>();
        var logger = loggerFactory?.CreateLogger(endpointName);
        var appUser = services.GetService<IAppUser>();
        string userName = appUser?.UserName ?? "Unknown";
        activity?.SetTag("enduser.id", userName);
        using IDisposable? scope = logger?.BeginScope(new Dictionary<string, object?>
        {
            ["UserName"] = userName,
            ["Endpoint"] = endpointName,
            ["NavigationId"] = NavigationId,
        });

        // httpContext is validated non-null above
        string acceptHeader = httpContext.Request.Headers.Accept.ToString().ToLower(CultureInfo.InvariantCulture);

        if (acceptHeader.Contains("text/csv"))
        {
            // Ignore pagination for CSV reports
            req = req with { Skip = 0, Take = int.MaxValue };
        }

        ReportResponseBase<RespType> response = await GetResponse(req, ct);

        if (acceptHeader.Contains("text/csv"))
        {
            await using MemoryStream csvData = await GenerateCsvStreamAsync(response, ct);
            await Send.StreamAsync(csvData, $"{ReportFileName}.csv", contentType: "text/csv", cancellation: ct);
            return;
        }

        await Send.OkAsync(response, ct);
    }

    private async Task<MemoryStream> GenerateCsvStreamAsync(ReportResponseBase<RespType> report, CancellationToken cancellationToken)
    {
        MemoryStream memoryStream = new MemoryStream();
        await using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        await using (CsvWriter csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
        {
            await streamWriter.WriteLineAsync($"{report.ReportDate:MMM dd yyyy HH:mm}".AsMemory(), cancellationToken);
            await streamWriter.WriteLineAsync(report.ReportName.AsMemory(), cancellationToken);

            await GenerateCsvContent(csvWriter, report, cancellationToken);

            await streamWriter.FlushAsync(cancellationToken);
        }

        memoryStream.Position = 0; // Reset the stream position to the beginning
        return memoryStream;
    }

    protected internal virtual Task GenerateCsvContent(CsvWriter csvWriter, ReportResponseBase<RespType> report, CancellationToken cancellationToken)
    {
        _ = csvWriter.Context.RegisterClassMap<MapType>();
        return csvWriter.WriteRecordsAsync(report.Response.Results, cancellationToken);
    }
}
