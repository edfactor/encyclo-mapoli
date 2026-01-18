using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Validation;

/// <summary>
/// Service for orchestrating cross-reference validation across multiple year-end report fields.
/// Extracted from ChecksumValidationService to improve separation of concerns (PS-1721).
/// </summary>
/// <remarks>
/// This service validates that related fields across different year-end reports (PAY443, PAY444, QPAY129, QPAY066TA)
/// maintain consistent values by comparing current values against archived checksums.
///
/// Validation is organized into logical groups:
/// - Beginning Balance: PAY444.BeginningBalance vs PAY443.TotalProfitSharingBalance
/// - Distributions: PAY444.DISTRIB vs PAY443, QPAY129, QPAY066TA
/// - Forfeitures: PAY444.FORFEITS vs PAY443, QPAY129
/// - Contributions: PAY444.CONTRIB vs PAY443
/// - Earnings: PAY444.EARNINGS vs PAY443
/// - ALLOC Transfers: ALLOC + PAID ALLOC must sum to zero (Balance Matrix Rule 2)
/// - Balance Equation: Comprehensive validation of all balance components (Balance Matrix Rule 5)
/// </remarks>
public class CrossReferenceValidationService : ICrossReferenceValidationService
{
    private readonly IChecksumValidationService _checksumValidationService;
    private readonly IAllocTransferValidationService _allocTransferValidationService;
    private readonly IBalanceEquationValidationService _balanceEquationValidationService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<CrossReferenceValidationService> _logger;

    public CrossReferenceValidationService(
        IChecksumValidationService checksumValidationService,
        IAllocTransferValidationService allocTransferValidationService,
        IBalanceEquationValidationService balanceEquationValidationService,
        IProfitSharingDataContextFactory dataContextFactory,
        ILogger<CrossReferenceValidationService> logger)
    {
        _checksumValidationService = checksumValidationService;
        _allocTransferValidationService = allocTransferValidationService;
        _balanceEquationValidationService = balanceEquationValidationService;
        _dataContextFactory = dataContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Validates cross-references between Master Update (PAY444) fields and related year-end reports.
    /// </summary>
    /// <param name="profitYear">The profit year to validate.</param>
    /// <param name="currentValues">Dictionary of current field values keyed by "ReportCode.FieldName".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Comprehensive validation response with groups, statistics, and recommended actions.</returns>
    public async Task<Result<MasterUpdateCrossReferenceValidationResponse>> ValidateMasterUpdateCrossReferencesAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Starting cross-reference validation for Master Update year {ProfitYear} with {FieldCount} current values",
                profitYear, currentValues.Count);

            // Track which reports have been validated (to avoid duplicate checks)
            var validatedReports = new HashSet<string>();

