using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.YearEnd;

public class YearEndProcessFinalRunEndpoint : ProfitSharingRequestEndpoint<YearRequestWithRebuild>
{
    private readonly IYearEndService _yearEndService;

    public YearEndProcessFinalRunEndpoint(IYearEndService yearEndService)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        Post("final");
        Summary(s =>
        {
            s.Summary = "Commits year-end profit sharing calculations (FINAL RUN)";
            s.Description = @"Finalizes and commits the profit sharing calculations for the specified year by updating critical employee records.

**Updates the following fields:**
- **Earn Points**: Contribution allocation amounts (how much goes toward each employee's contribution)
- **ZeroContributionReason**: Why an employee received zero contribution (Normal, Under21, Terminated (Vest Only), Retired, Soon to be Retired)
- **EmployeeType**: Identifies new employees in the plan (first year >21 with >1000 hours)
- **PsCertificateIssuedDate**: Indicates employee should receive a printed certificate (proxy for Earn Points > 0)

**IMPORTANT USAGE NOTES:**
- ✅ **Safe to run multiple times** BEFORE Master Update is saved
- ⚠️ **DO NOT run** AFTER Master Update - this will cause incorrect earnings and contribution calculations
- This operation is idempotent until Master Update is executed

**Typical Workflow:**
1. Run year-end enrollment updates
2. Review Profit Share Summary report
3. Run this COMMIT/Final Run endpoint
4. Verify calculations
5. Execute Master Update (point of no return)

Use the 'rebuild' parameter to force recalculation even if already committed.";

            s.ExampleRequest = new YearRequestWithRebuild
            {
                ProfitYear = 2024,
                Rebuild = false
            };

            s.Responses[204] = "Success. Year-end final calculations committed.";
            s.Responses[400] = "Bad Request. Invalid profit year or request parameters.";
            s.Responses[500] = "Internal Server Error. Failed to process final run updates.";
        });
        Group<YearEndGroup>();
    }

    protected override Task HandleRequestAsync(YearRequestWithRebuild req, CancellationToken ct)
    {
        return _yearEndService.RunFinalYearEndUpdatesAsync(req.ProfitYear, req.Rebuild, ct);
    }
}
