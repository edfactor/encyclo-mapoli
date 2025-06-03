using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IBeneficiaryService
{
    Task<CreateBeneficiaryResponse> CreateBeneficiary(CreateBeneficiaryRequest req, CancellationToken cancellationToken);
    Task UpdateBeneficiary(UpdateBeneficiaryRequest req, CancellationToken cancellationToken);
    Task DeleteBeneficiary(int id, CancellationToken cancellationToken);
    Task DeleteBeneficiaryContact(int id, CancellationToken cancellation);
}
