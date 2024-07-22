using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class NegativeETVAForSSNsOnPayProfitEndPoint : EndpointWithCSVBase<EmptyRequest, NegativeETVAForSSNsOnPayProfitResponse, NegativeETVAForSSNsOnPayProfitEndPoint.NegativeETVAForSSNsOnPayProfitResponseMap>
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
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>
                    {
                        ReportName = "NEGATIVE ETVA FOR SSNs ON PAYPROFIT",
                        ReportDate = DateTimeOffset.Now,
                        Results = new HashSet<NegativeETVAForSSNsOnPayProfitResponse>
                        {
                            new NegativeETVAForSSNsOnPayProfitResponse { EmployeeBadge = 47425, EmployeeSSN = 900047425, EtvaValue = -1293.43m },
                            new NegativeETVAForSSNsOnPayProfitResponse { EmployeeBadge = 82424, EmployeeSSN = 900082424, EtvaValue = -1152.33m },
                            new NegativeETVAForSSNsOnPayProfitResponse { EmployeeBadge = 85744, EmployeeSSN = 900085744, EtvaValue = -2899.78m },
                            new NegativeETVAForSSNsOnPayProfitResponse { EmployeeBadge = 94861, EmployeeSSN = 900094861, EtvaValue = -1200.00m }
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
        Options(x => x.CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5))));
    }

    public override async Task<ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>> GetResponse(CancellationToken ct)
    {
        return await _reportService.GetNegativeETVAForSSNsOnPayProfitResponse(ct);
    }

    public override string ReportFileName => "ETVA-LESS-THAN-ZERO";

    public sealed class NegativeETVAForSSNsOnPayProfitResponseMap : ClassMap<NegativeETVAForSSNsOnPayProfitResponse>
    {
        public NegativeETVAForSSNsOnPayProfitResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.EmployeeBadge).Index(2).Name("BADGE");
            Map(m => m.EmployeeSSN).Index(3).Name("SSN");
            Map(m => m.EtvaValue).Index(4).Name("ETVA");
        }
    }
}
