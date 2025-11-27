using System.ComponentModel;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Lookups;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Services;

/// <summary>
/// Unit tests for UnmaskingService to verify SSN unmasking functionality with proper authorization and audit logging.
/// Tests role-based access control, demographic lookup, and sensitive data audit trail creation.
/// </summary>
[Description("PS-2098 : Verify SSN unmasking service enforces authorization and audit logging")]
public sealed class UnmaskingServiceTests : IDisposable
{
    private readonly MockDataContextFactory _mockContextFactory;
    private readonly IUnmaskingService _unmaskingService;
    private readonly IAuditService _auditService;

    public UnmaskingServiceTests()
    {
        _mockContextFactory = new MockDataContextFactory();

        // Create mock services
        var mockAppUser = new MockAppUser { UserName = "TestUser", EffectiveRoles = [Role.SSN_UNMASKING] };
        var mockHttpContextAccessor = new MockHttpContextAccessor();

        _auditService = new Demoulas.ProfitSharing.Services.Audit.AuditService(
            _mockContextFactory,
            mockAppUser,
            mockHttpContextAccessor);

        _unmaskingService = new UnmaskingService(
            _mockContextFactory,
            new DemographicReaderService(_mockContextFactory),
            mockAppUser,
            _auditService);
    }

    #region Authorization Tests

