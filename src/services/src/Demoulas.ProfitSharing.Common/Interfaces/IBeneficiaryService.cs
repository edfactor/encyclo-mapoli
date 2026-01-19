using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IBeneficiaryService
{
    Task<CreateBeneficiaryResponse> CreateBeneficiaryAsync(CreateBeneficiaryRequest req, CancellationToken cancellationToken);
    Task<CreateBeneficiaryContactResponse> CreateBeneficiaryContactAsync(CreateBeneficiaryContactRequest req, CancellationToken cancellationToken);
    Task<UpdateBeneficiaryResponse> UpdateBeneficiaryAsync(UpdateBeneficiaryRequest req, CancellationToken cancellationToken);
    Task<UpdateBeneficiaryContactResponse> UpdateBeneficiaryContactAsync(UpdateBeneficiaryContactRequest req, CancellationToken cancellationToken);

    Task DeleteBeneficiaryAsync(int id, CancellationToken cancellationToken);
    Task DeleteBeneficiaryContactAsync(int id, CancellationToken cancellation);

    /// <summary>
    /// Gets the total beneficiary percentage for a specific badge number, excluding a specific beneficiary if provided.
    /// </summary>
    /// <param name="badgeNumber">The employee badge number</param>
    /// <param name="beneficiaryIdToExclude">Optional: Beneficiary ID to exclude (e.g., when updating)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total percentage sum, or -1 if badge not found</returns>
    Task<decimal> GetBeneficiaryPercentageSumAsync(int badgeNumber, int? beneficiaryIdToExclude, CancellationToken cancellationToken);
}
