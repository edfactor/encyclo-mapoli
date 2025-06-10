using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;
public sealed class UpdateBeneficiaryEndpoint : Endpoint<UpdateBeneficiaryRequest>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public UpdateBeneficiaryEndpoint(IBeneficiaryService beneficiaryService)
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

    public async override Task HandleAsync(UpdateBeneficiaryRequest req, CancellationToken ct)
    {
        await _beneficiaryService.UpdateBeneficiary(req, ct);
        await SendOkAsync(req, ct);
    }
}
