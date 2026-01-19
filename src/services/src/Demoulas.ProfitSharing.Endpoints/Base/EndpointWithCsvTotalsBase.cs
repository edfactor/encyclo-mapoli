using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Api.Endpoints;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

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
    : DemoulasEndpoint<ReqType, Results<Ok<RespType>, FileStreamHttpResult>>, IHasNavigationId
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

    protected sealed override async Task<Results<Ok<RespType>, FileStreamHttpResult>> HandleRequestAsync(ReqType req, CancellationToken ct)
    {
        var httpContext = HttpContext ?? throw new InvalidOperationException("HttpContext is not available during endpoint execution.");
        string acceptHeader = httpContext.Request.Headers.Accept.ToString().ToLower(CultureInfo.InvariantCulture);

        if (acceptHeader.Contains("text/csv"))
        {
            // Ignore pagination for CSV reports
            req = req with { Skip = 0, Take = int.MaxValue };
        }
        var response = await GetResponse(req, ct);

        if (acceptHeader.Contains("text/csv"))
        {
            MemoryStream csvData = await GenerateCsvStreamAsync(response, ct);
            return TypedResults.File(csvData, "text/csv", $"{ReportFileName}.csv");
        }

        return TypedResults.Ok(response);
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
