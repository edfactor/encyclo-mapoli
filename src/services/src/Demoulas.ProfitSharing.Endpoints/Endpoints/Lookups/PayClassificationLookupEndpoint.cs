using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Services;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class PayClassificationLookupEndpoint : EndpointWithoutRequest<ISet<PayClassificationResponseDto>>
{
    private readonly PayClassificationService _payClassificationService;

    public PayClassificationLookupEndpoint(PayClassificationService payClassificationService)
    {
        _payClassificationService = payClassificationService;
    }

    public override void Configure()
    {
        Get("search");
        Summary(s =>
        {
            s.Summary = "Search for customers by either account number or customer name";
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
