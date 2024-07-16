using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class NegativeETVAForSSNsOnPayProfitEndPoint : EndpointWithoutRequest<ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>>
{
    private readonly IYearEndService _reportService;

    public NegativeETVAForSSNsOnPayProfitEndPoint(IYearEndService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("negative-evta-ssn");
        Summary(s =>
        {
            s.Summary = "Negative ETVA for SSNs on PayProfit";
            s.ResponseExamples = new Dictionary<int, object> { { 200, new List<ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>>() } };
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string acceptHeader = HttpContext.Request.Headers["Accept"].ToString().ToLower(CultureInfo.InvariantCulture);

        ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse> response = await _reportService.GetNegativeETVAForSSNsOnPayProfitResponse(ct);

        if (acceptHeader.Contains("text/csv"))
        {
            await using MemoryStream csvData = GenerateCsvStream(response);
            await SendStreamAsync(csvData, "ETVA-LESS-THAN-ZERO.csv", contentType: "text/csv", cancellation: ct);
            return;
        }

        await SendOkAsync(response, ct);
    }


    private MemoryStream GenerateCsvStream(ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse> report)
    {
        MemoryStream memoryStream = new MemoryStream();
        using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        using (CsvWriter csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
        {
            streamWriter.WriteLine($"{report.ReportDate:MMM dd yyyy HH:mm}");
            streamWriter.WriteLine(report.ReportName);
            streamWriter.WriteLine(",,BADGE,SSN,ETVA");

            csvWriter.Context.RegisterClassMap<NegativeETVAForSSNsOnPayProfitResponseMap>();
            csvWriter.WriteRecords(report.Results);
            streamWriter.Flush();
        }
        memoryStream.Position = 0; // Reset the stream position to the beginning
        return memoryStream;
    }

    private sealed class NegativeETVAForSSNsOnPayProfitResponseMap : ClassMap<NegativeETVAForSSNsOnPayProfitResponse>
    {
        public NegativeETVAForSSNsOnPayProfitResponseMap()
        {
            Map(m => m.EmployeeBadge).Index(1).Name("BADGE");
            Map(m => m.EmployeeSSN).Index(2).Name("SSN");
            Map(m => m.EtvaValue).Index(3).Name("ETVA");
        }
    }
}
