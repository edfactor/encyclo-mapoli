using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;

public class NegativeEtvaForSsNsOnPayProfitEndPoint : EndpointWithCsvBase<ProfitYearRequest, NegativeEtvaForSsNsOnPayProfitResponse, NegativeEtvaForSsNsOnPayProfitEndPoint.NegativeEtvaForSsNsOnPayProfitResponseMap>
{
    private readonly INegativeEtvaReportService _reportService;

    public NegativeEtvaForSsNsOnPayProfitEndPoint(INegativeEtvaReportService reportService)
        : base(Navigation.Constants.NegativeETVA)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("negative-evta-ssn");
        Summary(s =>
        {
            s.Summary = "Negative ETVA for SSNs on PayProfit";
            s.Description = "ETVA = Early Termination Vested Amount";
            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<NegativeEtvaForSsNsOnPayProfitResponse>()
                        {
                            Results = new List<NegativeEtvaForSsNsOnPayProfitResponse>
                            {
                                new NegativeEtvaForSsNsOnPayProfitResponse { BadgeNumber = 47425, Ssn = "XXX-XX-7425", EtvaValue = -1293.43m }
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

    public override Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return _reportService.GetNegativeETVAForSsNsOnPayProfitResponseAsync(req, ct);
    }

    public override string ReportFileName => "ETVA-LESS-THAN-ZERO";

    public sealed class NegativeEtvaForSsNsOnPayProfitResponseMap : ClassMap<NegativeEtvaForSsNsOnPayProfitResponse>
    {
        public NegativeEtvaForSsNsOnPayProfitResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.Ssn).Index(3).Name("SSN");
            Map(m => m.EtvaValue).Index(4).Name("ETVA");
        }
    }
}
