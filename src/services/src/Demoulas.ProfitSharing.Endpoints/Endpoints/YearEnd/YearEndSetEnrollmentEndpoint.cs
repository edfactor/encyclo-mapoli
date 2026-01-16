using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.YearEnd;

public sealed class YearEndSetEnrollmentEndpoint : ProfitSharingRequestEndpoint<ProfitYearRequest>
{
    private readonly IYearEndService _yearEndService;

    public YearEndSetEnrollmentEndpoint(IYearEndService yearEndService)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        Post("/enrollments");
        Summary(s =>
        {
            s.Summary = "Updates the enrollment id of all members for the year";
            s.Description = "Updates the enrollment ID of all members for a given profit year. Accepts profit year as optional route parameter or in request body. Route parameter takes precedence if provided.";
        });
        Group<YearEndGroup>();

        // Wire up FluentValidation validator for profit year validation
        Validator<ProfitYearRequestValidator>();
    }

    protected override Task HandleRequestAsync(ProfitYearRequest req, CancellationToken ct)
    {
        return _yearEndService.UpdateEnrollmentId(req.ProfitYear, ct);
    }
}
