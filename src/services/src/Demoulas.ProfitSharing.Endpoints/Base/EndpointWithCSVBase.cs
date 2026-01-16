using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Api.Endpoints;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Base;

/// <summary>
/// Endpoints deriving from this class will automatically be able to return a CSV when the accept headers contain text/csv.
/// The developer needs to override the GetResponse member to provide a response via a DTO.  The developer also needs to override the report filename property.
/// Configuration is still the responsibility of the developer.
/// </summary>
/// <typeparam name="ReqType">Request type of the endpoint.  Can be EmptyRequest</typeparam>
/// <typeparam name="RespType">Response type of the endpoint.</typeparam>
/// <typeparam name="MapType">A mapping class that converts from a dto to a CSV format</typeparam>
public abstract class EndpointWithCsvBase<ReqType, RespType, MapType> : DemoulasEndpoint<ReqType, Results<Ok<ReportResponseBase<RespType>>, FileStreamHttpResult>>, IHasNavigationId
    where ReqType : SortedPaginationRequestDto
    where RespType : class
    where MapType : ClassMap<RespType>
{
    private readonly bool _addHeaderWithDateAndReportName;
    // Activity source is held in non-generic static to avoid per-closed-generic duplication
    protected EndpointWithCsvBase(short navigationId, bool addHeaderWithDateAndReportName = true)
    {
        NavigationId = navigationId;
        _addHeaderWithDateAndReportName = addHeaderWithDateAndReportName;
    }

    public short NavigationId { get; protected set; }

    public override void Configure()
    {
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

    protected sealed override async Task<Results<Ok<ReportResponseBase<RespType>>, FileStreamHttpResult>> HandleRequestAsync(ReqType req, CancellationToken ct)
    {
        var httpContext = HttpContext ?? throw new InvalidOperationException("HttpContext is not available during endpoint execution.");
        string acceptHeader = httpContext.Request.Headers.Accept.ToString().ToLower(CultureInfo.InvariantCulture);

        if (acceptHeader.Contains("text/csv"))
        {
            // Ignore pagination for CSV reports
            req = req with { Skip = 0, Take = int.MaxValue };
        }

        ReportResponseBase<RespType> response = await GetResponse(req, ct);

        if (acceptHeader.Contains("text/csv"))
        {
            MemoryStream csvData = await GenerateCsvStreamAsync(response, ct);
            return TypedResults.File(csvData, "text/csv", $"{ReportFileName}.csv");
        }

        return TypedResults.Ok(response);
    }

    private async Task<MemoryStream> GenerateCsvStreamAsync(ReportResponseBase<RespType> report, CancellationToken cancellationToken)
    {
        MemoryStream memoryStream = new MemoryStream();
        await using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        await using (CsvWriter csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
        {
            if (_addHeaderWithDateAndReportName)
            {
                await streamWriter.WriteLineAsync($"{report.ReportDate:MMM dd yyyy HH:mm}".AsMemory(), cancellationToken);
                await streamWriter.WriteLineAsync(report.ReportName.AsMemory(), cancellationToken);
            }

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
