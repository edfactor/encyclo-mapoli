using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;
public sealed class UpdateBeneficiaryPercentageEndpoint : Endpoint<UpdateBeneficiaryPercentageRequest, UpdateBeneficiaryPercentageResponse>
{
    private readonly IBeneficiaryService _beneficiaryService;

    public UpdateBeneficiaryPercentageEndpoint(IBeneficiaryService beneficiaryService)
    {
        _beneficiaryService = beneficiaryService;
    }

    public override void Configure()
    {
        Put("/percentage");
        Summary(s =>
        {
            s.Summary = "Updates beneficiary percentage";
            s.ExampleRequest = UpdateBeneficiaryPercentageRequest.SampleRequest();
        });
        Group<BeneficiariesGroup>();
    }

    public override Task<UpdateBeneficiaryPercentageResponse> ExecuteAsync(UpdateBeneficiaryPercentageRequest req, CancellationToken ct)
    {
        return _beneficiaryService.UpdateBeneficiaryPercentage(req, ct);
    }
}
