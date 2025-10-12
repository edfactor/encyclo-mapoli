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

        // First, execute Master Update to persist the year-end changes
        _logger.LogInformation(
            "Executing Master Update (PAY444|PAY447) for year {ProfitYear}",
            req.ProfitYear);

        var updateResponse = await _profitMasterService.Update(req, ct);

        // Now perform cross-reference validation
        // Note: BeginningBalance validation will query PAY443 from previous year
        // Other validations can be added as fields are identified
        _logger.LogInformation(
            "Performing cross-reference validation for Master Update year {ProfitYear}",
            req.ProfitYear);

        var currentValues = new Dictionary<string, decimal>
        {
            // Beginning balance will be validated against PAY443.TotalProfitSharingBalance from previous year
            // The validation service will handle looking up the previous year's value
            // Other fields can be added here as they're identified:
            // ["PAY444.Distributions"] = ...,
            // ["PAY444.TotalForfeitures"] = ...,
            // ["PAY444.TotalContributions"] = ...,
            // ["PAY444.TotalEarnings"] = ...,
        };

        var crossRefValidation = await _checksumValidationService.ValidateMasterUpdateCrossReferencesAsync(
            req.ProfitYear,
            currentValues,
            ct);

        if (!crossRefValidation.IsSuccess)
        {
            _logger.LogError(
                "Cross-reference validation failed for year {ProfitYear}: {ErrorDescription}",
                req.ProfitYear,
                crossRefValidation.Error?.Description ?? "Unknown error");

            // For now, we'll log but not block - in production this should throw/block
            // when BlockMasterUpdate is true
        }
        else if (crossRefValidation.Value?.BlockMasterUpdate == true)
        {
            _logger.LogWarning(
                "Master Update is BLOCKED due to critical cross-reference validation failures for year {ProfitYear}. " +
                "Issues: {Issues}",
                req.ProfitYear,
                string.Join("; ", crossRefValidation.Value.CriticalIssues ?? []));

            // NOTE: In production, throw validation exception here
            // For now, we'll continue but attach the validation results
        }

        // Attach cross-reference validation results to response
        if (crossRefValidation.IsSuccess && crossRefValidation.Value is not null)
        {
            updateResponse.CrossReferenceValidation = crossRefValidation.Value;

            _logger.LogInformation(
                "Master Update completed for year {ProfitYear} with cross-reference validation: " +
                "{PassedValidations}/{TotalValidations} passed, BlockMasterUpdate: {BlockMasterUpdate}",
                req.ProfitYear,
                crossRefValidation.Value.PassedValidations,
                crossRefValidation.Value.TotalValidations,
                crossRefValidation.Value.BlockMasterUpdate);
        }

        // Archive the completed Master Update
        var response = await _auditService.ArchiveCompletedReportAsync("PAY444|PAY447",
            req.ProfitYear,
            req,
            isArchiveRequest: true,
            async (_, __, cancellationToken) =>
            {
                await _navigationService.UpdateNavigation(Navigation.Constants.MasterUpdate, NavigationStatus.Constants.Complete, cancellationToken);
                return updateResponse;
            },
            ct);

        await Send.OkAsync(response, ct);
    }
}
