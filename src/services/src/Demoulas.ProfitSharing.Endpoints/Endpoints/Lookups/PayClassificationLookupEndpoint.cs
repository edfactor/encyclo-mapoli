using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
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
        AllowAnonymous();
        Get("payclassification/all");
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
        });
        Group<LookupGroup>();
    }

    public override Task<ISet<PayClassificationResponseDto>> ExecuteAsync(CancellationToken ct)
    {
        return _payClassificationService.GetAllPayClassifications(ct);
    }
}
