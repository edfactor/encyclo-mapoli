using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class PayClassificationLookupEndpoint : EndpointWithoutRequest<ISet<PayClassificationResponseDto>>
{
    private readonly IPayClassificationService _payClassificationService;

    public PayClassificationLookupEndpoint(IPayClassificationService payClassificationService)
    {
        _payClassificationService = payClassificationService;
    }

    public override void Configure()
    {
        Get("payclassification/all");
        Policies(Security.Policy.CanViewPayClassificationTypes);
        Summary(s =>
        {
            s.Summary = "Get all pay classifications";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<PayClassificationResponseDto>
                {
                    new PayClassificationResponseDto { Id = 0, Name = "Example"}
                }
            } };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR}, {Role.FINANCEMANAGER}, {Role.DISTRIBUTIONSCLERK}, or {Role.HARDSHIPADMINISTRATOR}";
        });
        Group<LookupGroup>();
    }

    public override Task<ISet<PayClassificationResponseDto>> ExecuteAsync(CancellationToken ct)
    {
        return _payClassificationService.GetAllPayClassificationsAsync(ct);
    }
}
