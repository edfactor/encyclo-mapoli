using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.TypeConverters;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;

public class DuplicateNamesAndBirthdaysEndpoint : EndpointWithCsvBase<DuplicateNamesAndBirthdaysRequest, DuplicateNamesAndBirthdaysResponse, DuplicateNamesAndBirthdaysEndpoint.DuplicateNamesAndBirthdaysResponseMap>
{
    private readonly IDuplicateNamesAndBirthdaysService _duplicateNamesAndBirthdaysService;
    private readonly ILogger<DuplicateNamesAndBirthdaysEndpoint> _logger;

    public DuplicateNamesAndBirthdaysEndpoint(
        IDuplicateNamesAndBirthdaysService duplicateNamesAndBirthdaysService,
        ILogger<DuplicateNamesAndBirthdaysEndpoint> logger)
        : base(Navigation.Constants.DuplicateNamesAndBirthdays)
    {
        _duplicateNamesAndBirthdaysService = duplicateNamesAndBirthdaysService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("duplicate-names-and-birthdays");
        Summary(s =>
        {
            s.Summary = "List of duplicate names, and birthdays in the demographics area";
            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>
                    {
                        ReportDate = DateTime.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        ReportName = "DUPLICATE NAMES AND BIRTHDAYS",
                        Response = new PaginatedResponseDto<DuplicateNamesAndBirthdaysResponse>
                        {
                            Results = new List<DuplicateNamesAndBirthdaysResponse>
                            {
                                new DuplicateNamesAndBirthdaysResponse
                                {
                                    State = "MA",
                                        PostalCode = "01876",
                                        City = "Tewksbury",
                                        Address = "1900 Main St",
                                        CountryIso="US",
                                    BadgeNumber = 100110,
                                    Count = 2,
                                    IncomeCurrentYear = 23003,
                                    DateOfBirth = new DateOnly(1990,7,30),
                                    HireDate = new DateOnly(2015,9,14),
                                    HoursCurrentYear = 1524,
                                    Name = "Henry Rollins",
                                    NetBalance = 52500,
                                    Ssn = "XXX-XX-1234",
                                    Status = 'A',
                                    EmploymentStatusName = "Active",
                                    StoreNumber = 22,
                                    Years = 3,
                                    IsExecutive = false,
                                }
                            }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "duplicate-names-and-birthdays";

    public override async Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetResponse(DuplicateNamesAndBirthdaysRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            _logger.LogInformation("Fetching duplicate names and birthdays data (ProfitYear: {ProfitYear})",
                req.ProfitYear);

            // Get cached data with filtering applied by service
            var cachedResponse = await _duplicateNamesAndBirthdaysService.GetCachedDuplicateNamesAndBirthdaysAsync(req, ct);

            ReportResponseBase<DuplicateNamesAndBirthdaysResponse> reportResult;

            if (cachedResponse != null)
            {
                _logger.LogInformation("Using cached duplicate names and birthdays data (AsOfDate: {AsOfDate})",
                    cachedResponse.AsOfDate);

                // Convert cached response to ReportResponseBase
                reportResult = new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>
                {
                    ReportName = ReportFileName,
                    ReportDate = cachedResponse.AsOfDate,
                    StartDate = DateOnly.FromDateTime(cachedResponse.AsOfDate.DateTime),
                    EndDate = DateOnly.FromDateTime(cachedResponse.AsOfDate.DateTime),
                    Response = cachedResponse.Data
                };

                // Record year-end cleanup report metrics
                EndpointTelemetry.BusinessOperationsTotal.Add(1,
                    new("operation", "year-end-cleanup-duplicate-names-birthdays"),
                    new("endpoint", "DuplicateNamesAndBirthdaysEndpoint"),
                    new("report_type", "cleanup"),
                    new("cleanup_type", "duplicate-names-and-birthdays"),
                    new("data_source", "cache"));
            }
            else
            {
                // Return empty result if cache is not available
                _logger.LogWarning("Cache not available, returning empty result. Cache will be populated by background service.");

                reportResult = new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>
                {
                    ReportName = ReportFileName,
                    StartDate = DateOnly.FromDateTime(DateTime.Today),
                    EndDate = DateOnly.FromDateTime(DateTime.Today),
                    Response = new() { Results = [] }
                };

                // Record cache miss
                EndpointTelemetry.BusinessOperationsTotal.Add(1,
                    new("operation", "year-end-cleanup-duplicate-names-birthdays"),
                    new("endpoint", "DuplicateNamesAndBirthdaysEndpoint"),
                    new("report_type", "cleanup"),
                    new("cleanup_type", "duplicate-names-and-birthdays"),
                    new("data_source", "cache-miss"));
            }

            var resultCount = reportResult.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "duplicate-names-birthdays-cleanup"),
                new("endpoint", "DuplicateNamesAndBirthdaysEndpoint"));

            _logger.LogInformation("Year-end cleanup report for duplicate names and birthdays returned {Count} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            this.RecordResponseMetrics(HttpContext, _logger, reportResult);
            return reportResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }

    public sealed class DuplicateNamesAndBirthdaysResponseMap : ClassMap<DuplicateNamesAndBirthdaysResponse>
    {
        public DuplicateNamesAndBirthdaysResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.Ssn).Index(3).Name("SSN");
            Map(m => m.Name).Index(4).Name("NAME");
            Map(m => m.DateOfBirth).Index(5).Name("DOB").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.Address).Index(6).Name("ADDRESS");
            Map(m => m.City).Index(7).Name("CITY");
            Map(m => m.State).Index(8).Name("ST");
            Map(m => m.Years).Index(9).Name("YRS");
            Map(m => m.HireDate).Index(10).Name("HIRE").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.TerminationDate).Index(11).Name("TERM").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.Status).Index(12).Name("ST");
            Map(m => m.StoreNumber).Index(13).Name("STORE");
            Map(m => m.Count).Index(14).Name("PS#");
            Map(m => m.NetBalance).Index(15).Name("PSBAL");
            Map(m => m.HoursCurrentYear).Index(16).Name("CUR HURS");
            Map(m => m.IncomeCurrentYear).Index(17).Name("CUR WAGE");
        }
    }
}
