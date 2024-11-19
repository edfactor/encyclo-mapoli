using System.Globalization;
using System.Text;
using CsvHelper.Configuration;
using CsvHelper;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Base;

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
    : FastEndpoints.Endpoint<ReqType, RespType>
    where RespType : ReportResponseBase<ItemType>
    where ReqType : PaginationRequestDto
    where ItemType : class
    where MapType : ClassMap<ItemType>
{
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

    protected PaginationRequestDto SimpleExampleRequest => new PaginationRequestDto { Skip = 0, Take = byte.MaxValue };

    public abstract Task<RespType> GetResponse(ReqType req, CancellationToken ct);

    public abstract string ReportFileName { get; }

    public sealed override async Task HandleAsync(ReqType req, CancellationToken ct)
    {
        string acceptHeader = HttpContext.Request.Headers.Accept.ToString().ToLower(CultureInfo.InvariantCulture);
        var response = await GetResponse(req, ct);

        if (acceptHeader.Contains("text/csv"))
        {
            await using MemoryStream csvData = await GenerateCsvStreamAsync(response, ct);
            await SendStreamAsync(csvData, $"{ReportFileName}.csv", contentType: "text/csv", cancellation: ct);
            return;
        }

        await SendOkAsync(response, ct);
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
