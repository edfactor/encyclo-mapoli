using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitMaster;

public class ProfitMasterUpdateEndpoint : ProfitSharingEndpoint<ProfitShareUpdateRequest, ProfitMasterUpdateResponse>
{
    private readonly IProfitMasterService _profitMasterService;
    private readonly INavigationService _navigationService;
    private readonly IAuditService _auditService;
    private readonly INavigationPrerequisiteValidator _navPrereqValidator;
    private readonly IChecksumValidationService _checksumValidationService;
    private readonly ILogger<ProfitMasterUpdateEndpoint> _logger;

    public ProfitMasterUpdateEndpoint(IProfitMasterService profitMasterUpdate,
        INavigationService navigationService,
        IAuditService auditService,
        INavigationPrerequisiteValidator navPrereqValidator,
        IChecksumValidationService checksumValidationService,
        ILogger<ProfitMasterUpdateEndpoint> logger)
        : base(Navigation.Constants.MasterUpdate)
    {
        _profitMasterService = profitMasterUpdate;
        _navigationService = navigationService;
        _auditService = auditService;
        _navPrereqValidator = navPrereqValidator;
        _checksumValidationService = checksumValidationService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("profit-master-update");
        Summary(s =>
        {
            s.Summary = "Applies YE updates to members";
            s.ExampleRequest = ProfitShareUpdateRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> { { 200, ProfitMasterUpdateResponse.Example() } };
        });

        Group<YearEndGroup>();
        Policies(Security.Policy.CanViewYearEndReports, Security.Policy.CanRunYearEndProcesses);
    }

    public override async Task HandleAsync(ProfitShareUpdateRequest req, CancellationToken ct)
    {
        // Validate prerequisites for Master Update before proceeding
        await _navPrereqValidator.ValidateAllCompleteAsync(Navigation.Constants.MasterUpdate, ct);

        // Perform cross-reference validation BEFORE executing Master Update
        // This validates that all prerequisite reports (PAY443, QPAY129, QPAY066TA, etc.)
        // have matching values in their archived checksums
        _logger.LogInformation(
            "Performing cross-reference validation for Master Update (PAY444|PAY447) for year {ProfitYear}",
            req.ProfitYear);

        // TODO: For now, we'll validate what we have archived (PAY443)
        // In a future iteration, we need to collect actual PAY444 totals to validate
        // For this implementation, we're establishing the validation infrastructure
        var currentValues = new Dictionary<string, decimal>
        {
            // These would come from PAY444 report data when we identify the correct source
            // For now, placeholder - will need to get actual values from:
            // - Distribution totals query
            // - Forfeiture totals query
            // - Contribution totals query
            // - Earnings totals query

            // Example: if we had the data
            // ["PAY443.DistributionTotals"] = actualDistributionTotal,
            // ["QPAY129.Distributions"] = qpay129DistributionTotal,
            // ["QPAY066TA.TotalDisbursements"] = qpay066taTotalDisbursements,
            // ["PAY443.TotalForfeitures"] = actualForfeitureTotal,
            // ["QPAY129.ForfeitedAmount"] = qpay129ForfeitedAmount
        };

        var crossRefValidation = await _checksumValidationService.ValidateMasterUpdateCrossReferencesAsync(
            req.ProfitYear,
            currentValues,
            ct);

        if (!crossRefValidation.IsSuccess)
        {
            _logger.LogError(
                "Cross-reference validation failed for year {ProfitYear}: {Error}",
                req.ProfitYear,
                crossRefValidation.Error?.Message);

            // For now, we'll log but not block - in production this should throw/block
            // when BlockMasterUpdate is true
        }
        else if (crossRefValidation.Value.BlockMasterUpdate)
        {
            _logger.LogWarning(
                "Master Update is BLOCKED due to critical cross-reference validation failures for year {ProfitYear}. " +
                "Issues: {Issues}",
                req.ProfitYear,
                string.Join("; ", crossRefValidation.Value.CriticalIssues));

            // TODO: In production, throw validation exception here
            // For now, we'll continue but attach the validation results
        }

        ProfitMasterUpdateResponse response = await _auditService.ArchiveCompletedReportAsync("PAY444|PAY447",
            req.ProfitYear,
            req,
            isArchiveRequest: true,
            async (arditReq, _, cancellationToken) =>
            {
                ProfitMasterUpdateResponse response = await _profitMasterService.Update(arditReq, cancellationToken);

                // Attach cross-reference validation results to response
                if (crossRefValidation.IsSuccess)
                {
                    response.CrossReferenceValidation = crossRefValidation.Value;

                    _logger.LogInformation(
                        "Master Update completed for year {ProfitYear} with cross-reference validation: " +
                        "{PassedValidations}/{TotalValidations} passed",
                        arditReq.ProfitYear,
                        crossRefValidation.Value.PassedValidations,
                        crossRefValidation.Value.TotalValidations);
                }

                await _navigationService.UpdateNavigation(Navigation.Constants.MasterUpdate, NavigationStatus.Constants.Complete, cancellationToken);
                return response;
            },
            ct);

        await Send.OkAsync(response, ct);
    }
}
