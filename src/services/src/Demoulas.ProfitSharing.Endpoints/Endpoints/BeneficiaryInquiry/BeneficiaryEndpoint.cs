using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.BeneficiaryInquiry;

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

    public override Task<BeneficiaryResponse> ExecuteAsync(BeneficiaryRequestDto req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _beneficiaryService.GetBeneficiary(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "beneficiary-search"),
                new("endpoint", "BeneficiaryEndpoint"));

            var beneficiaryCount = response?.Beneficiaries?.Total ?? 0;
            var beneficiaryOfCount = response?.BeneficiaryOf?.Total ?? 0;
            var totalRecords = beneficiaryCount + beneficiaryOfCount;

            EndpointTelemetry.RecordCountsProcessed.Record(totalRecords,
                new("record_type", "beneficiaries"),
                new("endpoint", "BeneficiaryEndpoint"));

            _logger.LogInformation("Beneficiary search completed for Badge: {BadgeNumber}, PSN Suffix: {PsnSuffix}, returned {BeneficiaryCount} beneficiaries, {BeneficiaryOfCount} beneficiary-of records (total: {TotalCount}) (correlation: {CorrelationId})",
                req.BadgeNumber, req.PsnSuffix, beneficiaryCount, beneficiaryOfCount, totalRecords, HttpContext.TraceIdentifier);

            return response ?? new BeneficiaryResponse();
        });
    }

}
