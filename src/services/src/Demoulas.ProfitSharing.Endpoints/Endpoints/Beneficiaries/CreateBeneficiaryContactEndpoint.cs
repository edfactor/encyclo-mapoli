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

public class CreateBeneficiaryContactEndpoint : ProfitSharingEndpoint<CreateBeneficiaryContactRequest, Results<Ok<CreateBeneficiaryContactResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public CreateBeneficiaryContactEndpoint(IBeneficiaryService beneficiaryService) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
    }
    public override void Configure()
    {
        Post("/contact");
        Summary(s =>
        {
            s.Summary = "Adds a new beneficiary contact";
            s.ExampleRequest = CreateBeneficiaryContactRequest.SampleRequest();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, CreateBeneficiaryContactResponse.SampleResponse()}
            };
        });
        Group<BeneficiariesGroup>();
    }

    public override async Task<Results<Ok<CreateBeneficiaryContactResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(CreateBeneficiaryContactRequest req, CancellationToken ct)
    {
        try
        {
            var created = await _beneficiaryService.CreateBeneficiaryContact(req, ct);
            return Result<CreateBeneficiaryContactResponse>.Success(created).ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<CreateBeneficiaryContactResponse>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
