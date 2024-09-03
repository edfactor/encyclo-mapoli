using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
public class NamesMissingCommasEndpoint : EndpointWithCsvBase<PaginationRequestDto, NamesMissingCommaResponse, NamesMissingCommasEndpoint.NamesMissingCommasResponseMap>
{
    private readonly IYearEndService _yearEndService;

    public NamesMissingCommasEndpoint(IYearEndService yearEndService)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        AllowAnonymous();
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
                        Response = new PaginatedResponseDto<NamesMissingCommaResponse>
                        {
                            Results = new List<NamesMissingCommaResponse>
                            {
                                new NamesMissingCommaResponse() {EmployeeName="Jane Doe", EmployeeBadge=10010, EmployeeSsn = 999999999}
                            }
                        }
                    }
                }
            };
            s.ExampleRequest = SimpleExampleRequest;
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "NAMES-MISSING-COMMAS";

    public override async Task<ReportResponseBase<NamesMissingCommaResponse>> GetResponse(PaginationRequestDto req, CancellationToken ct)
    {
        return await _yearEndService.GetNamesMissingComma(req, ct);
    }

    public sealed class NamesMissingCommasResponseMap : ClassMap<NamesMissingCommaResponse>
    {
        public NamesMissingCommasResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.EmployeeBadge).Index(2).Name("BADGE");
            Map(m => m.EmployeeSsn).Index(3).Name("DEM SSN");
            Map(m => m.EmployeeName).Index(4).Name("Name");
        }
    }
}
