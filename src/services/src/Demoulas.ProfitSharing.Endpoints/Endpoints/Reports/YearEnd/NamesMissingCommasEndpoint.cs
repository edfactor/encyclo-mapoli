using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Demoulas.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
public class NamesMissingCommasEndpoint : EndpointWithCSVBase<PaginationRequestDto, NamesMissingCommaResponse, NamesMissingCommasEndpoint.NamesMissingCommasResponseMap>
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
        });
        Group<YearEndGroup>();
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
            Map(m => m.EmployeeSSN).Index(3).Name("DEM SSN");
            Map(m => m.EmployeeName).Index(4).Name("Name");
        }
    }
}
