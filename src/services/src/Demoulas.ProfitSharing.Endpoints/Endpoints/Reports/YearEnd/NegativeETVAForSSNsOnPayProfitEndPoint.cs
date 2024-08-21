using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class NegativeETVAForSSNsOnPayProfitEndPoint : EndpointWithCSVBase<PaginationRequestDto, NegativeEtvaForSsNsOnPayProfitResponse, NegativeETVAForSSNsOnPayProfitEndPoint.NegativeETVAForSSNsOnPayProfitResponseMap>
{
    private readonly IYearEndService _reportService;

    public NegativeETVAForSSNsOnPayProfitEndPoint(IYearEndService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("negative-evta-ssn");
        Summary(s =>
        {
            s.Summary = "Negative ETVA for SSNs on PayProfit";
            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<NegativeEtvaForSsNsOnPayProfitResponse>()
                        {
                            Results = new List<NegativeEtvaForSsNsOnPayProfitResponse>
                            {
                                new NegativeEtvaForSsNsOnPayProfitResponse { EmployeeBadge = 47425, EmployeeSsn = 900047425, EtvaValue = -1293.43m }
                            }
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
        Options(x => x.CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5))));
    }

    public override async Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetResponse(PaginationRequestDto req, CancellationToken ct)
    {
        return await _reportService.GetNegativeETVAForSSNsOnPayProfitResponse(req, ct);
    }

    public override string ReportFileName => "ETVA-LESS-THAN-ZERO";

    public sealed class NegativeETVAForSSNsOnPayProfitResponseMap : ClassMap<NegativeEtvaForSsNsOnPayProfitResponse>
    {
        public NegativeETVAForSSNsOnPayProfitResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.EmployeeBadge).Index(2).Name("BADGE");
            Map(m => m.EmployeeSsn).Index(3).Name("SSN");
            Map(m => m.EtvaValue).Index(4).Name("ETVA");
        }
    }
}
