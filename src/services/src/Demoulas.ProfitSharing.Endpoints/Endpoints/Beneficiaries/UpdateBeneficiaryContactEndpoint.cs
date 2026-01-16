using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;

public class UpdateBeneficiaryContactEndpoint : ProfitSharingEndpoint<UpdateBeneficiaryContactRequest, Results<Ok<UpdateBeneficiaryContactResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;
    public UpdateBeneficiaryContactEndpoint(IBeneficiaryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Put("/contact");
        Summary(s =>
        {
            s.Summary = "Updates beneficiary contact information";
            s.ExampleRequest = UpdateBeneficiaryContactRequest.SampleRequest();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, UpdateBeneficiaryContactResponse.SampleResponse() }
            };
        });
        Group<BeneficiariesGroup>();
    }

    protected override async Task<Results<Ok<UpdateBeneficiaryContactResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(UpdateBeneficiaryContactRequest req, CancellationToken ct)
    {
        try
        {
            var updated = await _beneficiaryService.UpdateBeneficiaryContact(req, ct);
            if (updated is null)
            {
                return Result<UpdateBeneficiaryContactResponse>.Failure(Error.EntityNotFound("BeneficiaryContact")).ToHttpResult(Error.EntityNotFound("BeneficiaryContact"));
            }
            return Result<UpdateBeneficiaryContactResponse>.Success(updated).ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<UpdateBeneficiaryContactResponse>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
