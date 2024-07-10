using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.PayProfit;

public class GetAllProfitEndpoint : Endpoint<PaginationRequestDto, PaginatedResponseDto<PayProfitResponseDto>?>
{
    private readonly IPayProfitService _payProfitService;

    public GetAllProfitEndpoint(IPayProfitService payProfitService)
    {
        _payProfitService = payProfitService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Post("all");
        Summary(s =>
        {
            s.Summary = "Get all the Profit";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<PayProfitResponseDto>()
            } };
        });
        Group<PayProfitGroup>();
    }

    public override Task<PaginatedResponseDto<PayProfitResponseDto>?> ExecuteAsync(PaginationRequestDto req, CancellationToken ct)
    {
        return _payProfitService.GetAllProfits(req, ct);
    }
}
