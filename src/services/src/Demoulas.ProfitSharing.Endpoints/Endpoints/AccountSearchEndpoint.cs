using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints;

public class AccountSearchEndpoint : EndpointWithoutRequest
{
    public AccountSearchEndpoint()
    {
        
    }

    public override void Configure()
    {
        Get("hello");
       
    }

    public override Task<object?> ExecuteAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
