using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using Demoulas.ProfitSharing.Common.Contracts.Response.ProfitDetails;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Corrections;

public sealed class GetProfitSharingAdjustmentsEndpoint : ProfitSharingEndpoint<GetProfitSharingAdjustmentsRequest, Results<Ok<GetProfitSharingAdjustmentsResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IProfitSharingAdjustmentsService _service;

    public GetProfitSharingAdjustmentsEndpoint(
        IProfitSharingAdjustmentsService service) : base(Navigation.Constants.ProfitSharingAdjustments)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("under21");
        Group<AdjustmentsGroup>();
        Validator<GetProfitSharingAdjustmentsRequestValidator>();

        Summary(s =>
        {
            s.Summary = "Get Profit Sharing Adjustments";
            s.ExampleRequest = GetProfitSharingAdjustmentsRequest.RequestExample();
            s.Description = "Loads Profit Detail rows for the Profit Sharing Adjustments screen (TPR008-22 parity).";
        });
    }

    protected override async Task<Results<Ok<GetProfitSharingAdjustmentsResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        GetProfitSharingAdjustmentsRequest req,
        CancellationToken ct)
    {
        var result = await _service.GetAdjustmentsAsync(req, ct);
        return result.ToHttpResult(Error.EmployeeNotFound);
    }
}
