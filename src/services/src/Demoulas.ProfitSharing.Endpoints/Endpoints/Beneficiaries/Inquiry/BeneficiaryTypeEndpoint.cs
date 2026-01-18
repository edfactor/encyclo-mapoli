using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.BeneficiaryInquiry;

public class BeneficiaryTypeEndpoint : ProfitSharingEndpoint<BeneficiaryTypesRequestDto, BeneficiaryTypesResponseDto>
{

    private readonly IBeneficiaryInquiryService _beneficiaryService;
    private readonly ILogger<BeneficiaryTypeEndpoint> _logger;

    public BeneficiaryTypeEndpoint(IBeneficiaryInquiryService beneficiaryService, ILogger<BeneficiaryTypeEndpoint> logger) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("");
        Summary(m =>
        {
            m.Summary = "Get beneficiaries type list";
            m.Description = "It will provide you list of Beneficiary Types";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new PaginatedResponseDto<BeneficiaryDto>() } };
        });
        Group<BeneficiaryTypeGroup>();
    }

    protected override async Task<BeneficiaryTypesResponseDto> HandleRequestAsync(BeneficiaryTypesRequestDto req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var response = await _beneficiaryService.GetBeneficiaryTypesAsync(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "beneficiary-type-lookup"),
                new("endpoint", "BeneficiaryTypeEndpoint"));

            var typeCount = response?.BeneficiaryTypeList?.Count ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(typeCount,
                new("record_type", "beneficiary-types"),
                new("endpoint", "BeneficiaryTypeEndpoint"));

            _logger.LogInformation("Beneficiary type lookup completed, returned {TypeCount} types (correlation: {CorrelationId})",
                typeCount, HttpContext.TraceIdentifier);

            var safeResponse = response ?? new BeneficiaryTypesResponseDto();
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
