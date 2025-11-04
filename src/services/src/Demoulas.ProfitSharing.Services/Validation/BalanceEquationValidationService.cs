using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Validation;

/// <summary>
/// Service for validating the complete PAY444 balance equation per the Balance Reports Cross-Reference Matrix.
/// Focused on Rule 5: Ending Balance = Beginning Balance + Contributions + ALLOC - Distributions - PAID ALLOC + Earnings - Forfeitures
/// </summary>
public sealed class BalanceEquationValidationService : IBalanceEquationValidationService
{
    private readonly ILogger<BalanceEquationValidationService> _logger;

    public BalanceEquationValidationService(
        ILogger<BalanceEquationValidationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<Result<CrossReferenceValidationGroup>> ValidateBalanceEquationAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        CancellationToken cancellationToken = default)
    {
        var validations = new List<CrossReferenceValidation>();

        try
        {
            _logger.LogInformation(
                "Starting balance equation validation for year {ProfitYear} with {FieldCount} current values",
                profitYear, currentValues.Count);

            // Extract balance equation components from currentValues
            // Expected keys: "PAY444.BeginningBalance", "PAY444.CONTRIB", "PAY444.ALLOC", etc.

            var beginningBalance = GetFieldValue(currentValues, "PAY444.BeginningBalance");
            var contributions = GetFieldValue(currentValues, "PAY444.CONTRIB");
            var allocIn = GetFieldValue(currentValues, "PAY444.ALLOC");
            var distributions = GetFieldValue(currentValues, "PAY444.DISTRIB");
            var allocOut = GetFieldValue(currentValues, "PAY444.PAIDALLOC");
            var earnings = GetFieldValue(currentValues, "PAY444.EARNINGS");
            var forfeitures = GetFieldValue(currentValues, "PAY444.FORFEITS");
            var expectedEndingBalance = GetFieldValue(currentValues, "PAY444.EndingBalance");

            // Record each component for visibility
            validations.Add(CreateComponentValidation(
                "BeginningBalance", "PAY444", beginningBalance,
                "Starting balance from prior year PAY443"));

            validations.Add(CreateComponentValidation(
                "Contributions", "PAY444", contributions,
                "Employer + employee contributions (excluding ALLOC transfers)"));

            validations.Add(CreateComponentValidation(
                "ALLOC", "PAY444", allocIn,
                "Internal transfers IN from other accounts"));

            validations.Add(CreateComponentValidation(
                "Distributions", "PAY444", distributions,
                "Withdrawals and distributions (excluding PAID ALLOC)"));

            validations.Add(CreateComponentValidation(
                "PAID ALLOC", "PAY444", allocOut,
                "Internal transfers OUT to other accounts"));

            validations.Add(CreateComponentValidation(
                "Earnings", "PAY444", earnings,
                "Investment gains/losses for the year"));

            validations.Add(CreateComponentValidation(
                "Forfeitures", "PAY444", forfeitures,
                "Forfeited amounts redistributed"));

            // Calculate ending balance from equation
            // Ending Balance = Beginning + Contributions + ALLOC - Distributions - PAID ALLOC + Earnings - Forfeitures
            decimal calculatedEndingBalance = beginningBalance
                + contributions
                + allocIn
                - distributions
                - allocOut
                + earnings
                - forfeitures;

            _logger.LogInformation(
                "Balance equation for year {ProfitYear}: " +
                "Beginning={BeginningBalance:C} + Contributions={Contributions:C} + ALLOC={Alloc:C} - " +
                "Distributions={Distributions:C} - PAID ALLOC={PaidAlloc:C} + Earnings={Earnings:C} - " +
                "Forfeitures={Forfeitures:C} = Calculated Ending={CalculatedEnding:C}, " +
                "Expected Ending={ExpectedEnding:C}",
                profitYear, beginningBalance, contributions, allocIn, distributions, allocOut,
                earnings, forfeitures, calculatedEndingBalance, expectedEndingBalance);

            // Compare calculated vs expected ending balance
            decimal variance = calculatedEndingBalance - expectedEndingBalance;
            bool isValid = Math.Abs(variance) < 0.01m; // Allow for penny rounding differences

            // Create validation for calculated ending balance
            validations.Add(new CrossReferenceValidation
            {
                FieldName = "CalculatedEndingBalance",
                ReportCode = "PAY444",
                CurrentValue = calculatedEndingBalance,
                ExpectedValue = expectedEndingBalance,
                Variance = variance,
                IsValid = isValid,
                Message = isValid
                    ? $"✅ Balance equation validates: {calculatedEndingBalance:C} matches expected {expectedEndingBalance:C}"
                    : $"⚠️ Balance equation MISMATCH: Calculated {calculatedEndingBalance:C} vs Expected {expectedEndingBalance:C} (variance: {variance:C})",
                Notes = isValid
                    ? "All balance equation components reconcile correctly"
                    : "CRITICAL: Balance equation does not reconcile - investigate component discrepancies"
            });

            string summary = isValid
                ? $"✅ Balance equation validates for year {profitYear}: {calculatedEndingBalance:C} = {expectedEndingBalance:C}"
                : $"⚠️ Balance equation MISMATCH for year {profitYear}: Calculated {calculatedEndingBalance:C} vs Expected {expectedEndingBalance:C} (variance: {variance:C})";

            string detailedBreakdown =
                $"Beginning: {beginningBalance:C} + " +
                $"Contrib: {contributions:C} + " +
                $"ALLOC: {allocIn:C} - " +
                $"Distrib: {distributions:C} - " +
                $"Paid ALLOC: {allocOut:C} + " +
                $"Earnings: {earnings:C} - " +
                $"Forfeits: {forfeitures:C} = " +
                $"{calculatedEndingBalance:C}";

            // Record telemetry
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "balance-equation-validation"),
                new KeyValuePair<string, object?>("endpoint.category", "balance-validation"),
                new KeyValuePair<string, object?>("profit_year", profitYear),
                new KeyValuePair<string, object?>("validation_result", isValid ? "pass" : "fail"));

