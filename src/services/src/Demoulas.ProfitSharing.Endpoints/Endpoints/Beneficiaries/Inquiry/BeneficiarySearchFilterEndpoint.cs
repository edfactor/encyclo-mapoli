using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries.Inquiry;

public class BeneficiarySearchFilterEndpoint : ProfitSharingEndpoint<BeneficiarySearchFilterRequest, PaginatedResponseDto<BeneficiarySearchFilterResponse>>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;
    private readonly ILogger<BeneficiarySearchFilterEndpoint> _logger;

    public BeneficiarySearchFilterEndpoint(IBeneficiaryInquiryService beneficiaryService, ILogger<BeneficiarySearchFilterEndpoint> logger)
        : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("search");
        Summary(m =>
        {
            m.Summary = "Get Member result based on beneficiary search Filter";
            m.Description = "It will search member based on Member Type i-e beneficiary or employee.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new PaginatedResponseDto<BeneficiarySearchFilterResponse>() } };
        });
        Group<BeneficiariesGroup>();
    }

    protected override Task<PaginatedResponseDto<BeneficiarySearchFilterResponse>> HandleRequestAsync(
        BeneficiarySearchFilterRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _beneficiaryService.BeneficiarySearchFilterAsync(req, ct);
            var response = result ?? new PaginatedResponseDto<BeneficiarySearchFilterResponse>();

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "beneficiary-search"),
                new("endpoint", nameof(BeneficiarySearchFilterEndpoint)));

            EndpointTelemetry.RecordCountsProcessed.Record(response.Total,
                new("record_type", "beneficiary-search-results"),
                new("endpoint", nameof(BeneficiarySearchFilterEndpoint)));

            return response;
        }, "Ssn");
    }

}
