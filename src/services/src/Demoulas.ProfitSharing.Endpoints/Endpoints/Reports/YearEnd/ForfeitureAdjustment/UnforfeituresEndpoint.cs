using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.TypeConverters;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;

public class UnforfeituresEndpoint :
    EndpointWithCsvBase<FilterableStartAndEndDateRequest, UnforfeituresResponse, UnforfeituresEndpoint.RehireProfitSharingResponseMap>
{
    private readonly IUnforfeitService _reportService;
    private readonly IAuditService _auditService;
    private readonly ILogger<UnforfeituresEndpoint> _logger;
    private readonly ICalendarService _calendarService;

    public UnforfeituresEndpoint(IUnforfeitService reportService, IAuditService auditService, ILogger<UnforfeituresEndpoint> logger, ICalendarService calendarService)
        : base(Navigation.Constants.Unforfeit)
    {
        _reportService = reportService;
        _auditService = auditService;
        _logger = logger;
        _calendarService = calendarService;
    }

    public override void Configure()
    {
        Post("unforfeitures");
        Summary(s =>
        {
            s.Summary = "Rehire Forfeiture Adjustments Endpoint";
            s.Description =
                "The Rehire Profit Sharing Data endpoint produces a comprehensive report on employees who are either currently on military leave or have been rehired, focusing on their eligibility for forfeiture adjustments in profit-sharing. The report contains employee information, such as badge number, rehire date, and current year-to-date hours, along with profit-sharing records, including profit year, forfeiture amounts, and comments. This report supports multiple executions and is primarily used to address forfeiture discrepancies before profit sharing is finalized.";

            s.ExampleRequest = StartAndEndDateRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<UnforfeituresResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<UnforfeituresResponse>
                        {
                            Results = new List<UnforfeituresResponse> { UnforfeituresResponse.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "REHIRE'S PROFIT SHARING DATA";

#pragma warning disable AsyncFixer01 // The method does use async/await inside ExecuteWithTelemetry lambda
    public override async Task<ReportResponseBase<UnforfeituresResponse>> GetResponse(FilterableStartAndEndDateRequest req, CancellationToken ct)
#pragma warning restore AsyncFixer01
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var mostRecentProfitYear = req.EndingDate.Year + 1; // Get the profit year containing the ending date.   
            DateOnly startDate;
            var iteration = 0;
            do
            {
                mostRecentProfitYear--;
                var calendarResult = await _calendarService.GetYearStartAndEndAccountingDatesAsync((short)mostRecentProfitYear);
                startDate = calendarResult.FiscalBeginDate;
            } while (iteration++ < 7 && startDate >= req.EndingDate);


            // Database queries and business logic are timed inside the service layer
            var result = await _auditService.ArchiveCompletedReportAsync(
                "Rehire Forfeiture Adjustments Endpoint",
                (short)mostRecentProfitYear,
                req,
                (archiveReq, _, cancellationToken) => _reportService.FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYearsAsync(archiveReq, cancellationToken),
                ct);

            // Record business operation for year-end unforfeitures report
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-unforfeitures-report"),
                new("endpoint", nameof(UnforfeituresEndpoint)),
                new("date_range", $"{req.BeginningDate:yyyy-MM-dd}_to_{req.EndingDate:yyyy-MM-dd}"));

            _logger.LogInformation(
                "Unforfeitures endpoint completed: {RecordCount} records for year {Year} (correlation: {CorrelationId})",
                result.Response?.Results?.Count() ?? 0,
                req.EndingDate.Year,
                HttpContext?.TraceIdentifier ?? "unknown");

            return result;
        }, "Ssn", "BadgeNumber"); // Declare sensitive fields accessed
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, ReportResponseBase<UnforfeituresResponse> report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<RehireProfitSharingResponseMap>();

        // Write the headers using the registered class map
        csvWriter.WriteHeader<UnforfeituresResponse>();

        // Add additional headers for the details section (Profit Year, Forfeitures, Comment)
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField(string.Empty);
        csvWriter.WriteField(string.Empty);
        csvWriter.WriteField(string.Empty);
        csvWriter.WriteField(string.Empty);
        csvWriter.WriteField(string.Empty);
        csvWriter.WriteField("YEAR");
        csvWriter.WriteField("FORFEITURES");
        csvWriter.WriteField("COMMENT");

        // Move to the next record to separate the headers from the data
        await csvWriter.NextRecordAsync();

        // Write the records (member + details)
        foreach (var member in report.Response.Results)
        {
            // Write the member details once
            csvWriter.WriteRecord(member);
            await csvWriter.NextRecordAsync();

            // Write each profit-detail record under the employee details
            foreach (var record in member.Details)
            {
                // These fields correspond to the empty fields for the member data
                csvWriter.WriteField(string.Empty); // Empty field for BadgeNumber
                csvWriter.WriteField(string.Empty); // Empty field for EmployeeName
                csvWriter.WriteField(string.Empty); // Empty field for SSN
                csvWriter.WriteField(string.Empty); // Empty field for ReHireDate
                csvWriter.WriteField(string.Empty); // Empty field for PY-YRS
                csvWriter.WriteField(string.Empty); // Empty field for YTD HOURS
                csvWriter.WriteField(record.ProfitYear);   // YEAR
                csvWriter.WriteField(record.Forfeiture);   // FORFEITURES
                csvWriter.WriteField(record.Remark);       // COMMENT
                await csvWriter.NextRecordAsync();
            }
        }
    }

    public sealed class RehireProfitSharingResponseMap : ClassMap<UnforfeituresResponse>
    {
        public RehireProfitSharingResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.FullName).Index(3).Name("EMPLOYEE NAME");
            Map(m => m.Ssn).Index(4).Name("SSN");
            Map(m => m.ReHiredDate).Index(5).Name("REHIRED").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.HireDate).Index(6).Name("HIRE DATE").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.NetBalanceLastYear).Index(9).Name("BEGINNING BALANCE").TypeConverterOption.Format("0.00");
            Map(m => m.VestedBalanceLastYear).Index(10).Name("BEGIN VESTED AMOUNT").TypeConverterOption.Format("0.00");

            Map().Index(13).Name("EC"); // Assuming EC is blank, leave an empty column
        }
    }
}

