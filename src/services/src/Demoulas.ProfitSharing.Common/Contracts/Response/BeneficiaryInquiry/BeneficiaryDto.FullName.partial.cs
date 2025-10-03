using Demoulas.ProfitSharing.Common.Contracts.Shared;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

public partial record BeneficiaryDto
{
    // If FullName was not populated externally, compute on demand via extension logic.
    public string EffectiveFullName => FullName ?? this.ComputeFullName();
}
