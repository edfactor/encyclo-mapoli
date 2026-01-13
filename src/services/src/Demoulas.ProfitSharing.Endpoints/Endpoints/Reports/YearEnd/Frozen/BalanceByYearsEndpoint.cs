using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.BalanceByYearsEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class BalanceByYearsEndpoint : EndpointWithCsvTotalsBase<FrozenReportsByAgeRequest, BalanceByYears, BalanceByYearsDetail, ProfitSharingBalanceByYearsMapper>
{
    private readonly IFrozenReportService _frozenReportService;
    private readonly ILogger<BalanceByYearsEndpoint> _logger;

    public BalanceByYearsEndpoint(IFrozenReportService frozenReportService, ILogger<BalanceByYearsEndpoint> logger)
        : base(Navigation.Constants.BalanceByYears)
    {
        _frozenReportService = frozenReportService;
        _logger = logger;
    }

    public override string ReportFileName => "PROFIT SHARING BALANCE BY YEARS";

    public override void Configure()
    {
        Get("frozen/balance-by-years");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their balances over the year grouped by years";

            s.ExampleRequest = FrozenReportsByAgeRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> { { 200, BalanceByYears.ResponseExample() } };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<BalanceByYears> GetResponse(FrozenReportsByAgeRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _frozenReportService.GetBalanceByYearsAsync(req, ct);

            // Record year-end frozen balance by years report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-frozen-balance-by-years"),
                new("endpoint", "BalanceByYearsEndpoint"),
                new("report_type", "frozen"),
                new("frozen_report_type", "balance-by-years"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "frozen-balance-by-years"),
                new("endpoint", "BalanceByYearsEndpoint"));

            _logger.LogInformation("Year-end frozen balance by years report generated, returned {Count} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new BalanceByYears
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] },
                TotalMembers = 0,
                BalanceTotalAmount = 0,
                TotalBeneficiaries = 0
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

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, BalanceByYears report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<ProfitSharingBalanceByYearsMapper>();

        await base.GenerateCsvContent(csvWriter, report, cancellationToken);

        // Write out totals
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("BEN");
        csvWriter.WriteField(report.TotalBeneficiaries);
        csvWriter.WriteField(report.TotalBeneficiariesAmount);
        csvWriter.WriteField(report.TotalBeneficiariesVestedAmount);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("");
        csvWriter.WriteField(report.TotalEmployee);
        csvWriter.WriteField(report.TotalEmployeeAmount);
        csvWriter.WriteField(report.TotalEmployeesVestedAmount);


        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<ForfeituresByAgeDetail>();
        await csvWriter.NextRecordAsync();
    }


    public class ProfitSharingBalanceByYearsMapper : ClassMap<BalanceByYearsDetail>
    {
        public ProfitSharingBalanceByYearsMapper()
        {
            Map(m => m.Years).Index(0).Name("YRS");
            Map(m => m.EmployeeCount).Index(1).Name("EMPS");
            Map(m => m.CurrentBalance).Index(2).Name("BALANCE");
            Map(m => m.VestedBalance).Index(2).Name("VESTED");
        }
    }
}
