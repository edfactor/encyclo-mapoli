using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error, ListResponseDto
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class PayClassificationLookupEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<PayClassificationResponseDto>>
{
    private readonly IPayClassificationService _payClassificationService;

    public PayClassificationLookupEndpoint(IPayClassificationService payClassificationService) : base(Navigation.Constants.Inquiries)
    {
        _payClassificationService = payClassificationService;
    }

    public override void Configure()
    {
        Get("pay-classifications");
        Summary(s =>
        {
            s.Summary = "Get all pay classifications";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<PayClassificationResponseDto>
                {
                    new PayClassificationResponseDto { Id = "0", Name = "Example"}
                }
            } };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR}, {Role.FINANCEMANAGER}, {Role.DISTRIBUTIONSCLERK}, or {Role.HARDSHIPADMINISTRATOR}";
        });
        Group<LookupGroup>();

        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<Results<Ok<ListResponseDto<PayClassificationResponseDto>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        try
        {
            var set = await _payClassificationService.GetAllPayClassificationsAsync(ct);
            var dto = ListResponseDto<PayClassificationResponseDto>.From(set.OrderBy(x => x.Name));
            var result = Result<ListResponseDto<PayClassificationResponseDto>>.Success(dto);
            return result.ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<ListResponseDto<PayClassificationResponseDto>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
