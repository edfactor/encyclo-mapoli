using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;

public class CreateBeneficiaryAndContactEndpoint : ProfitSharingEndpoint<CreateBeneficiaryRequest, Results<Ok<CreateBeneficiaryResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public CreateBeneficiaryAndContactEndpoint(IBeneficiaryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }
    public override void Configure()
    {
        Post("/");
        Summary(s =>
        {
            s.Summary = "Adds a new beneficiary";
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, CreateBeneficiaryResponse.SampleResponse()}
            };
        });
        Group<BeneficiariesGroup>();
    }

    public override async Task<Results<Ok<CreateBeneficiaryResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(CreateBeneficiaryRequest req, CancellationToken ct)
    {
        try
        {
            var created = await _beneficiaryService.CreateBeneficiary(req, ct);
            return Result<CreateBeneficiaryResponse>.Success(created).ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<CreateBeneficiaryResponse>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
