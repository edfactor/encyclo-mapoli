using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;
public class DeleteBeneficiaryContactEndpoint : Endpoint<IdRequest>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public DeleteBeneficiaryContactEndpoint(IBeneficiaryService beneficiaryService)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Delete("contact/{Id}");
        Summary(s =>
        {
            s.Summary = "Deletes a beneficiary contact";
        });
        Group<BeneficiariesGroup>();
    }

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        await _beneficiaryService.DeleteBeneficiaryContact(req.Id, ct);
        await SendOkAsync(req, ct);
    }
}
