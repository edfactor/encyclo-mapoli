using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IBeneficiaryDisbursementService
{
    Task<Result<bool>> DisburseFundsToBeneficiaries(BeneficiaryDisbursementRequest request, CancellationToken cancellationToken);
}
