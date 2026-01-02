using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for managing RMD (Required Minimum Distribution) factor data.
/// </summary>
public interface IRmdsFactorService
{
    /// <summary>
    /// Gets all RMD factors by age.
    /// </summary>
    Task<List<RmdsFactorDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single RMD factor by age.
    /// </summary>
    Task<RmdsFactorDto?> GetByAgeAsync(byte age, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates an RMD factor for a specific age.
    /// </summary>
    Task<RmdsFactorDto> UpsertAsync(RmdsFactorRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an RMD factor for a specific age.
    /// </summary>
    Task<bool> DeleteAsync(byte age, CancellationToken cancellationToken = default);
}
