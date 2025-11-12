using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployees;

public class TerminatedEmployeesEndPoint
    : EndpointWithCsvTotalsBase<FilterableStartAndEndDateRequest,
        TerminatedEmployeeAndBeneficiaryResponse,
        TerminatedEmployeeAndBeneficiaryDataResponseDto,
        TerminatedEmployeesEndPoint.TerminatedEmployeeCsvMap>
{
    private readonly ITerminatedEmployeeService _terminatedEmployeeService;
    private readonly IAuditService _auditService;
    private readonly ILogger<TerminatedEmployeesEndPoint> _logger;

    public TerminatedEmployeesEndPoint(
        ITerminatedEmployeeService terminatedEmployeeService,
        IAuditService auditService,
        ILogger<TerminatedEmployeesEndPoint> logger)
        : base(Navigation.Constants.Terminations)
    {
        _terminatedEmployeeService = terminatedEmployeeService;
        _auditService = auditService;
        _logger = logger;
    }

    public override string ReportFileName => "TerminatedEmployeeAndBeneficiaryReport";

    public override void Configure()
    {
        Post("/terminated-employees");
        Summary(s =>
        {
            s.Summary = "Get the Terminated Employees (QPAY066) report as JSON or CSV.";
            s.Description =
                "Returns a report of beneficiaries with a non-zero balance and employees who were terminated (and not retired) in the specified date range. " +
                "Optionally filter by vested balance using VestedBalanceValue and VestedBalanceOperator (Equals=0, LessThan=1, LessThanOrEqual=2, GreaterThan=3, GreaterThanOrEqual=4). " +
                "The endpoint supports both JSON and CSV output based on the Accept header. " +
                "Requires roles: ADMINISTRATOR or FINANCEMANAGER.";
            s.ExampleRequest = FilterableStartAndEndDateRequest.RequestExample();
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
            s.Responses[200] = "Success. Returns the report as JSON or CSV (if Accept: text/csv).";
            s.Responses[400] = "Bad Request. Invalid or missing parameters.";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[500] = "Internal Server Error. Unexpected error occurred.";
            // Parameter documentation is included in the description due to FastEndpoints limitations.
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<TerminatedEmployeeAndBeneficiaryResponse> GetResponse(FilterableStartAndEndDateRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var result = await _auditService.ArchiveCompletedReportAsync(ReportFileName,
                (short)req.EndingDate.Year,
                req,
                (archiveReq, _, cancellationToken) => _terminatedEmployeeService.GetReportAsync(archiveReq, cancellationToken),
                ct);

            // Record year-end terminated employees report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-terminated-employees-report"),
                new("endpoint", "TerminatedEmployeesEndPoint"),
                new("report_type", "terminated-employees"),
                new("date_range_days", (req.EndingDate.DayNumber - req.BeginningDate.DayNumber).ToString()));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "terminated-employees-and-beneficiaries"),
                new("endpoint", "TerminatedEmployeesEndPoint"));

            _logger.LogInformation("Year-end terminated employees report generated for date range {StartDate} to {EndDate}, returned {Count} records (correlation: {CorrelationId})",
                req.BeginningDate, req.EndingDate, resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
            }

            return result!;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, TerminatedEmployeeAndBeneficiaryResponse responseWithTotals, CancellationToken cancellationToken)
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
                csvWriter.WriteField(employee.PSN);
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
                csvWriter.WriteField(yd.HasForfeited);
                await csvWriter.NextRecordAsync();
            }
        }
    }

    public sealed class TerminatedEmployeeCsvMap : ClassMap<TerminatedEmployeeAndBeneficiaryDataResponseDto>
    {
        public TerminatedEmployeeCsvMap()
        {
            // This ClassMap is required by the base class but won't be used since we override GenerateCsvContent
            Map(m => m.PSN).Name("BADGE_PSN");
            Map(m => m.Name).Name("NAME");
        }
    }
}
