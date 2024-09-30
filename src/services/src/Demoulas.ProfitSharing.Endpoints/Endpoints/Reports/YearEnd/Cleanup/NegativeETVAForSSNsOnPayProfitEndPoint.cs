using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;

public class NegativeEtvaForSsNsOnPayProfitEndPoint : EndpointWithCsvBase<ProfitYearRequest, NegativeEtvaForSsNsOnPayProfitResponse, NegativeEtvaForSsNsOnPayProfitEndPoint.NegativeEtvaForSsNsOnPayProfitResponseMap>
{
    private readonly ICleanupReportService _reportService;

    public NegativeEtvaForSsNsOnPayProfitEndPoint(ICleanupReportService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
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
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return await _reportService.GetNegativeETVAForSSNsOnPayProfitResponse(req, ct);
    }

    public override string ReportFileName => "ETVA-LESS-THAN-ZERO";

    public sealed class NegativeEtvaForSsNsOnPayProfitResponseMap : ClassMap<NegativeEtvaForSsNsOnPayProfitResponse>
    {
        public NegativeEtvaForSsNsOnPayProfitResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.EmployeeBadge).Index(2).Name("BADGE");
            Map(m => m.EmployeeSsn).Index(3).Name("SSN");
            Map(m => m.EtvaValue).Index(4).Name("ETVA");
        }
    }
}
