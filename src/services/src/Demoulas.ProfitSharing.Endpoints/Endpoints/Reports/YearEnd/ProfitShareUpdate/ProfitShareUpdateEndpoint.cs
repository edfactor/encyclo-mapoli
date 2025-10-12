using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareUpdate;

public class ProfitShareUpdateEndpoint
    : EndpointWithCsvTotalsBase<ProfitShareUpdateRequest,
        ProfitShareUpdateResponse,
        ProfitShareUpdateMemberResponse,
        ProfitShareUpdateEndpoint.ProfitShareUpdateClassMap>
{
    private readonly IProfitShareUpdateService _profitShareUpdateService;
    private readonly IChecksumValidationService _checksumValidationService;
    private readonly ILogger<ProfitShareUpdateEndpoint> _logger;

    public ProfitShareUpdateEndpoint(
        IProfitShareUpdateService profitShareUpdateService,
        IChecksumValidationService checksumValidationService,
        ILogger<ProfitShareUpdateEndpoint> logger)
        : base(Navigation.Constants.ProfitShareReportEditRun)
    {
        _profitShareUpdateService = profitShareUpdateService;
        _checksumValidationService = checksumValidationService;
        _logger = logger;
    }

    public override string ReportFileName => "profit-sharing-update-report";

    public override void Configure()
    {
        Get("profit-sharing-update");
        Summary(s =>
        {
            s.Summary = "profit sharing update";
            s.Description =
                "Updates plan members based on points provided.";
            s.ExampleRequest = ProfitShareUpdateRequest.RequestExample();
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();

        base.Configure();
    }

    public override async Task<ProfitShareUpdateResponse> GetResponse(ProfitShareUpdateRequest req, CancellationToken ct)
    {
        // Get the Master Update preview data with totals
        var response = await _profitShareUpdateService.ProfitShareUpdate(req, ct);

        // Perform cross-reference validation using the totals from the response
        _logger.LogInformation(
            "Performing cross-reference validation for Master Update preview year {ProfitYear}",
            req.ProfitYear);

        // IMPORTANT: Use PAY443 field names as keys because the validation service compares current PAY444 values
        // against archived PAY443 checksums FOR THE SAME PROFIT YEAR (except Beginning Balance which compares to previous year).
        // The service looks up values using "PAY443.{fieldName}" or "PAY444.{fieldName}" keys.
        var currentValues = new Dictionary<string, decimal>
        {
            // Beginning Balance: PAY444 (2024) beginning balance is compared against PAY443 (2023) ending balance
            // The validation service looks for "PAY444.BeginningBalance (Year 2024)" key
            [$"PAY444.BeginningBalance (Year {req.ProfitYear})"] = response.ProfitShareUpdateTotals.BeginningBalance,

            // Distributions: PAY444 (2024) is compared against PAY443 (2024) archived data
            ["PAY443.DistributionTotals"] = response.ProfitShareUpdateTotals.Distributions,

            // Forfeitures: PAY444 (2024) is compared against PAY443 (2024) archived data
            ["PAY443.TotalForfeitures"] = response.ProfitShareUpdateTotals.Forfeiture,

            // Contributions: PAY444 (2024) is compared against PAY443 (2024) archived data
            ["PAY443.TotalContributions"] = response.ProfitShareUpdateTotals.TotalContribution,

            // Earnings: PAY444 (2024) is compared against PAY443 (2024) archived data
            ["PAY443.TotalEarnings"] = response.ProfitShareUpdateTotals.Earnings + response.ProfitShareUpdateTotals.Earnings2
        }; var crossRefValidation = await _checksumValidationService.ValidateMasterUpdateCrossReferencesAsync(
            req.ProfitYear,
            currentValues,
            ct);

        if (!crossRefValidation.IsSuccess)
        {
            _logger.LogWarning(
                "Cross-reference validation failed for Master Update preview year {ProfitYear}: {ErrorDescription}",
                req.ProfitYear,
                crossRefValidation.Error?.Description ?? "Unknown error");
        }
        else if (crossRefValidation.Value is not null)
        {
            // Attach validation results to response so UI can display them
            response.CrossReferenceValidation = crossRefValidation.Value;

            _logger.LogInformation(
                "Master Update preview validation completed for year {ProfitYear}: " +
                "{PassedValidations}/{TotalValidations} passed, BlockMasterUpdate: {BlockMasterUpdate}",
                req.ProfitYear,
                crossRefValidation.Value.PassedValidations,
                crossRefValidation.Value.TotalValidations,
                crossRefValidation.Value.BlockMasterUpdate);
        }

        return response;
    }

    public class ProfitShareUpdateClassMap : ClassMap<ProfitShareUpdateMemberResponse>
    {
        public ProfitShareUpdateClassMap()
        {
            Map(m => m.Psn).Index(0).Name("Number");
            Map(m => m.Name).Index(1).Name("Name");
            Map(m => m.BeginningAmount).Index(2).Name("Beginning Balance");
            Map(m => m.Contributions).Index(3).Name("Contributions");
            Map(m => m.AllEarnings).Index(4).Name("Earning");
            Map(m => m.AllSecondaryEarnings).Index(5).Name("Earning2");
            Map(m => m.IncomingForfeitures).Index(6).Name("Forfeits");
            Map(m => m.Distributions).Index(7).Name("Distributions");
            Map(m => m.Military).Index(8).Name("Military / Paid Alloc");
            Map(m => m.EndingBalance).Index(9).Name("Ending Balance");
        }
    }
}
