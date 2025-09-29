using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class DuplicateSsnExistsEndpoint : ProfitSharingResponseEndpoint<bool>
{
    private readonly IPayrollDuplicateSsnReportService _duplicateSsnReportService;
    private readonly ILogger<DuplicateSsnExistsEndpoint> _logger;

    public DuplicateSsnExistsEndpoint(IPayrollDuplicateSsnReportService duplicateSsnReportService, ILogger<DuplicateSsnExistsEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _duplicateSsnReportService = duplicateSsnReportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("duplicate-ssns/exists");
        Summary(s =>
        {
            s.Summary = "Returns true when there are duplicate SSNs present in demographics";
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, true }
            };
        });
        Group<LookupGroup>();
    }

    public override Task<bool> ExecuteAsync(CancellationToken ct) =>
        this.ExecuteWithTelemetry(HttpContext, _logger, new { }, async () =>
        {
            var duplicateExists = await _duplicateSsnReportService.DuplicateSsnExistsAsync(ct);

            // Record data quality check metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "duplicate-ssn-check"),
                new("endpoint", "DuplicateSsnExistsEndpoint"),
                new("has_duplicates", duplicateExists.ToString().ToLowerInvariant()));

            _logger.LogInformation("Duplicate SSN check completed, duplicates exist: {DuplicatesExist} (correlation: {CorrelationId})",
                duplicateExists, HttpContext.TraceIdentifier);

            // Log warning if duplicates are found as this indicates data quality issues
            if (duplicateExists)
            {
                _logger.LogWarning("Duplicate SSNs detected in demographics data (correlation: {CorrelationId})",
                    HttpContext.TraceIdentifier);
            }

            return duplicateExists;
        });
}
