using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class BeneficiaryKindGroup : GroupBase
{
    protected override string Route => "beneficiary-kinds";
    protected override string RouteName => "Beneficiary Kinds";

    public BeneficiaryKindGroup()
    {
        Configure(Route.ToLowerInvariant(), ep =>
        {
            ep.Description(x => x
                .ProducesProblemFE<ProblemDetails>()
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithTags(RouteName));
            ep.Policies(Policy.CanRunMasterInquiry);
        });
    }
}
