using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.Base;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
public class PayProfitBadgesNotInDemographicsEndpoint: EndpointWithCSVBase<PaginationRequestDto, PayProfitBadgesNotInDemographicsResponse, PayProfitBadgesNotInDemographicsEndpoint.PayProfitBadgesNotInDemographicsResponseMap>
{
    private readonly IYearEndService _yearEndService;

    public PayProfitBadgesNotInDemographicsEndpoint(IYearEndService yearEndService)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("payprofit-badges-without-demographics");
        Summary(s =>
        {
            s.Summary = "Payprofit Badges not in Demographics";
            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<PayProfitBadgesNotInDemographicsResponse>()
                        {
                            Results = new List<PayProfitBadgesNotInDemographicsResponse>
                            {
                                new PayProfitBadgesNotInDemographicsResponse { EmployeeBadge = 47425, EmployeeSsn = 900047425 }
                            }
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
    }

    public override string ReportFileName => "PAYPROFIT-WITOUT-DEMOGRAPHICS";

    public override async Task<ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>> GetResponse(PaginationRequestDto req, CancellationToken ct)
    {
        return await _yearEndService.GetPayProfitBadgesNotInDemographics(req, ct);
    }

    public sealed class PayProfitBadgesNotInDemographicsResponseMap : ClassMap<PayProfitBadgesNotInDemographicsResponse>
    {
        public PayProfitBadgesNotInDemographicsResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.EmployeeBadge).Index(2).Name("BADGE");
            Map(m => m.EmployeeSsn).Index(3).Name("SSN");
        }
    }
}
