using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using System.Text;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;

public class TerminatedEmployeeAndBeneficiaryDataEndpoint
    : FastEndpoints.Endpoint<StartAndEndDateRequest, TerminatedEmployeeAndBeneficiaryResponse>
{
    private readonly ITerminatedEmployeeAndBeneficiaryReportService _terminatedEmployeeAndBeneficiaryReportService;

    public TerminatedEmployeeAndBeneficiaryDataEndpoint(
        ITerminatedEmployeeAndBeneficiaryReportService terminatedEmployeeAndBeneficiaryReportService)
    {
        ReportFileName = "TerminatedEmployeeAndBeneficiaryReport.csv";
        _terminatedEmployeeAndBeneficiaryReportService = terminatedEmployeeAndBeneficiaryReportService;
    }

    public override void Configure()
    {
        Get("/terminated-employees");
        Summary(s =>
        {
            s.Summary = "Provide the Terminated Employees (QPAY066) report.";
            s.Description =
                "Reports on beneficiaries with a non-zero balance and employees who were terminated (and not retired) in the specified date range.";
            s.ExampleRequest = new ProfitYearRequest() { ProfitYear = 2024 };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new TerminatedEmployeeAndBeneficiaryResponse
                    {
                        ReportName = "Terminated Employees",
                        ReportDate = default,
                        TotalVested = 1000,
                        TotalForfeit = 2000,
                        TotalEndingBalance = 3000,
                        TotalBeneficiaryAllocation = 4000,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<TerminatedEmployeeAndBeneficiaryDataResponseDto>
                        {
                            Total = 1,
                            Results = new List<TerminatedEmployeeAndBeneficiaryDataResponseDto> { TerminatedEmployeeAndBeneficiaryDataResponseDto.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(StartAndEndDateRequest req, CancellationToken ct)
    {
        string acceptHeader = HttpContext.Request.Headers.Accept.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);
        if (acceptHeader.Contains("text/csv"))
        {
            req = req with { Skip = 0, Take = int.MaxValue };
        }
        var response = await _terminatedEmployeeAndBeneficiaryReportService.GetReportAsync(req, ct);
        if (acceptHeader.Contains("text/csv"))
        {
            await using var memoryStream = new MemoryStream();
            await using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
            await using (var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture) { Delimiter = "," }))
            {
                await GenerateCsvContent(csvWriter, response);
                await streamWriter.FlushAsync(ct);
            }
            memoryStream.Position = 0;
            await SendStreamAsync(memoryStream, $"{ReportFileName}.csv", contentType: "text/csv", cancellation: ct);
            return;
        }
        await SendOkAsync(response, ct);
    }

    public string ReportFileName { get; }

    private static async Task GenerateCsvContent(CsvWriter csvWriter, TerminatedEmployeeAndBeneficiaryResponse responseWithTotals)
    {
        // Write out totals
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("Amount In Profit Sharing");
        csvWriter.WriteField(responseWithTotals.TotalEndingBalance);
        csvWriter.WriteField("Vested Amount");
        csvWriter.WriteField(responseWithTotals.TotalVested);
        csvWriter.WriteField("Total Forfeitures");
        csvWriter.WriteField(responseWithTotals.TotalForfeit);
        csvWriter.WriteField("Total Beneficiary Allocations");
        csvWriter.WriteField(responseWithTotals.TotalBeneficiaryAllocation);

        await csvWriter.NextRecordAsync();
        await csvWriter.NextRecordAsync();

        // Write the headers for the flattened structure
        csvWriter.WriteField("BADGE_PSN");
        csvWriter.WriteField("NAME");
        csvWriter.WriteField("PROFIT_YEAR");
        csvWriter.WriteField("BEGINNING_BALANCE");
        csvWriter.WriteField("BENEFICIARY_ALLOCATION");
        csvWriter.WriteField("DISTRIBUTION_AMOUNT");
        csvWriter.WriteField("FORFEIT");
        csvWriter.WriteField("ENDING_BALANCE");
        csvWriter.WriteField("VESTED_BALANCE");
        csvWriter.WriteField("DATE_TERM");
        csvWriter.WriteField("YTD_PS_HOURS");
        csvWriter.WriteField("VESTED_PERCENT");
        csvWriter.WriteField("AGE");
        csvWriter.WriteField("ENROLLMENT_CODE");
        await csvWriter.NextRecordAsync();

        // Write each year detail as a row
        foreach (var employee in responseWithTotals.Response.Results)
        {
            foreach (var yd in employee.YearDetails)
            {
                csvWriter.WriteField(employee.BadgePSn);
                csvWriter.WriteField(employee.Name);
                csvWriter.WriteField(yd.ProfitYear);
                csvWriter.WriteField(yd.BeginningBalance);
                csvWriter.WriteField(yd.BeneficiaryAllocation);
                csvWriter.WriteField(yd.DistributionAmount);
                csvWriter.WriteField(yd.Forfeit);
                csvWriter.WriteField(yd.EndingBalance);
                csvWriter.WriteField(yd.VestedBalance);
                csvWriter.WriteField(yd.DateTerm);
                csvWriter.WriteField(yd.YtdPsHours);
                csvWriter.WriteField(yd.VestedPercent);
                csvWriter.WriteField(yd.Age);
                csvWriter.WriteField(yd.EnrollmentCode);
                await csvWriter.NextRecordAsync();
            }
        }
    }
}
