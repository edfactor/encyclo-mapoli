using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

public sealed class TerminatedEmployeesNeedingFormLetterEndpoint : EndpointWithCsvBase<FilterableStartAndEndDateRequest, AdhocTerminatedEmployeeResponse, TerminatedEmployeesNeedingFormLetterEndpoint.EndpointMap>
{
    private readonly IAdhocTerminatedEmployeesService _adhocTerminatedEmployeesService;
    private readonly ILogger<TerminatedEmployeesNeedingFormLetterEndpoint> _logger;

    public TerminatedEmployeesNeedingFormLetterEndpoint(IAdhocTerminatedEmployeesService adhocTerminatedEmployeesService, ILogger<TerminatedEmployeesNeedingFormLetterEndpoint> logger) : base(Navigation.Constants.Unknown) //TBD
    {
        _adhocTerminatedEmployeesService = adhocTerminatedEmployeesService;
        _logger = logger;
    }
    public override string ReportFileName => "Adhoc Terminated Employees Report needing Form letter";

    public override void Configure()
    {
        Get("terminated-employees-report-needing-letter");
        Summary(s =>
        {
            s.Summary = "Adhoc Terminated Employees Report needing a form letter";
            s.Description = "Returns a report of terminated employees who have not yet been sent a form letter to accompany a set of forms to receive vested interest .";
            s.ExampleRequest = new StartAndEndDateRequest
            {
                Skip = 0,
                Take = 100,
                SortBy = "TerminationDate",
                IsSortDescending = false,
                BeginningDate = new DateOnly(2023, 1, 1),
                EndingDate = new DateOnly(2025, 12, 31),
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<AdhocTerminatedEmployeeResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.MinValue,
                        EndDate = DateOnly.MaxValue,
                        Response = new PaginatedResponseDto<AdhocTerminatedEmployeeResponse>
                        {
                            Results = new List<AdhocTerminatedEmployeeResponse>
                            {
                                new AdhocTerminatedEmployeeResponse
                                {
                                    BadgeNumber = 12345,
                                    FullName = "John Doe",
                                    Ssn = "123-45-6789".MaskSsn(),
                                    TerminationDate = new DateOnly(2023, 5, 15),
                                    TerminationCodeId = 'A',
                                    TerminationCode = "Active",
                                    Address = "123 Main St",
                                    State = "MA",
                                    City = "Andover",
                                    PostalCode = "01810",
                                }
                            },
                            Total = 1
                        }
                    }
                }
            };
        });
        Group<AdhocReportsGroup>();
        base.Configure();
    }

    public override async Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetResponse(FilterableStartAndEndDateRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // This endpoint accesses SSN data, so mark as sensitive
            this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");

            var result = await _adhocTerminatedEmployeesService.GetTerminatedEmployeesNeedingFormLetter(req, ct);

            // Record terminated employees form letter metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "terminated-employees-form-letter"),
                new("endpoint", "TerminatedEmployeesNeedingFormLetterEndpoint"),
                new("report_type", "form-letter"),
                new("employee_status", "terminated"),
                new("letter_status", "needed"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "terminated-employees-letter"),
                new("endpoint", "TerminatedEmployeesNeedingFormLetterEndpoint"));

            _logger.LogInformation("Terminated employees needing form letter report generated, returned {Count} employees (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new ReportResponseBase<AdhocTerminatedEmployeeResponse>
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

    public class EndpointMap : ClassMap<AdhocTerminatedEmployeeResponse>
    {
        public EndpointMap()
        {
            Map(m => m.BadgeNumber).Name("Badge Number");
            Map(m => m.FullName).Name("Full Name");
            Map(m => m.Ssn).Name("SSN");
            Map(m => m.TerminationDate).Name("Termination Date");
            Map(m => m.TerminationCodeId).Name("Termination Code ID");
            Map(m => m.Address).Name("Address");
            Map(m => m.Address2).Name("Address 2");
            Map(m => m.City).Name("City");
            Map(m => m.State).Name("State");
            Map(m => m.PostalCode).Name("Postal Code");
        }
    }
}