            if (!isValid)
            {
                // Track variance amount for monitoring
                EndpointTelemetry.RecordCountsProcessed.Record((long)Math.Abs(variance * 100), // Convert to cents
                    new KeyValuePair<string, object?>("record_type", "balance-equation-variance"),
                    new KeyValuePair<string, object?>("profit_year", profitYear));

                _logger.LogWarning(
                    "CRITICAL: Balance equation mismatch for year {ProfitYear}: {Variance:C} difference. {Breakdown}",
                    profitYear, variance, detailedBreakdown);
            }
            else
            {
                _logger.LogInformation(
                    "Balance equation validated successfully for year {ProfitYear}. {Breakdown}",
                    profitYear, detailedBreakdown);
            }

            var result = new CrossReferenceValidationGroup
            {
                GroupName = "Balance Equation",
                Description = $"Validates PAY444 balance equation for year {profitYear}: " +
                             "Ending Balance = Beginning + Contributions + ALLOC - Distributions - PAID ALLOC + Earnings - Forfeitures",
                IsValid = isValid,
                Validations = validations,
                Summary = summary + Environment.NewLine + detailedBreakdown,
                Priority = "Critical",
                ValidationRule = "Ending Balance = Beginning Balance + Contributions + ALLOC - Distributions - PAID ALLOC + Earnings - Forfeitures (Balance Matrix Rule 5)"
            };

            return await Task.FromResult(Result<CrossReferenceValidationGroup>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating balance equation for year {ProfitYear}", profitYear);

            // Record failure telemetry
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "balance-equation-validation"),
                new KeyValuePair<string, object?>("endpoint.category", "balance-validation"),
                new KeyValuePair<string, object?>("profit_year", profitYear),
                new KeyValuePair<string, object?>("validation_result", "error"));

            validations.Add(new CrossReferenceValidation
            {
                FieldName = "BalanceEquation",
                ReportCode = "PAY444",
                CurrentValue = null,
                ExpectedValue = null,
                IsValid = false,
                Message = $"Exception during balance equation validation: {ex.Message}",
                Notes = "Error occurred during calculation or data retrieval"
            });

            var errorResult = new CrossReferenceValidationGroup
            {
                GroupName = "Balance Equation",
                Description = $"Error validating balance equation for year {profitYear}",
                IsValid = false,
                Validations = validations,
                Summary = $"❌ Error validating balance equation: {ex.Message}",
                Priority = "Critical",
                ValidationRule = "Ending Balance = Beginning Balance + Contributions + ALLOC - Distributions - PAID ALLOC + Earnings - Forfeitures (Balance Matrix Rule 5)"
            };

            return await Task.FromResult(Result<CrossReferenceValidationGroup>.Failure(
                Error.Unexpected($"Failed to validate balance equation: {ex.Message}")));
        }
    }

    /// <summary>
    /// Retrieves a field value from the currentValues dictionary with error handling.
    /// </summary>
    private decimal GetFieldValue(Dictionary<string, decimal> currentValues, string key)
    {
        if (currentValues.TryGetValue(key, out var value))
        {
            return value;
        }

        _logger.LogWarning("Field {FieldKey} not found in current values, defaulting to 0", key);
        return 0m;
    }

    /// <summary>
    /// Creates a component validation entry for visibility in the validation results.
    /// </summary>
    private CrossReferenceValidation CreateComponentValidation(
        string fieldName,
        string reportCode,
        decimal value,
        string notes)
    {
        return new CrossReferenceValidation
        {
            FieldName = fieldName,
            ReportCode = reportCode,
            CurrentValue = value,
            ExpectedValue = null, // Components don't have "expected" values individually
            IsValid = true, // Component values are informational
            Message = $"{fieldName}: {value:C}",
            Notes = notes
        };
    }
}
