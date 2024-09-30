using System.Globalization;
using System.Text;
using CsvHelper.Configuration;
using CsvHelper;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Endpoints.Base;

/// <summary>
/// Endpoints deriving from this class will automatically be able to return a CSV when the accept headers contain text/csv.
/// The developer needs to override the GetResponse member to provide a response via a DTO.  The developer also needs to override the report filename property.
/// Configuration is still the responsibility of the developer.
/// </summary>
/// <typeparam name="ReqType">Request type of the endpoint.  Can be EmptyRequest</typeparam>
/// <typeparam name="ResponseWithTotals">Top level response, appropriate place for totals 
/// <typeparam name="RespType">Response type of the endpoint.</typeparam>
/// <typeparam name="MapType">A mapping class that converts from a dto to a CSV format</typeparam>
public abstract class EndpointWithCsvTotalsBase<ReqType, ResponseWithTotals, RespType, MapType> : FastEndpoints.Endpoint<ReqType, ResponseWithTotals>
    where ResponseWithTotals : ReportResponseBase<RespType>
    where ReqType : PaginationRequestDto
    where RespType : class
    where MapType : ClassMap<RespType>
{
    public override void Configure()
    {
        if (!Env.IsTestEnvironment())
        {
           // Specify caching duration and store it in metadata
           var cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }

        Description(b => 
            b.Produces<ReportResponseBase<RespType>>(200, "application/json", "text/csv"));
    }
    
    /// <summary>
    /// Use to provide a simple example request when no more complex than a simple Pagination Request is needed
    /// </summary>
    protected PaginationRequestDto SimpleExampleRequest => new PaginationRequestDto { Skip = 0, Take = byte.MaxValue };

    /// <summary>
    /// Asynchronously retrieves a response for the given request.
    /// </summary>
    /// <param name="req">The request object containing the necessary parameters.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response object.</returns>
    public abstract Task<ReportResponseBase<TerminatedEmployeeAndBeneficiaryDataResponseDto>> GetResponse(ReqType req, CancellationToken ct);

    /// <summary>
    /// Returns the base portion of the filename downloaded to the browser.
    /// </summary>
    public abstract string ReportFileName { get; }

    public sealed override async Task HandleAsync(ReqType req, CancellationToken ct)
    {
        string acceptHeader = HttpContext.Request.Headers["Accept"].ToString().ToLower(CultureInfo.InvariantCulture);
        var response = await GetResponse(req, ct);
        ResponseWithTotals responseWithTotals = (response as ResponseWithTotals)!;

        if (acceptHeader.Contains("text/csv"))
        {
            await using MemoryStream csvData = await GenerateCsvStreamAsync(responseWithTotals, ct);
            await SendStreamAsync(csvData, $"{ReportFileName}.csv", contentType: "text/csv", cancellation: ct);
            return;
        }

        await SendOkAsync(responseWithTotals, ct);
    }

    private async Task<MemoryStream> GenerateCsvStreamAsync(ResponseWithTotals report, CancellationToken cancellationToken)
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

    protected internal virtual async Task GenerateCsvContent(CsvWriter csvWriter, ReportResponseBase<RespType> report, CancellationToken cancellationToken)
    {
        csvWriter.Context.RegisterClassMap<MapType>();
        await csvWriter.WriteRecordsAsync(report.Response.Results, cancellationToken);
    }
}