    [Fact(DisplayName = "PS-2098 : Should return Failure when user lacks SSN-Unmasking role")]
    public async Task GetUnmaskedSsn_WithoutSsnUnmaskingRole_ReturnsFailure()
    {
        // Arrange - Create mock app user WITHOUT SSN-Unmasking role
        var mockAppUser = new MockAppUser
        {
            UserName = "RestrictedUser",
            EffectiveRoles = [Role.ADMINISTRATOR] // No SSN-Unmasking role
        };

        var serviceWithoutRole = new UnmaskingService(
            _mockContextFactory,
            new DemographicReaderService(_mockContextFactory),
            mockAppUser,
            _auditService);

        var demographicId = _mockContextFactory.GetFirstDemographicId();

        // Act
        var result = await serviceWithoutRole.GetUnmaskedSsn(demographicId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
    }

    [Fact(DisplayName = "PS-2098 : Should return Failure when demographic not found")]
    public async Task GetUnmaskedSsn_WithInvalidDemographicId_ReturnsNotFound()
    {
        // Arrange
        long invalidDemographicId = 999999999;

        // Act
        var result = await _unmaskingService.GetUnmaskedSsn(invalidDemographicId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
    }

    #endregion

    #region Happy Path Tests

    [Fact(DisplayName = "PS-2098 : Should return formatted SSN for authorized user")]
    public async Task GetUnmaskedSsn_WithValidDemographicIdAndRole_ReturnsFormattedSsn()
    {
        // Arrange
        var demographicId = _mockContextFactory.GetFirstDemographicId();

        // Get the expected SSN from mock data
        await using var ctx = await _mockContextFactory.CreateDbContextAsync(CancellationToken.None);
        ctx.UseReadOnlyContext();

        var demographic = await ctx.Demographics
            .Where(d => d.Id == demographicId)
            .Select(d => d.Ssn)
            .FirstOrDefaultAsync();

        // Act
        var result = await _unmaskingService.GetUnmaskedSsn(demographicId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNullOrEmpty();

        // Verify SSN is formatted (contains dashes)
        result.Value.ShouldMatch(@"^\d{3}-\d{2}-\d{4}$", "SSN should be formatted as XXX-XX-XXXX");
    }

    [Fact(DisplayName = "PS-2098 : Should create audit log when SSN is unmasked")]
    public async Task GetUnmaskedSsn_WithValidRequest_CreatesAuditLog()
    {
        // Arrange
        var demographicId = _mockContextFactory.GetFirstDemographicId();

        // Act
        var result = await _unmaskingService.GetUnmaskedSsn(demographicId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Verify audit log was created
        await using var auditCtx = await _mockContextFactory.CreateDbContextAsync(CancellationToken.None);
        auditCtx.UseReadOnlyContext();

        var auditEvent = await auditCtx.AuditEvents
            .Where(ae => ae.TableName == "Demographics" &&
                        ae.Operation.Contains("UnmaskSsn"))
            .OrderByDescending(ae => ae.CreatedAt)
            .FirstOrDefaultAsync();

        auditEvent.ShouldNotBeNull("Audit event should be created for SSN unmasking");
        auditEvent.UserName.ShouldBe("TestUser");
    }

    [Fact(DisplayName = "PS-2098 : Should include badge number in audit log primary key")]
    public async Task GetUnmaskedSsn_WithValidRequest_IncludesBadgeNumberInAuditLog()
    {
        // Arrange
        var demographicId = _mockContextFactory.GetFirstDemographicId();

        // Get the expected badge number from mock data
        await using var ctx = await _mockContextFactory.CreateDbContextAsync(CancellationToken.None);
        ctx.UseReadOnlyContext();

        var badgeNumber = await ctx.DemographicHistories
            .Where(dh => dh.DemographicId == demographicId)
            .Select(dh => dh.BadgeNumber)
            .FirstOrDefaultAsync();

        // Act
        var result = await _unmaskingService.GetUnmaskedSsn(demographicId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Verify audit log includes badge number
        await using var auditCtx = await _mockContextFactory.CreateDbContextAsync(CancellationToken.None);
        auditCtx.UseReadOnlyContext();

        var auditEvent = await auditCtx.AuditEvents
            .Where(ae => ae.TableName == "Demographics" &&
                        ae.Operation.Contains("UnmaskSsn"))
            .OrderByDescending(ae => ae.CreatedAt)
            .FirstOrDefaultAsync();

        auditEvent.ShouldNotBeNull();
        auditEvent.PrimaryKey.ShouldBe(badgeNumber.ToString(),
            "Audit log should use badge number as primary key for compliance");
    }

    #endregion

    #region Multiple Role Tests

    [Theory(DisplayName = "PS-2098 : Should succeed with any readonly role that has SSN-Unmasking")]
    [InlineData(Role.SSN_UNMASKING)]
    [InlineData(Role.HR_READONLY)]
    [InlineData(Role.AUDITOR)]
    [InlineData(Role.ITDEVOPS)]
    public async Task GetUnmaskedSsn_WithVariousReadonlyRoles_Succeeds(string roleToTest)
    {
        // Arrange - Create mock app user with specific role
        var mockAppUser = new MockAppUser
        {
            UserName = $"TestUser_{roleToTest}",
            EffectiveRoles = [roleToTest, Role.SSN_UNMASKING]
        };

        var serviceWithRole = new UnmaskingService(
            _mockContextFactory,
            new DemographicReaderService(_mockContextFactory),
            mockAppUser,
            _auditService);

        var demographicId = _mockContextFactory.GetFirstDemographicId();

        // Act
        var result = await serviceWithRole.GetUnmaskedSsn(demographicId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue($"Should succeed with role {roleToTest}");
    }

    #endregion

    #region Edge Cases

    [Fact(DisplayName = "PS-2098 : Should handle cancellation token gracefully")]
    public async Task GetUnmaskedSsn_WithCancelledToken_ReturnsCancellation()
    {
        // Arrange
        var demographicId = _mockContextFactory.GetFirstDemographicId();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(
            () => _unmaskingService.GetUnmaskedSsn(demographicId, cts.Token));
    }

    [Fact(DisplayName = "PS-2098 : Should handle zero demographic ID")]
    public async Task GetUnmaskedSsn_WithZeroDemographicId_ReturnsFailure()
    {
        // Act
        var result = await _unmaskingService.GetUnmaskedSsn(0, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
    }

    [Fact(DisplayName = "PS-2098 : Should handle negative demographic ID")]
    public async Task GetUnmaskedSsn_WithNegativeDemographicId_ReturnsFailure()
    {
        // Act
        var result = await _unmaskingService.GetUnmaskedSsn(-1, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
    }

    #endregion

    public void Dispose()
    {
        _mockContextFactory?.Dispose();
    }
}
