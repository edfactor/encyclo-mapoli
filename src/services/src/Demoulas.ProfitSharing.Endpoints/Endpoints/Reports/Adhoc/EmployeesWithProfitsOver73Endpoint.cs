using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

/// <summary>
/// PROF-LETTER73 - Adhoc report for employees with profits over age 73.
/// </summary>
public sealed class EmployeesWithProfitsOver73Endpoint 
    : EndpointWithCsvBase<EmployeesWithProfitsOver73Request, EmployeesWithProfitsOver73DetailDto, EmployeesWithProfitsOver73Endpoint.ProfLetter73Map>
{
    private readonly IEmployeesWithProfitsOver73Service _reportService;
    private readonly ILogger<EmployeesWithProfitsOver73Endpoint> _logger;

    public EmployeesWithProfitsOver73Endpoint(
        IEmployeesWithProfitsOver73Service reportService,
        ILogger<EmployeesWithProfitsOver73Endpoint> logger)
        : base(Navigation.Constants.AdhocProfLetter73, false)
    {
        _reportService = reportService;
        _logger = logger;
    }

    public override string ReportFileName => "PROF-LETTER73 - Employees with Profits Over Age 73";

    public override void Configure()
    {
        Get("prof-letter73");
        Summary(s =>
        {
            s.Summary = "PROF-LETTER73: Employees with Profits Over Age 73";
            s.Description = "Returns employees over age 73 who have profit sharing balances. Supports both JSON and CSV formats via Accept header.";
            s.ExampleRequest = EmployeesWithProfitsOver73Request.RequestExample();
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<AdhocReportsGroup>();
        base.Configure();
    }

    public override async Task<ReportResponseBase<EmployeesWithProfitsOver73DetailDto>> GetResponse(
        EmployeesWithProfitsOver73Request req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _reportService.GetEmployeesWithProfitsOver73Async(req, ct);

            // Record standardized business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "employees-profits-over-73-report"),
                new("endpoint", "EmployeesWithProfitsOver73Endpoint"),
                new("report_type", "prof-letter73"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "employees-over-73"),
                new("endpoint", "EmployeesWithProfitsOver73Endpoint"));

            _logger.LogInformation(
                "PROF-LETTER73 report generated for year {ProfitYear}, returned {Count} employees over 73 with balances (correlation: {CorrelationId})",
                req.ProfitYear, resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new ReportResponseBase<EmployeesWithProfitsOver73DetailDto>
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] }
            };

            this.RecordResponseMetrics(HttpContext, _logger, emptyResult);
            return emptyResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }

    public sealed class ProfLetter73Map : ClassMap<EmployeesWithProfitsOver73DetailDto>
    {
        ////Badge,Name,Address,City,State,Zip,Status,DOB,Age,Masked SSN, Term Date
        public ProfLetter73Map()
        {
            ////Badge,Name,Address,City,State,Zip,Status,DOB,Age,Masked SSN, Term Date
            Map(m => m.BadgeNumber).Index(0).Name("Badge");
            Map(m => m.Name).Index(1).Name("Name");
            Map(m => m.Address).Index(2).Name("Address");
            Map(m => m.City).Index(3).Name("City");
            Map(m => m.State).Index(4).Name("State");
            Map(m => m.Zip).Index(5).Name("Zip");
            Map(m => m.Status).Index(6).Name("Status")
                .Convert(args => !string.IsNullOrEmpty(args.Value.Status) ? args.Value.Status[0].ToString() : string.Empty);
            Map(m => m.DateOfBirth).Index(7).Name("DOB")
                .Convert(args => args.Value.DateOfBirth.ToString("yyyyMMdd"));
            Map(m => m.Age).Index(8).Name("Age");
            Map(m => m.Ssn).Index(9).Name("Masked SSN");
            Map(m => m.TerminationDate).Index(10).Name("Term Date")
                .Convert(args => args.Value.TerminationDate?.ToString("yyyyMMdd") ?? string.Empty);     

        }
    }
}
