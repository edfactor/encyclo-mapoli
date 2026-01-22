using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries.Inquiry;

public class BeneficiaryEndpoint : ProfitSharingEndpoint<BeneficiaryRequestDto, BeneficiaryResponse>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;
    private readonly ILogger<BeneficiaryEndpoint> _logger;

    public BeneficiaryEndpoint(IBeneficiaryInquiryService beneficiaryService, ILogger<BeneficiaryEndpoint> logger)
        : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("");
        Summary(m =>
        {
            m.Summary = "Get beneficiaries by PSN_SUFFIX & BADEGE_NUMBER";
            m.Description = "Pass psn_suffix and badge number and get beneficiaries.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new BeneficiaryResponse() } };
        });
        Group<BeneficiariesGroup>();
    }

    protected override Task<BeneficiaryResponse> HandleRequestAsync(BeneficiaryRequestDto req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _beneficiaryService.GetBeneficiaryAsync(req, ct);
            var response = result ?? new BeneficiaryResponse();

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "beneficiary-relationships"),
                new("endpoint", nameof(BeneficiaryEndpoint)));

            var count = response.Beneficiaries?.Total ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(count,
                new("record_type", "beneficiaries"),
                new("endpoint", nameof(BeneficiaryEndpoint)));

            return response;
        });
    }

}
