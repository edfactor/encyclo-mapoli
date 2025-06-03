using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;
public class DeleteBeneficiaryEndpoint:Endpoint<IdRequest>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public DeleteBeneficiaryEndpoint(IBeneficiaryService beneficiaryService)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Delete("/{Id}");
        Summary(s =>
        {
            s.Summary = "Deletes a beneficiary, and their contact record if this was their only named beneficiary";
        });
        Group<BeneficiariesGroup>();
    }

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        await _beneficiaryService.DeleteBeneficiary(req.Id, ct);
        await SendOkAsync(req, ct);
    }
}