            // Execute validation groups
            var beginningBalanceGroup = await ValidateBeginningBalanceGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);

            var distributionsGroup = await ValidateDistributionsGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);

            var forfeituresGroup = await ValidateForfeituresGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);

            var contributionsGroup = await ValidateContributionsGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);

            var earningsGroup = await ValidateEarningsGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);

            var allocTransfersGroup = await ValidateAllocTransfersGroupAsync(
                profitYear, cancellationToken);

            var balanceEquationGroup = await ValidateBalanceEquationGroupAsync(
                profitYear, currentValues, cancellationToken);

            var forfeitPointsGroup = await ValidateForfeitPointsGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);

            var earningPointsGroup = await ValidateEarningPointsGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);

            // Combine all validation groups
            var validationGroups = new List<CrossReferenceValidationGroup>
            {
                beginningBalanceGroup,
                distributionsGroup,
                forfeituresGroup,
                contributionsGroup,
                earningsGroup,
                allocTransfersGroup,
                balanceEquationGroup,
                forfeitPointsGroup,
                earningPointsGroup
            };

            int totalValidations, passedValidations, failedValidations;
            List<string> criticalIssues, warnings;
            SummarizeValidations(validationGroups, out totalValidations, out passedValidations, out failedValidations, out criticalIssues, out warnings);
            bool blockMasterUpdate = criticalIssues.Any();

            _logger.LogInformation(
                "Cross-reference validation completed for year {ProfitYear}: {PassedCount}/{TotalCount} validations passed, {CriticalCount} critical issues, Block={Block}",
                profitYear, passedValidations, totalValidations, criticalIssues.Count, blockMasterUpdate);

            var message = blockMasterUpdate
                ? "⚠️ BLOCK Master Update submission. Critical validation failures detected."
                : warnings.Any()
                    ? "⚠️ Allow Master Update submission with warnings. Review non-critical issues."
                    : "✅ All cross-reference validations passed. Safe to submit Master Update.";

            var response = new MasterUpdateCrossReferenceValidationResponse
            {
                ProfitYear = profitYear,
                ValidationGroups = validationGroups,
                TotalValidations = totalValidations,
                PassedValidations = passedValidations,
                FailedValidations = failedValidations,
                CriticalIssues = criticalIssues,
                Warnings = warnings,
                BlockMasterUpdate = blockMasterUpdate,
                Message = message
            };

            return Result<MasterUpdateCrossReferenceValidationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Master Update cross-references for year {ProfitYear}", profitYear);
            return Result<MasterUpdateCrossReferenceValidationResponse>.Failure(
                Error.Unexpected($"Cross-reference validation failed: {ex.Message}"));
        }
    }

    public async Task<Result<ValidationResponse>> ValidateProfitSharingReportAsync(short profitYear, string reportSuffix, bool isFrozen, CancellationToken cancellationToken = default)
    {
        var checksumPrefix = reportSuffix switch
        {
            "1" => "18_20WillVest",
            "2" => "Over21WillVest",
            "3" => "Under18",
            "4" => "Ovr18WillNotVestPrior",
            "5" => "Ovr18WillNotVestNoPrior",
            "6" => "TermOvr18WouldVest",
            "7" => "TermUndrWouldNotVestNoPrior",
            "8" => "TermOvr18WouldNotVestNoPrior",
            _ => null
        };

        var message = "Nothing done yet";

        if (string.IsNullOrEmpty(checksumPrefix))
        {
            return Result<ValidationResponse>.Failure(Error.ReportNotFound);
        }

        var membersKey = checksumPrefix + "Members";
        var balanceKey = checksumPrefix + "Balance";
        var wagesKey = checksumPrefix + "Wages";

        var memberValidation = await ValidateProfitSharingReportGroup(profitYear, membersKey, "Members", isFrozen, cancellationToken);

        var balanceValidation = await ValidateProfitSharingReportGroup(profitYear, balanceKey, "Balance", isFrozen, cancellationToken);

        var wagesValidation = await ValidateProfitSharingReportGroup(profitYear, wagesKey, "Wages", isFrozen, cancellationToken);

        var validationGroups = new List<CrossReferenceValidationGroup>
        {
            memberValidation,
            balanceValidation,
            wagesValidation
        };

        int totalValidations, passedValidations, failedValidations;
        List<string> criticalIssues, warnings;
        SummarizeValidations(validationGroups, out totalValidations, out passedValidations, out failedValidations, out criticalIssues, out warnings);

        var rslt = new ValidationResponse()
        {
            ProfitYear = profitYear,
            Message = message,
            ValidationGroups = validationGroups,
            CriticalIssues = criticalIssues,
            Warnings = warnings,
            TotalValidations = totalValidations,
            FailedValidations = failedValidations,
            PassedValidations = passedValidations
        };

        return Result<ValidationResponse>.Success(rslt);
    }

    public async Task<ValidationResponse> ValidateForfeitureAndPointsReportAsync(short profitYear, decimal distributionTotal, decimal forfeitTotal, CancellationToken cancellationToken = default)
    {
        var currentValues = new Dictionary<string, decimal>
        {
            { "QPAY129.QPAY129_DistributionTotals", distributionTotal},
            { "QPAY129.ForfeitureTotal", forfeitTotal }
        };

        // Execute validations in parallel for better performance
        var distributionTask = ValidateSingleFieldAsync(
            profitYear,
            "QPAY129",
            "QPAY129_DistributionTotals",
            currentValues,
            cancellationToken);
        var forfeitTask = ValidateSingleFieldAsync(
            profitYear,
            "QPAY129",
            "ForfeitureTotal",
            currentValues,
            cancellationToken);

        await Task.WhenAll(distributionTask, forfeitTask);

        var distributionTotalValidation = await distributionTask;
        var forfeitTotalValidation = await forfeitTask;

        var validationGroup = new CrossReferenceValidationGroup
        {
            GroupName = "Forfeitures and Points",
            Description = "Validates PAY444 Forfeitures and Points against archived values",
            IsValid = distributionTotalValidation.IsValid
                      && forfeitTotalValidation.IsValid,
            Validations = new List<CrossReferenceValidation>
            {
                distributionTotalValidation,
                forfeitTotalValidation
            },
            Summary = "Forfeitures and Points validation completed.",
            Priority = "High",
            ValidationRule = "PAY444 Distribution Totals, and Forfeit Totals should match archived values"
        };

        var validationGroups = new List<CrossReferenceValidationGroup> { validationGroup };
        int totalValidations, passedValidations, failedValidations;
        List<string> criticalIssues, warnings;
        SummarizeValidations(validationGroups, out totalValidations, out passedValidations, out failedValidations, out criticalIssues, out warnings);
        var rslt = new ValidationResponse()
        {
            ProfitYear = profitYear,
            Message = "Forfeitures and Points validation completed.",
            ValidationGroups = validationGroups,
            CriticalIssues = criticalIssues,
            Warnings = warnings,
            TotalValidations = totalValidations,
            FailedValidations = failedValidations,
            PassedValidations = passedValidations
        };
        return rslt;
    }

    public async Task<ValidationResponse> ValidateBreakoutReportGrandTotalAsync(
        short profitYear,
        int numberOfEmployees,
        decimal beginningBalance,
        decimal earningsTotal,
        decimal contributionsTotal,
        decimal disbursementTotals,
        decimal endingBalance,
        CancellationToken cancellationToken = default
        )
    {

        var currentValues = new Dictionary<string, decimal>
        {
            { "PAY444.BeginningBalanceTotal", beginningBalance},
            { "PAY444.EarningsGrandTotal", earningsTotal  },
            { "PAY444.ContributionGrandTotal", contributionsTotal }
        };

        // Execute validations in parallel for better performance
        var beginningBalanceTask = ValidateSingleFieldAsync(
            profitYear,
            "PAY444",
            "BeginningBalanceTotal",
            currentValues,
            cancellationToken);

        var earningsTotalTask = ValidateSingleFieldAsync(
            profitYear,
            "PAY444",
            "EarningsGrandTotal",
            currentValues,
            cancellationToken);

        var contributionsTotalTask = ValidateSingleFieldAsync(
            profitYear,
            "PAY444",
            "ContributionsGrandTotal",
            currentValues,
            cancellationToken);

        await Task.WhenAll(beginningBalanceTask, earningsTotalTask, contributionsTotalTask);

        var beginningBalanceValidation = await beginningBalanceTask;
        var earningsTotalValidation = await earningsTotalTask;
        var contributionsTotalValidation = await contributionsTotalTask;

        var beginningBalanceValidationGroup = new CrossReferenceValidationGroup
        {
            GroupName = "Beginning Balance",
            Description = "Validates beginning balance between Master Update and the breakdown report",
            IsValid = beginningBalanceValidation.IsValid,
            Validations = new List<CrossReferenceValidation>
            {
                beginningBalanceValidation
            },
            Summary = "Beginning balance validation.",
            Priority = "High",
            ValidationRule = "Store breakdown beginning balance should match archived values"
        };

        var earningsTotalValidationGroup = new CrossReferenceValidationGroup
        {
            GroupName = "Earnings Total",
            Description = "Validates earnings total between Master Update and the breakdown report",
            IsValid = earningsTotalValidation.IsValid,
            Validations = new List<CrossReferenceValidation>
            {
                earningsTotalValidation
            },
            Summary = "Earnings total validation.",
            Priority = "High",
            ValidationRule = "Store breakdown earnings total should match archived values"
        };

        var contributionsTotalValidationGroup = new CrossReferenceValidationGroup
        {
            GroupName = "Contributions Total",
            Description = "Validates contributions total between Master Update and the breakdown report",
            IsValid = contributionsTotalValidation.IsValid,
            Validations = new List<CrossReferenceValidation>
            {
                contributionsTotalValidation
            },
            Summary = "Contributions total validation.",
            Priority = "High",
            ValidationRule = "Store breakdown contributions total should match archived values"
        };

        var validationGroups = new List<CrossReferenceValidationGroup>() { beginningBalanceValidationGroup, earningsTotalValidationGroup, contributionsTotalValidationGroup };
        int totalValidations, passedValidations, failedValidations;
        List<string> criticalIssues, warnings;
        SummarizeValidations(validationGroups, out totalValidations, out passedValidations, out failedValidations, out criticalIssues, out warnings);

        var rslt = new ValidationResponse()
        {
            ProfitYear = profitYear,
            Message = "Breakdown validations",
            ValidationGroups = validationGroups,
            CriticalIssues = criticalIssues,
            Warnings = warnings,
            TotalValidations = totalValidations,
            FailedValidations = failedValidations,
            PassedValidations = passedValidations
        };

        return rslt;
    }

    private static void SummarizeValidations(List<CrossReferenceValidationGroup> validationGroups, out int totalValidations, out int passedValidations, out int failedValidations, out List<string> criticalIssues, out List<string> warnings)
    {
        // Calculate statistics
        totalValidations = validationGroups.Sum(g => g.Validations.Count);
        passedValidations = validationGroups.Sum(g => g.Validations.Count(v => v.IsValid));
        failedValidations = totalValidations - passedValidations;

        // Identify critical issues
        criticalIssues = validationGroups
            .Where(g => !g.IsValid && g.Priority == "Critical")
            .Select(g => g.GroupName)
            .ToList();
        warnings = validationGroups
            .Where(g => !g.IsValid && g.Priority != "Critical")
            .Select(g => g.GroupName)
            .ToList();
    }

    #region Validation Groups

    private async Task<CrossReferenceValidationGroup> ValidateProfitSharingReportGroup(short profitYear, string key, string validationName, bool isFrozen, CancellationToken cancellationToken)
    {
        var reportNameSuffix = isFrozen ? "_FROZEN" : "";
        var wagesValidation = await ValidateSingleFieldAsync(
            profitYear,
            ReportNames.ProfitSharingSummary.ReportCode + reportNameSuffix,
            key,
            new Dictionary<string, decimal>(),
            cancellationToken);

        var validations = new List<CrossReferenceValidation>() { wagesValidation };

        bool allValid = validations.All(v => v.IsValid);

        string summary = allValid
            ? $"✅ {validationName} matches archived Profit Sharing Summary for year {profitYear}"
            : $"⚠️ {validationName} does NOT match archived Profit Sharing Summary for year {profitYear}";

        return new CrossReferenceValidationGroup
        {
            GroupName = validationName,
            Description = $"Validates Profit Sharing Report.  Matches archived Profit Sharing Summary {validationName} for year {profitYear}",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "Critical",
            ValidationRule = $"Profit Sharing Report {validationName} should equal archived Profit Sharing Summary {validationName}"
        };
    }

    private async Task<CrossReferenceValidationGroup> ValidateBeginningBalanceGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // Get archived PAY443.TotalProfitSharingBalance for SAME year (profitYear)
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear,
            "PAY443",
            "TotalProfitSharingBalance",
            currentValues,
            cancellationToken);

        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");
        validatedReports.Add("PAY444");

        bool allValid = validations.All(v => v.IsValid);

        string summary = allValid
            ? $"✅ Beginning balance matches archived PAY443 for year {profitYear}"
            : $"⚠️ Beginning balance does NOT match archived PAY443 for year {profitYear}";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Beginning Balance",
            Description = $"Validates PAY444.BeginningBalance matches archived PAY443.TotalProfitSharingBalance for year {profitYear}",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "Critical",
            ValidationRule = $"PAY444.BeginningBalance should equal archived PAY443.TotalProfitSharingBalance"
        };
    }

    /// <summary>
    /// Validates the Total Distributions cross-references (PAY443, QPAY129, QPAY066TA).
    /// Validation Rule: PAY444.DISTRIB = PAY443.TotalDistributions = QPAY129.Distributions = QPAY066TA.TotalDisbursements
    /// </summary>
    private async Task<CrossReferenceValidationGroup> ValidateDistributionsGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // PAY443.DistributionTotals
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear, "PAY443", "DistributionTotals", currentValues, cancellationToken);
        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");

        // QPAY129.Distributions
        var qpay129Validation = await ValidateSingleFieldAsync(
            profitYear, ReportNames.DistributionAndForfeitures.ReportCode, "QPAY129_DistributionTotals", currentValues, cancellationToken);
        validations.Add(qpay129Validation);
        validatedReports.Add(ReportNames.DistributionAndForfeitures.ReportCode);

        // QPAY066TA.TotalDisbursements
        var qpay066taValidation = await ValidateSingleFieldAsync(
            profitYear, "QPAY066TA", "TotalDisbursements", currentValues, cancellationToken);
        validations.Add(qpay066taValidation);
        validatedReports.Add("QPAY066TA");

        bool allValid = validations.All(v => v.IsValid);
        string summary = allValid
            ? "All distribution totals are in sync across PAY443, QPAY129, and QPAY066TA."
            : "Distribution totals are OUT OF SYNC. This is a critical financial discrepancy.";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Total Distributions",
            Description = "Cross-validation of distribution totals across year-end reports",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "Critical",
            ValidationRule = "PAY444.DISTRIB = PAY443.DistributionTotals = QPAY129.Distributions = QPAY066TA.TotalDisbursements"
        };
    }

    /// <summary>
    /// Validates the Total Forfeitures cross-references (PAY443, QPAY129).
    /// Validation Rule: PAY444.FORFEITS = PAY443.TotalForfeitures = QPAY129.ForfeitedAmount
    /// </summary>
    private async Task<CrossReferenceValidationGroup> ValidateForfeituresGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // PAY443.TotalForfeitures
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear, "PAY443", "TotalForfeitures", currentValues, cancellationToken);
        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");

        // QPAY129.ForfeitedAmount
        var qpay129Validation = await ValidateSingleFieldAsync(
            profitYear, "QPAY129", "ForfeitedAmount", currentValues, cancellationToken);
        validations.Add(qpay129Validation);
        validatedReports.Add("QPAY129");

        bool allValid = validations.All(v => v.IsValid);
        string summary = allValid
            ? "All forfeiture totals are in sync across PAY443 and QPAY129."
            : "Forfeiture totals are OUT OF SYNC. Review PAY443 and QPAY129 data.";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Total Forfeitures",
            Description = "Cross-validation of forfeiture totals across year-end reports",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "Critical",
            ValidationRule = "PAY444.FORFEITS = PAY443.TotalForfeitures = QPAY129.ForfeitedAmount"
        };
    }

    /// <summary>
    /// Validates the Total Contributions cross-references (PAY443).
    /// Validation Rule: PAY444.CONTRIB = PAY443.TotalContributions
    /// </summary>
    private async Task<CrossReferenceValidationGroup> ValidateContributionsGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // PAY443.TotalContributions (if it exists - need to add to response DTO)
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear, "PAY443", "TotalContributions", currentValues, cancellationToken);
        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");

        bool allValid = validations.All(v => v.IsValid);
        string summary = allValid
            ? "Contribution totals are in sync."
            : "Contribution totals mismatch detected.";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Total Contributions",
            Description = "Cross-validation of contribution totals",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "High",
            ValidationRule = "PAY444.CONTRIB = PAY443.TotalContributions"
        };
    }

    /// <summary>
    /// Validates the Total Earnings cross-references (PAY443).
    /// Validation Rule: PAY444.EARNINGS = PAY443.TotalEarnings
    /// </summary>
    private async Task<CrossReferenceValidationGroup> ValidateEarningsGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // PAY443.TotalEarnings (if it exists - need to add to response DTO)
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear, "PAY443", "TotalEarnings", currentValues, cancellationToken);
        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");

        bool allValid = validations.All(v => v.IsValid);
        string summary = allValid
            ? "Earnings totals are in sync."
            : "Earnings totals mismatch detected.";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Total Earnings",
            Description = "Cross-validation of earnings totals",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "High",
            ValidationRule = "PAY444.EARNINGS = PAY443.TotalEarnings"
        };
    }

    private async Task<CrossReferenceValidationGroup> ValidateForfeitPointsGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // PAY443.TotalForfeitPoints (if it exists - need to add to response DTO)
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear, "PAY443", "TotalForfeitPoints", currentValues, cancellationToken);
        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");

        bool allValid = validations.All(v => v.IsValid);
        string summary = allValid
            ? "Forfeit point totals are in sync."
            : "Forfeit point totals mismatch detected.";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Total Forfeit Points",
            Description = "Cross-validation of forfeit point totals",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "High",
            ValidationRule = "PAY444.FORFEIT_POINTS = PAY443.TotalForfeitPoints"
        };
    }

    private async Task<CrossReferenceValidationGroup> ValidateEarningPointsGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // PAY443.TotalForfeitPoints (if it exists - need to add to response DTO)
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear, "PAY443", "TotalEarningPoints", currentValues, cancellationToken);
        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");

        bool allValid = validations.All(v => v.IsValid);
        string summary = allValid
            ? "Earning point totals are in sync."
            : "Earning point totals mismatch detected.";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Total Earning Points",
            Description = "Cross-validation of earning point totals",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "High",
            ValidationRule = "PAY444.EARNING_POINTS = PAY443.TotalEarningPoints"
        };
    }

    /// <summary>
    /// Validates that ALLOC (Incoming QDRO Beneficiary) and PAID ALLOC (Outgoing XFER Beneficiary)
    /// transactions sum to zero, per Balance Matrix Rule 2.
    /// Delegates to IAllocTransferValidationService for the actual validation logic.
    /// </summary>
    /// <remarks>
    /// ALLOC: ProfitCodeId = 6 (IncomingQdroBeneficiary), stored in Contribution field.
    /// PAID ALLOC: ProfitCodeId = 5 (OutgoingXferBeneficiary), stored in Forfeiture field.
    /// Rule: Sum(ALLOC) + Sum(PAID ALLOC) = 0
    /// Reference: BALANCE_REPORTS_CROSS_REFERENCE_MATRIX.md - Rule 2
    /// </remarks>
    private async Task<CrossReferenceValidationGroup> ValidateAllocTransfersGroupAsync(
        short profitYear,
        CancellationToken cancellationToken)
    {
        // Delegate to dedicated ALLOC transfer validation service
        var result = await _allocTransferValidationService.ValidateAllocTransfersAsync(profitYear, cancellationToken);

        // Return the validation group, or a default error group if the service call failed
        if (result.IsSuccess && result.Value != null)
        {
            return result.Value;
        }

        _logger.LogError("ALLOC transfer validation service returned failure: {Error}", result.Error?.Description);

        // Return error validation group
        return new CrossReferenceValidationGroup
        {
            GroupName = "ALLOC/PAID ALLOC Transfers",
            Description = $"Error validating ALLOC transfers for year {profitYear}",
            IsValid = false,
            Validations = new List<CrossReferenceValidation>
            {
                new CrossReferenceValidation
                {
                    FieldName = "NetAllocTransfer",
                    ReportCode = "PAY444",
                    CurrentValue = null,
                    ExpectedValue = 0m,
                    IsValid = false,
                    Message = $"Service error: {result.Error?.Description ?? "Unknown error"}",
                    Notes = "Balance validation service failed"
                }
            },
            Summary = $"❌ Error validating ALLOC transfers: {result.Error?.Description ?? "Unknown error"}",
            Priority = "Critical",
            ValidationRule = "Sum(ALLOC) + Sum(PAID ALLOC) must equal 0 (Balance Matrix Rule 2)"
        };
    }

    private async Task<CrossReferenceValidationGroup> ValidateBalanceEquationGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        CancellationToken cancellationToken)
    {
        // Delegate to dedicated balance equation validation service
        var result = await _balanceEquationValidationService.ValidateBalanceEquationAsync(
            profitYear, currentValues, cancellationToken);

        // Return the validation group, or a default error group if the service call failed
        if (result.IsSuccess && result.Value != null)
        {
            return result.Value;
        }

        _logger.LogError("Balance equation validation service returned failure: {Error}", result.Error?.Description);

        // Return error validation group
        return new CrossReferenceValidationGroup
        {
            GroupName = "Balance Equation",
            Description = $"Error validating balance equation for year {profitYear}",
            IsValid = false,
            Validations = new List<CrossReferenceValidation>
            {
                new CrossReferenceValidation
                {
                    FieldName = "BalanceEquation",
                    ReportCode = "PAY444",
                    CurrentValue = null,
                    ExpectedValue = null,
                    IsValid = false,
                    Message = $"Service error: {result.Error?.Description ?? "Unknown error"}",
                    Notes = "Balance equation validation service failed"
                }
            },
            Summary = $"❌ Error validating balance equation: {result.Error?.Description ?? "Unknown error"}",
            Priority = "Critical",
            ValidationRule = "Ending Balance = Beginning Balance + Contributions + ALLOC - Distributions - PAID ALLOC + Earnings - Forfeitures (Balance Matrix Rule 5)"
        };
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Validates a single field against its archived checksum.
    /// </summary>
    private async Task<CrossReferenceValidation> ValidateSingleFieldAsync(
        short profitYear,
        string reportCode,
        string fieldName,
        Dictionary<string, decimal> currentValues,
        CancellationToken cancellationToken)
    {
        try
        {
            // Look up current value using "ReportCode.FieldName" key
            string lookupKey = $"{reportCode}.{fieldName}";
            decimal? currentValue = currentValues.TryGetValue(lookupKey, out decimal value)
                ? value
                : null;

            // Always fetch the archived/expected value, even if no current value provided
            // This allows the UI to perform its own comparison
            decimal? expectedValue = null;
            DateTime? archivedAt = null;

            var archived = await _dataContextFactory.UseReadOnlyContext(async ctx =>
                await ctx.ReportChecksums
                    .TagWith($"GetArchivedValue-{reportCode}-{fieldName}-Year{profitYear}")
                    .Where(r => r.ProfitYear == profitYear && r.ReportType == reportCode)
                    .OrderByDescending(r => r.CreatedAtUtc)
                    .FirstOrDefaultAsync(cancellationToken), cancellationToken);

            if (archived != null)
            {
                archivedAt = archived.CreatedAtUtc.DateTime;
                var archivedFieldData = archived.KeyFieldsChecksumJson
                    .FirstOrDefault(kvp => kvp.Key == fieldName);

                if (!string.IsNullOrEmpty(archivedFieldData.Key))
                {
                    expectedValue = archivedFieldData.Value.Key; // The actual archived value
                }
            }

            if (currentValue == null)
            {
                // Return archived value even though no current value to compare against
                // UI will use expectedValue for its own comparison
                return new CrossReferenceValidation
                {
                    FieldName = fieldName,
                    ReportCode = reportCode,
                    IsValid = false, // Cannot validate without current value
                    CurrentValue = null,
                    ExpectedValue = expectedValue,
                    ArchivedAt = archivedAt,
                    Message = $"Current value not provided for {reportCode}.{fieldName}",
                    Notes = expectedValue.HasValue
                        ? $"Archived value available: {expectedValue.Value:N2}"
                        : "No archived value found"
                };
            }

            // Validate against archived checksum
            var validationResult = await _checksumValidationService.ValidateReportFieldsAsync(
                profitYear,
                reportCode,
                new Dictionary<string, decimal> { [fieldName] = currentValue.Value },
                cancellationToken);

            if (!validationResult.IsSuccess)
            {
                return new CrossReferenceValidation
                {
                    FieldName = fieldName,
                    ReportCode = reportCode,
                    IsValid = false,
                    CurrentValue = currentValue,
                    Message = $"Validation failed: {validationResult.Error?.Description ?? "Unknown error"}",
                    Notes = "Error during validation"
                };
            }

            var checksumResponse = validationResult.Value;
            bool fieldIsValid = checksumResponse?.IsValid ?? false;

            // expectedValue and archivedAt already fetched above

            decimal? variance = expectedValue.HasValue && currentValue.HasValue
                ? currentValue - expectedValue
                : null;

            string message = fieldIsValid
                ? $"{reportCode}.{fieldName} matches archived value"
                : $"{reportCode}.{fieldName} does NOT match archived value";

            return new CrossReferenceValidation
            {
                FieldName = fieldName,
                ReportCode = reportCode,
                IsValid = fieldIsValid,
                CurrentValue = currentValue,
                ExpectedValue = expectedValue,
                Variance = variance,
                Message = message,
                ArchivedAt = checksumResponse?.ArchivedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error validating {ReportCode}.{FieldName} for year {ProfitYear}",
                reportCode, fieldName, profitYear);

            return new CrossReferenceValidation
            {
                FieldName = fieldName,
                ReportCode = reportCode,
                IsValid = false,
                Message = $"Validation error: {ex.Message}",
                Notes = "Exception during validation"
            };
        }
    }

    #endregion
}
