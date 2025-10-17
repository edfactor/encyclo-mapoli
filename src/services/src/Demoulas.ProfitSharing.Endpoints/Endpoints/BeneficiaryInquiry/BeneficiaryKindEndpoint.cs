using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.BeneficiaryInquiry;

public class BeneficiaryKindEndpoint : ProfitSharingEndpoint<BeneficiaryKindRequestDto, BeneficiaryKindResponseDto>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;
    private readonly ILogger<BeneficiaryKindEndpoint> _logger;

    public BeneficiaryKindEndpoint(IBeneficiaryInquiryService beneficiaryService, ILogger<BeneficiaryKindEndpoint> logger)
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
            m.Summary = "Get beneficiaries kind list";
            m.Description = "It will provide you list of Beneficiary kind";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new BeneficiaryKindResponseDto() { } } };
        });
        Group<BeneficiaryKindGroup>();
    }

    public override async Task<BeneficiaryKindResponseDto> ExecuteAsync(BeneficiaryKindRequestDto req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var response = await _beneficiaryService.GetBeneficiaryKind(ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "beneficiary-kind-lookup"),
                new("endpoint", "BeneficiaryKindEndpoint"));

            var kindCount = response?.BeneficiaryKindList?.Count ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(kindCount,
                new("record_type", "beneficiary-kinds"),
                new("endpoint", "BeneficiaryKindEndpoint"));

            _logger.LogInformation("Beneficiary kind lookup completed, returned {KindCount} kinds (correlation: {CorrelationId})",
                kindCount, HttpContext.TraceIdentifier);

            var safeResponse = response ?? new BeneficiaryKindResponseDto();
            this.RecordResponseMetrics(HttpContext, _logger, safeResponse);

            return safeResponse;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }

}
