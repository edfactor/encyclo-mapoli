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
    private readonly ILogger<ProfitMasterUpdateEndpoint> _logger;

    public ProfitMasterUpdateEndpoint(IProfitMasterService profitMasterUpdate,
        INavigationService navigationService,
        IAuditService auditService,
        INavigationPrerequisiteValidator navPrereqValidator,
        ILogger<ProfitMasterUpdateEndpoint> logger)
        : base(Navigation.Constants.MasterUpdate)
    {
        _profitMasterService = profitMasterUpdate;
        _navigationService = navigationService;
        _auditService = auditService;
        _navPrereqValidator = navPrereqValidator;
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
