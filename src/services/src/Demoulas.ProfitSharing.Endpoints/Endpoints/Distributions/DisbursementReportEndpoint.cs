using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DisbursementReportEndpoint : ProfitSharingEndpoint<ProfitYearRequest, Results<Ok<PaginatedResponseDto<DisbursementReportDetailResponse>>, NotFound, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;

    public DisbursementReportEndpoint(IDistributionService distributionService) : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
    }

    public override void Configure()
    {
        Get("disbursement-report");
        Summary(s =>
        {
            s.Description = "Gets the disbursement report with distribution details by profit year.";
            s.Summary = "Disbursement report - QPAY078";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<DisbursementReportDetailResponse>()
                {
                    DisbursementReportDetailResponse.SampleResponse()
                }
            } };
        });
        Group<DistributionGroup>();
    }

    protected override async Task<Results<Ok<PaginatedResponseDto<DisbursementReportDetailResponse>>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        ProfitYearRequest req,
        CancellationToken ct)
    {
        var result = await _distributionService.GetDisbursementReport(req, ct);
        return result.ToHttpResult(Common.Contracts.Error.EntityNotFound("DisbursementReport"));
    }
}
