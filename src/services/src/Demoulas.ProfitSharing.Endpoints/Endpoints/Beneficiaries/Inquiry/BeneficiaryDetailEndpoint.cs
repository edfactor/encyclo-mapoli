using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries.Inquiry;

public class BeneficiaryDetailEndpoint : ProfitSharingEndpoint<BeneficiaryDetailRequest, BeneficiaryDetailResponse>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;
    private readonly ILogger<BeneficiaryDetailEndpoint> _logger;

    public BeneficiaryDetailEndpoint(IBeneficiaryInquiryService beneficiaryService, ILogger<BeneficiaryDetailEndpoint> logger)
        : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("detail");
        Summary(m =>
        {
            m.Summary = "Get Beneficiary Detail ";
            m.Description = "It will return Beneficiary Detail depends on PSN.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new BeneficiaryDetailResponse() } };
        });
        Group<BeneficiariesGroup>();
    }

    protected override Task<BeneficiaryDetailResponse> HandleRequestAsync(BeneficiaryDetailRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _beneficiaryService.GetBeneficiaryDetailAsync(req, ct);

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "beneficiary-detail"),
                new("endpoint", nameof(BeneficiaryDetailEndpoint)));

            return response;
        });
    }

}
