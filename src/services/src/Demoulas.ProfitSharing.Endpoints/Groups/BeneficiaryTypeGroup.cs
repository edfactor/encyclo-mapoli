using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class BeneficiaryTypeGroup : GroupBase
{
    protected override string Route => "beneficiary-types";
    protected override string RouteName => "Beneficiary Types";

    public BeneficiaryTypeGroup()
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
