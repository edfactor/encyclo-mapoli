using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;

public sealed class BeneficiaryDisbursementEndpoint : ProfitSharingEndpoint<BeneficiaryDisbursementRequest, Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IBeneficiaryDisbursementService _beneficiaryDisbursementService;
    private readonly ILogger<BeneficiaryDisbursementEndpoint> _logger;

    public BeneficiaryDisbursementEndpoint(
        IBeneficiaryDisbursementService beneficiaryDisbursementService,
        ILogger<BeneficiaryDisbursementEndpoint> logger)
        : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryDisbursementService = beneficiaryDisbursementService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/disbursement");
        Summary(s =>
        {
            s.Summary = "Updates beneficiary information";
            s.ExampleRequest = BeneficiaryDisbursementRequest.SampleRequest();
        });
        Group<BeneficiariesGroup>();
    }

    protected override Task<Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        BeneficiaryDisbursementRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _beneficiaryDisbursementService.DisburseFundsToBeneficiaries(req, ct);

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "beneficiary-disbursement"),
                new("endpoint", nameof(BeneficiaryDisbursementEndpoint)));

            return result.ToHttpResultWithValidation();
        });
    }
}
