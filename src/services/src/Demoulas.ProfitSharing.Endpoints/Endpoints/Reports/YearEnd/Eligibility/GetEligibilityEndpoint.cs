using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Eligibility;

public class GetEligibleEmployeesEndpoint : EndpointWithCsvTotalsBase<ProfitYearRequest, GetEligibleEmployeesResponse, EligibleEmployee, GetEligibleEmployeesEndpoint.GetEligibleEmployeesResponseDtoMap>
{
    private readonly IGetEligibleEmployeesService _getEligibleEmployeesService;
    private readonly ILogger<GetEligibleEmployeesEndpoint> _logger;
    public override string ReportFileName { get; } = "GetEligibleEmployeesReport.csv";

    public GetEligibleEmployeesEndpoint(
        IGetEligibleEmployeesService getEligibleEmployeesService,
        ILogger<GetEligibleEmployeesEndpoint> logger)
        : base(Navigation.Constants.GetEligibleEmployees)
    {
        _getEligibleEmployeesService = getEligibleEmployeesService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/eligible-employees");
        Summary(s =>
        {
            s.Summary = "Provide the Eligible Employees report.";
            s.Description =
                "Reports on employees eligible to participate in profit sharing based on several factors considered at fiscal end of year.";

            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<GetEligibleEmployeesResponse> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _getEligibleEmployeesService.GetEligibleEmployeesAsync(req, ct);

            // Record year-end eligible employees report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-eligible-employees-report"),
                new("endpoint", "GetEligibleEmployeesEndpoint"),
                new("report_type", "eligibility"),
                new("profit_year", req.ProfitYear.ToString()));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "eligible-employees"),
                new("endpoint", "GetEligibleEmployeesEndpoint"));

            _logger.LogInformation("Year-end eligible employees report generated for year {ProfitYear}, returned {Count} employees (correlation: {CorrelationId})",
                req.ProfitYear, resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new GetEligibleEmployeesResponse
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] },
                NumberReadOnFrozen = 0,
                NumberNotSelected = 0,
                NumberWritten = 0
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

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, GetEligibleEmployeesResponse responseWithTotals, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<GetEligibleEmployeesResponseDtoMap>();

        // Write out totals
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("Number read on FROZEN");
        csvWriter.WriteField(responseWithTotals.NumberReadOnFrozen);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("Number not selected");
        csvWriter.WriteField(responseWithTotals.NumberNotSelected);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("Number written");
        csvWriter.WriteField(responseWithTotals.NumberWritten);

        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<EligibleEmployee>();
        await csvWriter.NextRecordAsync();

        await base.GenerateCsvContent(csvWriter, responseWithTotals, cancellationToken);
    }

    public sealed class GetEligibleEmployeesResponseDtoMap : ClassMap<EligibleEmployee>
    {
        public GetEligibleEmployeesResponseDtoMap()
        {
            Map(m => m.DepartmentId).Index(1).Name("ASSIGNMENT_ID");
            Map(m => m.BadgeNumber).Index(2).Name("BADGE_PSN");
            Map(m => m.FullName).Index(3).Name("NAME");
        }

    }
}
