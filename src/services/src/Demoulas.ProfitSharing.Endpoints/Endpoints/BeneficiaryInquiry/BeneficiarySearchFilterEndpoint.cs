using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.BeneficiaryInquiry;

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

    public override Task<PaginatedResponseDto<BeneficiarySearchFilterResponse>> ExecuteAsync(BeneficiarySearchFilterRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _beneficiaryService.BeneficiarySearchFilter(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "beneficiary-search-filter"),
                new("endpoint", "BeneficiarySearchFilterEndpoint"));

            var resultCount = response?.Total ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "beneficiary-search-results"),
                new("endpoint", "BeneficiarySearchFilterEndpoint"));

            _logger.LogInformation("Beneficiary search filter completed for Badge: {BadgeNumber}, PSN Suffix: {PsnSuffix}, Name: {Name}, MemberType: {MemberType}, returned {ResultCount} results (correlation: {CorrelationId})",
                req.BadgeNumber, req.PsnSuffix, req.Name, req.MemberType, resultCount, HttpContext.TraceIdentifier);

            return response ?? new PaginatedResponseDto<BeneficiarySearchFilterResponse>();
        }, "SSN");
    }

}
