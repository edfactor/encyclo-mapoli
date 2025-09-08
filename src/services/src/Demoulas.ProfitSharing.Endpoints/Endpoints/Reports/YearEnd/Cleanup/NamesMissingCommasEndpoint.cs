using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;
public class NamesMissingCommasEndpoint : EndpointWithCsvBase<SortedPaginationRequestDto, NamesMissingCommaResponse, NamesMissingCommasEndpoint.NamesMissingCommasResponseMap>
{
    private readonly ICleanupReportService _cleanupReportService;

    public NamesMissingCommasEndpoint(ICleanupReportService cleanupReportService)
        : base(Navigation.Constants.CleanupReports)
    {
        _cleanupReportService = cleanupReportService;
    }

    public override void Configure()
    {
        Get("names-missing-commas");
        Summary(s =>
        {
            s.Summary = "Lists employees whose full names don't have commas";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<NamesMissingCommaResponse>
                    {
                        ReportDate = DateTime.Now,
                        ReportName = "MISSING COMMA IN PY_NAME",
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<NamesMissingCommaResponse>
                        {
                            Results = new List<NamesMissingCommaResponse>
                            {
                                new NamesMissingCommaResponse() {EmployeeName="Jane Doe", BadgeNumber=10010, Ssn = "XXXX-XX-1999", IsExecutive = false}
                            }
                        }
                    }
                }
            };
            s.ExampleRequest = SimpleExampleRequest;
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "NAMES-MISSING-COMMAS";

    public override Task<ReportResponseBase<NamesMissingCommaResponse>> GetResponse(SortedPaginationRequestDto req, CancellationToken ct)
    {
        return _cleanupReportService.GetNamesMissingCommaAsync(req, ct);
    }

    public sealed class NamesMissingCommasResponseMap : ClassMap<NamesMissingCommaResponse>
    {
        public NamesMissingCommasResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.Ssn).Index(3).Name("DEM SSN");
            Map(m => m.EmployeeName).Index(4).Name("Name");
        }
    }
}
