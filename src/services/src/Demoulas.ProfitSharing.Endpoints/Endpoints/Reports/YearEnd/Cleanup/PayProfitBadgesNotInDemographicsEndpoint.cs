using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;

#pragma warning disable S1133
[Obsolete("Not possible in new schema")]
#pragma warning restore S1133
public class PayProfitBadgesNotInDemographicsEndpoint : EndpointWithCsvBase<ProfitYearRequest, PayProfitBadgesNotInDemographicsResponse, PayProfitBadgesNotInDemographicsEndpoint.PayProfitBadgesNotInDemographicsResponseMap>
{
    public PayProfitBadgesNotInDemographicsEndpoint()
    {
    
    }

    public override void Configure()
    {
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
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "PAYPROFIT-WITOUT-DEMOGRAPHICS";

    public override Task<ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        throw new NotImplementedException();
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
