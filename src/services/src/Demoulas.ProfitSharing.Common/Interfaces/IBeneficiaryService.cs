using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IBeneficiaryService
{
    Task<CreateBeneficiaryResponse> CreateBeneficiary(CreateBeneficiaryRequest req, CancellationToken cancellationToken);
    Task<CreateBeneficiaryContactResponse> CreateBeneficiaryContact(CreateBeneficiaryContactRequest req, CancellationToken cancellationToken);
    Task<UpdateBeneficiaryResponse> UpdateBeneficiary(UpdateBeneficiaryRequest req, CancellationToken cancellationToken);
    Task<UpdateBeneficiaryContactResponse> UpdateBeneficiaryContact(UpdateBeneficiaryContactRequest req, CancellationToken cancellationToken);

    Task DeleteBeneficiary(int id, CancellationToken cancellationToken);
    Task DeleteBeneficiaryContact(int id, CancellationToken cancellation);
}
