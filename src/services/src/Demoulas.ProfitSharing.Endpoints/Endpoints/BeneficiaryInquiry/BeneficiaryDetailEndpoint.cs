using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.BeneficiaryInquiry;

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

    public override Task<BeneficiaryDetailResponse> ExecuteAsync(BeneficiaryDetailRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _beneficiaryService.GetBeneficiaryDetail(req, ct);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "beneficiary-detail-inquiry"),
                new KeyValuePair<string, object?>("endpoint.category", "beneficiary-inquiry"));

            _logger.LogInformation("Beneficiary detail inquiry completed for Badge: {BadgeNumber}, PSN Suffix: {PsnSuffix} (correlation: {CorrelationId})",
                req.BadgeNumber, req.PsnSuffix, HttpContext.TraceIdentifier);

            return response;
        });
    }

}
