using System;
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
/// <typeparam name="ReqType">Request type of the endpoint. Can be EmptyRequest</typeparam>
/// <typeparam name="RespType">Response type of the endpoint, containing totals and results.</typeparam>
/// <typeparam name="ItemType">The type of the individual result items in the response.</typeparam>
/// <typeparam name="MapType">A mapping class that converts from a dto to a CSV format</typeparam>
public abstract class EndpointWithCsvTotalsBase<ReqType, RespType, ItemType, MapType>
    : FastEndpoints.Endpoint<ReqType, RespType>, IHasNavigationId
    where RespType : ReportResponseBase<ItemType>
    where ReqType : SortedPaginationRequestDto
    where ItemType : class
    where MapType : ClassMap<ItemType>
{
    protected EndpointWithCsvTotalsBase(short navigationId)
    {
        NavigationId = navigationId;
    }

    public short NavigationId { get; protected set; }

    public override void Configure()
    {
        if (!Env.IsTestEnvironment())
        {
            var cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }

        Description(b =>
            b.Produces<RespType>(200, "application/json", "text/csv"));
    }

    protected SortedPaginationRequestDto SimpleExampleRequest => new SortedPaginationRequestDto
    {
        Skip = 0,
        Take = byte.MaxValue,
        SortBy = "columnName",
        IsSortDescending = true
    };

    public abstract Task<RespType> GetResponse(ReqType req, CancellationToken ct);

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

        string acceptHeader = httpContext.Request.Headers.Accept.ToString().ToLower(CultureInfo.InvariantCulture);

        if (acceptHeader.Contains("text/csv"))
        {
            // Ignore pagination for CSV reports
            req = req with { Skip = 0, Take = int.MaxValue };
        }
        var response = await GetResponse(req, ct);

        if (acceptHeader.Contains("text/csv"))
        {
            await using MemoryStream csvData = await GenerateCsvStreamAsync(response, ct);
            await Send.StreamAsync(csvData, $"{ReportFileName}.csv", contentType: "text/csv", cancellation: ct);
            return;
        }

        await Send.OkAsync(response, ct);
    }

    private async Task<MemoryStream> GenerateCsvStreamAsync(RespType report, CancellationToken cancellationToken)
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

    protected internal virtual Task GenerateCsvContent(CsvWriter csvWriter, RespType report, CancellationToken cancellationToken)
    {
        return GenerateCsvContent(csvWriter, report.Response.Results, cancellationToken);
    }

    protected internal virtual Task GenerateCsvContent(CsvWriter csvWriter, IEnumerable<ItemType> items, CancellationToken cancellationToken)
    {
        csvWriter.Context.RegisterClassMap<MapType>();
        return csvWriter.WriteRecordsAsync(items, cancellationToken);
    }
}
