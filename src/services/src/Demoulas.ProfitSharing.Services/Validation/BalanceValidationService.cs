using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Validation;

/// <summary>
/// Service for validating balance integrity rules per the Balance Reports Cross-Reference Matrix.
/// Implements Rule 2: ALLOC + PAID ALLOC = 0 validation.
/// </summary>
public sealed class BalanceValidationService : IBalanceValidationService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<BalanceValidationService> _logger;

    public BalanceValidationService(
        IProfitSharingDataContextFactory dataContextFactory,
        ILogger<BalanceValidationService> logger)
    {
        _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<Result<CrossReferenceValidationGroup>> ValidateAllocTransfersAsync(
        short profitYear,
        CancellationToken cancellationToken = default)
    {
        var validations = new List<CrossReferenceValidation>();

        try
        {
            _logger.LogInformation("Starting ALLOC/PAID ALLOC transfer validation for year {ProfitYear}", profitYear);

            // Query ALLOC (code 6) and PAID ALLOC (code 5) totals
            var allocTotals = await _dataContextFactory.UseReadOnlyContext(async ctx =>
                await ctx.ProfitDetails
                    .TagWith($"AllocTransferValidation-{profitYear}")
                    .Where(pd => pd.ProfitYear == profitYear &&
                                 (pd.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id ||
                                  pd.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id))
                    .GroupBy(pd => pd.ProfitCodeId)
                    .Select(g => new
                    {
                        CodeId = g.Key,
                        Total = g.Sum(pd => pd.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id
                            ? pd.Contribution
                            : pd.Forfeiture)
                    })
                    .ToListAsync(cancellationToken), cancellationToken);

            // Extract totals
            decimal allocTotal = allocTotals
                .FirstOrDefault(x => x.CodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id)?.Total ?? 0m;
            decimal paidAllocTotal = allocTotals
                .FirstOrDefault(x => x.CodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id)?.Total ?? 0m;

            // Calculate net transfer (should be zero)
            decimal netTransfer = allocTotal + paidAllocTotal;
            bool isValid = netTransfer == 0m;

            _logger.LogInformation(
                "ALLOC transfer validation for year {ProfitYear}: ALLOC={AllocTotal:C}, PAID ALLOC={PaidAllocTotal:C}, Net={NetTransfer:C}, Valid={IsValid}",
                profitYear, allocTotal, paidAllocTotal, netTransfer, isValid);

            // Create validation result for ALLOC
            validations.Add(new CrossReferenceValidation
            {
                FieldName = "IncomingAllocations",
                ReportCode = "PAY444",
                CurrentValue = allocTotal,
                ExpectedValue = null, // Not archived separately
                IsValid = true, // Individual values are informational
                Message = $"ALLOC (code 6) total: {allocTotal:C}",
                Notes = "Incoming QDRO beneficiary allocations (ProfitCodeId=6, Contribution field)"
            });

            // Create validation result for PAID ALLOC
            validations.Add(new CrossReferenceValidation
            {
                FieldName = "OutgoingAllocations",
                ReportCode = "PAY444",
                CurrentValue = paidAllocTotal,
                ExpectedValue = null, // Not archived separately
                IsValid = true, // Individual values are informational
                Message = $"PAID ALLOC (code 5) total: {paidAllocTotal:C}",
                Notes = "Outgoing XFER beneficiary allocations (ProfitCodeId=5, Forfeiture field)"
            });

            // Create validation result for net transfer
            validations.Add(new CrossReferenceValidation
            {
                FieldName = "NetAllocTransfer",
                ReportCode = "PAY444",
                CurrentValue = netTransfer,
                ExpectedValue = 0m,
                Variance = netTransfer,
                IsValid = isValid,
                Message = isValid
                    ? $"✅ Net transfer balances to zero: {allocTotal:C} + {paidAllocTotal:C} = {netTransfer:C}"
                    : $"⚠️ Transfer imbalance: {allocTotal:C} + {paidAllocTotal:C} = {netTransfer:C} (expected 0)",
                Notes = isValid ? "Transfers balance correctly" : "CRITICAL: Transfers do not balance"
            });

            string summary = isValid
                ? $"✅ ALLOC transfers balance to zero for year {profitYear} (ALLOC: {allocTotal:C}, PAID ALLOC: {paidAllocTotal:C})"
                : $"⚠️ ALLOC transfer IMBALANCE for year {profitYear}: Net difference is {netTransfer:C}";

            // Record telemetry
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "alloc-transfer-validation"),
                new KeyValuePair<string, object?>("endpoint.category", "balance-validation"),
                new KeyValuePair<string, object?>("profit_year", profitYear),
                new KeyValuePair<string, object?>("validation_result", isValid ? "pass" : "fail"));

            if (!isValid)
            {
                // Track imbalance amount for monitoring
                EndpointTelemetry.RecordCountsProcessed.Record((long)Math.Abs(netTransfer * 100), // Convert to cents for histogram
                    new KeyValuePair<string, object?>("record_type", "alloc-transfer-imbalance"),
                    new KeyValuePair<string, object?>("profit_year", profitYear));

                _logger.LogWarning(
                    "CRITICAL: ALLOC transfer imbalance detected for year {ProfitYear}: {NetTransfer:C}",
                    profitYear, netTransfer);
            }

            var result = new CrossReferenceValidationGroup
            {
                GroupName = "ALLOC/PAID ALLOC Transfers",
                Description = $"Validates that ALLOC (code 6) and PAID ALLOC (code 5) transfers sum to zero for year {profitYear}",
                IsValid = isValid,
                Validations = validations,
                Summary = summary,
                Priority = "Critical",
                ValidationRule = "Sum(ALLOC) + Sum(PAID ALLOC) must equal 0 (Balance Matrix Rule 2)"
            };

            return Result<CrossReferenceValidationGroup>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating ALLOC transfers for year {ProfitYear}", profitYear);

            // Record failure telemetry
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "alloc-transfer-validation"),
                new KeyValuePair<string, object?>("endpoint.category", "balance-validation"),
                new KeyValuePair<string, object?>("profit_year", profitYear),
                new KeyValuePair<string, object?>("validation_result", "error"));

            validations.Add(new CrossReferenceValidation
            {
                FieldName = "NetAllocTransfer",
                ReportCode = "PAY444",
                CurrentValue = null,
                ExpectedValue = 0m,
                IsValid = false,
                Message = $"Exception during ALLOC transfer validation: {ex.Message}",
                Notes = "Error occurred during database query or calculation"
            });

            var errorResult = new CrossReferenceValidationGroup
            {
                GroupName = "ALLOC/PAID ALLOC Transfers",
                Description = $"Error validating ALLOC transfers for year {profitYear}",
                IsValid = false,
                Validations = validations,
                Summary = $"❌ Error validating ALLOC transfers: {ex.Message}",
                Priority = "Critical",
                ValidationRule = "Sum(ALLOC) + Sum(PAID ALLOC) must equal 0 (Balance Matrix Rule 2)"
            };

            return Result<CrossReferenceValidationGroup>.Failure(
                Error.Unexpected($"Failed to validate ALLOC transfers: {ex.Message}"));
        }
    }
}
