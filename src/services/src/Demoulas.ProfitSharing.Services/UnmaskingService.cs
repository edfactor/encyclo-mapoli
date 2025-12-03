using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Audit;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

/// <summary>
/// Service for unmasking sensitive demographic data (SSN) for compliance and verification purposes.
/// Restricted to users with SSN-Unmasking role via policy authorization and server-side role validation.
/// </summary>
public sealed class UnmaskingService : IUnmaskingService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly IAppUser _appUser;
    private readonly IAuditService _auditService;

    public UnmaskingService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        IAppUser appUser,
        IAuditService auditService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _appUser = appUser;
        _auditService = auditService;
    }

    /// <summary>
    /// Gets the unmasked, formatted SSN for a demographic record.
    /// Defense in depth: Validates user has SSN-Unmasking role both at policy level and here.
    /// Access is logged to audit trail for compliance and security monitoring.
    /// </summary>
    public async Task<Result<string>> GetUnmaskedSsnAsync(int demographicId, CancellationToken cancellationToken = default)
    {
        // Defense in depth: Re-validate server-side that user has the required role
        // This ensures protection even if policy layer is somehow bypassed
        var userRoles = _appUser.GetUserAllRoles() ?? new List<string>();
        if (!userRoles.Contains(Role.SSN_UNMASKING, StringComparer.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("User does not have SSN-Unmasking role");
        }

        // Fetch demographic data in single query
        var demographicData = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx);

            return await demographicQuery
                .TagWith($"UnmaskSsn-DemographicId:{demographicId}")
                .Where(d => d.Id == demographicId)
                .Select(d => new { d.BadgeNumber, d.Ssn })
                .FirstOrDefaultAsync(cancellationToken);
        }, cancellationToken);

        if (demographicData == null || demographicData.BadgeNumber == 0)
        {
            return Result<string>.Failure(Error.EntityNotFound("Demographic"));
        }

        var result = await _auditService.LogSensitiveDataAccessAsync(
            operationName: "Get Unmasked SSN",
            tableName: "Demographic",
            primaryKey: demographicData.BadgeNumber.ToString(),
            details: $"User requested unmasked SSN for badge number {demographicData.BadgeNumber} for compliance/verification purposes",
            operation: async ct =>
            {
                if (demographicData.Ssn == 0)
                {
                    return Result<string>.Failure(Error.EntityNotFound("Demographic"));
                }

                // Format SSN (e.g., 1234567890 -> "123-45-6789")
                var formattedSsn = FormatSsn(demographicData.Ssn);

                return Result<string>.Success(formattedSsn);
            },
            cancellationToken);

        return result;
    }

    /// <summary>
    /// Formats an unformatted SSN integer into the standard "XXX-XX-XXXX" format.
    /// Example: 1234567890 -> "123-45-6789"
    /// </summary>
    private static string FormatSsn(int ssn)
    {
        var ssnStr = ssn.ToString("D9"); // Pad to 9 digits with leading zeros
        return $"{ssnStr[..3]}-{ssnStr.Substring(3, 2)}-{ssnStr.Substring(5, 4)}";
    }
}
