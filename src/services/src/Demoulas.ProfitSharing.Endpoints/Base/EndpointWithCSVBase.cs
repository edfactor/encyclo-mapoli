using System.Globalization;
using System.Text;
using CsvHelper.Configuration;
using CsvHelper;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Base;

/// <summary>
/// Endpoints deriving from this class will automatically be able to return a CSV when the accept headers contain text/csv.
/// The developer needs to override the GetResponse member to provide a response via a DTO.  The developer also needs to override the report filename property.
/// Configuration is still the responsibility of the developer.
/// </summary>
/// <typeparam name="ReqType">Request type of the endpoint.  Can be EmptyRequest</typeparam>
/// <typeparam name="RespType">Response type of the endpoint.</typeparam>
/// <typeparam name="MapType">A mapping class that converts from a dto to a CSV format</typeparam>
public abstract class EndpointWithCsvBase<ReqType, RespType, MapType> : Endpoint<ReqType, ReportResponseBase<RespType>>
    where ReqType : PaginationRequestDto
    where RespType : class
    where MapType : ClassMap<RespType>
{
    /// <summary>
    /// Use to provide a simple example request when no more complex than a simple Pagination Request is needed
    /// </summary>
    protected PaginationRequestDto SimpleExampleRequest => new PaginationRequestDto { Skip = 0, Take = byte.MaxValue };

    /// <summary>
    /// Called when the service is requested.  Developer should return a dto
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public abstract Task<ReportResponseBase<RespType>> GetResponse(ReqType req, CancellationToken ct);

    /// <summary>
    /// Returns the base portion of the filename downloaded to the browser.
    /// </summary>
    public abstract string ReportFileName { get; }

    public sealed override async Task HandleAsync(ReqType req, CancellationToken ct)
    {
        string acceptHeader = HttpContext.Request.Headers["Accept"].ToString().ToLower(CultureInfo.InvariantCulture);
        var response = await GetResponse(req, ct);

        if (acceptHeader.Contains("text/csv"))
        {
            await using MemoryStream csvData = GenerateCsvStream(response);
            await SendStreamAsync(csvData, $"{ReportFileName}.csv", contentType: "text/csv", cancellation: ct);
            return;
        }

        await SendOkAsync(response, ct);
    }

    private MemoryStream GenerateCsvStream(ReportResponseBase<RespType> report)
    {
        MemoryStream memoryStream = new MemoryStream();
        using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        using (CsvWriter csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
        {
            streamWriter.WriteLine($"{report.ReportDate:MMM dd yyyy HH:mm}");
            streamWriter.WriteLine(report.ReportName);

            csvWriter.Context.RegisterClassMap<MapType>();
            csvWriter.WriteRecords(report.Response.Results);
            streamWriter.Flush();
        }

        memoryStream.Position = 0; // Reset the stream position to the beginning
        return memoryStream;
    }
}
