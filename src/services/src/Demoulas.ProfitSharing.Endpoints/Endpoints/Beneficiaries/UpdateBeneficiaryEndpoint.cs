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

public sealed class UpdateBeneficiaryEndpoint : ProfitSharingEndpoint<UpdateBeneficiaryRequest, Results<Ok<UpdateBeneficiaryResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public UpdateBeneficiaryEndpoint(IBeneficiaryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Put("/");
        Summary(s =>
        {
            s.Summary = "Updates beneficiary information";
            s.ExampleRequest = UpdateBeneficiaryRequest.SampleRequest();
        });
        Group<BeneficiariesGroup>();
    }

    public override async Task<Results<Ok<UpdateBeneficiaryResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(UpdateBeneficiaryRequest req, CancellationToken ct)
    {
        try
        {
            var updated = await _beneficiaryService.UpdateBeneficiary(req, ct);
            // If service returns null when not found, map to Result failure with specific not-found error
            if (updated is null)
            {
                return Result<UpdateBeneficiaryResponse>.Failure(Error.EntityNotFound("Beneficiary")).ToHttpResult(Error.EntityNotFound("Beneficiary"));
            }
            return Result<UpdateBeneficiaryResponse>.Success(updated).ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<UpdateBeneficiaryResponse>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
