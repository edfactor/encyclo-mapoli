using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using FastEndpoints;

<<<<<<<< HEAD:src/services/src/Demoulas.ProfitSharing.Endpoints/Base/EndpointWithCSVBase.cs
namespace Demoulas.ProfitSharing.Endpoints.Base;
========
namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Csv;
>>>>>>>> origin/develop:src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/Reports/YearEnd/Csv/EndpointWithCSVBase.cs
/// <summary>
/// Endpoints deriving from this class will automatically be able to return a CSV when the accept headers contain text/csv.
/// The developer needs to override the GetResponse member to provide a response via a DTO.  The developer also needs to override the report filename property.
/// Configuration is still the responsibility of the developer.
/// </summary>
/// <typeparam name="ReqType">Request type of the endpoint.  Can be EmptyRequest</typeparam>
/// <typeparam name="RespType">Response type of the endpoint.</typeparam>
/// <typeparam name="MapType">A mapping class that converts from a dto to a CSV format</typeparam>
public abstract class EndpointWithCSVBase<ReqType, RespType, MapType> : Endpoint<ReqType, ReportResponseBase<RespType>>
    where ReqType : notnull
    where RespType : class
    where MapType : ClassMap<RespType>
{

    /// <summary>
    /// Called when the service is requested.  Developer should return a dto
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public abstract Task<ReportResponseBase<RespType>> GetResponse(CancellationToken ct);

    /// <summary>
    /// Returns the base portion of the filename downloaded to the browser.
    /// </summary>
    public abstract string ReportFileName { get; }

    public sealed override async Task HandleAsync(ReqType req, CancellationToken ct)
    {
        string acceptHeader = HttpContext.Request.Headers["Accept"].ToString().ToLower(CultureInfo.InvariantCulture);

        var response = await GetResponse(ct);

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
            csvWriter.WriteRecords(report.Results);
            streamWriter.Flush();
        }
        memoryStream.Position = 0; // Reset the stream position to the beginning
        return memoryStream;
    }
}
