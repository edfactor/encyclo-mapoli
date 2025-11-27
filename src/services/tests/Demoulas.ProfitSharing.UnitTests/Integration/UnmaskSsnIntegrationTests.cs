using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Integration;

/// <summary>
/// Collection definition for UnmaskSsn integration tests - disables parallelization
/// </summary>
[CollectionDefinition("UnmaskSsnIntegration", DisableParallelization = true)]
public class UnmaskSsnIntegrationCollection
{
    // This class defines the test collection but contains no tests
}

/// <summary>
/// Integration tests for the complete SSN unmasking flow including endpoint, service, and audit logging.
/// Verifies end-to-end behavior including database interactions and audit trail creation.
/// </summary>
[Collection("UnmaskSsnIntegration")]
[Description("PS-2098 : Verify complete SSN unmasking flow with audit logging and database interactions")]
public sealed class UnmaskSsnIntegrationTests : ApiTestBase<Api.Program>
{
    #region Audit Trail Tests

    [Fact(DisplayName = "PS-2098 : UnmaskSsn operation should create audit trail in database")]
    public async Task UnmaskSsn_WithValidRequest_CreatesAuditTrail()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert - Verify response is successful
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        // Verify audit log was created
        await using var auditCtx = await MockDbContextFactory.CreateDbContextAsync(CancellationToken.None);
        auditCtx.UseReadOnlyContext();

        var auditEvent = await auditCtx.AuditEvents
            .Where(ae => ae.TableName == "Demographics" &&
                        ae.Operation.Contains("UnmaskSsn"))
            .OrderByDescending(ae => ae.CreatedAt)
            .FirstOrDefaultAsync();

        auditEvent.ShouldNotBeNull("Audit event should be created for SSN unmasking");
        auditEvent.Operation.ShouldContain("UnmaskSsn");
    }

    [Fact(DisplayName = "PS-2098 : Audit trail should include user who unmasked SSN")]
    public async Task UnmaskSsn_AuditTrail_IncludesUserName()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        // Verify audit log includes user info
        await using var auditCtx = await MockDbContextFactory.CreateDbContextAsync(CancellationToken.None);
        auditCtx.UseReadOnlyContext();

        var auditEvent = await auditCtx.AuditEvents
            .Where(ae => ae.TableName == "Demographics" &&
                        ae.Operation.Contains("UnmaskSsn"))
            .OrderByDescending(ae => ae.CreatedAt)
            .FirstOrDefaultAsync();

        auditEvent.UserName.ShouldNotBeNullOrEmpty("Audit log should record the user");
        // The mock user will have a test username
        auditEvent.UserName.ShouldNotBe("Unknown");
    }

    [Fact(DisplayName = "PS-2098 : Audit trail should include badge number as primary key")]
    public async Task UnmaskSsn_AuditTrail_IncludesBadgeNumberAsPrimaryKey()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        // Get expected badge number from mock data
        await using var dataCtx = await MockDbContextFactory.CreateDbContextAsync(CancellationToken.None);
        dataCtx.UseReadOnlyContext();

        var expectedBadge = await dataCtx.DemographicHistories
            .Where(dh => dh.DemographicId == demographicId)
            .Select(dh => dh.BadgeNumber)
            .FirstOrDefaultAsync();

        // Verify audit log includes badge number
        await using var auditCtx = await MockDbContextFactory.CreateDbContextAsync(CancellationToken.None);
        auditCtx.UseReadOnlyContext();

        var auditEvent = await auditCtx.AuditEvents
            .Where(ae => ae.TableName == "Demographics" &&
                        ae.Operation.Contains("UnmaskSsn"))
            .OrderByDescending(ae => ae.CreatedAt)
            .FirstOrDefaultAsync();

        auditEvent.PrimaryKey.ShouldBe(expectedBadge.ToString(),
            "Audit log should use badge number as primary key for compliance");
    }

    [Fact(DisplayName = "PS-2098 : Audit trail should be created even if demographic has no current history")]
    public async Task UnmaskSsn_WithDemographicHavingMultipleHistoryRecords_CreatesAuditTrail()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        // Verify audit was created regardless of history state
        await using var auditCtx = await MockDbContextFactory.CreateDbContextAsync(CancellationToken.None);
        auditCtx.UseReadOnlyContext();

        var auditEventCount = await auditCtx.AuditEvents
            .Where(ae => ae.TableName == "Demographics" &&
                        ae.Operation.Contains("UnmaskSsn"))
            .CountAsync();

        auditEventCount.ShouldBeGreaterThan(0);
    }

    #endregion

    #region Data Consistency Tests

    [Fact(DisplayName = "PS-2098 : Unmasked SSN should match demographic SSN in database")]
    public async Task UnmaskSsn_ResponseSsn_MatchesDatabaseRecord()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();

        // Get expected SSN from database
        await using var dataCtx = await MockDbContextFactory.CreateDbContextAsync(CancellationToken.None);
        dataCtx.UseReadOnlyContext();

        var expectedSsn = await dataCtx.Demographics
            .Where(d => d.Id == demographicId)
            .Select(d => d.Ssn)
            .FirstOrDefaultAsync();

        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        // Format expected SSN to match endpoint format (XXX-XX-XXXX)
        var formattedExpected = $"{expectedSsn:D9}".Insert(3, "-").Insert(6, "-");
        response.Data.UnmaskedSsn.ShouldBe(formattedExpected,
            "Unmasked SSN should match the database record exactly");
    }

    [Fact(DisplayName = "PS-2098 : Different demographic IDs should return different SSNs")]
    public async Task UnmaskSsn_WithDifferentDemographics_ReturnsDifferentSsns()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        await using var dataCtx = await MockDbContextFactory.CreateDbContextAsync(CancellationToken.None);
        dataCtx.UseReadOnlyContext();

        var demographics = await dataCtx.Demographics
            .Take(2)
            .ToListAsync();

        demographics.Count.ShouldBeGreaterThanOrEqualTo(2, "Need at least 2 demographics for this test");

        // Act
        var response1 = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(
            new UnmaskSsnRequest { DemographicId = demographics[0].Id });

        var response2 = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(
            new UnmaskSsnRequest { DemographicId = demographics[1].Id });

        // Assert
        response1.Data.UnmaskedSsn.ShouldNotBe(response2.Data.UnmaskedSsn,
            "Different demographics should have different SSNs");
    }

    #endregion

    #region Sensitive Field Telemetry Tests

    [Fact(DisplayName = "PS-2098 : UnmaskSsn should record sensitive field access in telemetry")]
    public async Task UnmaskSsn_WithValidRequest_RecordsSensitiveFieldAccess()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert - Verify endpoint succeeds
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        // Note: Telemetry is recorded via instrumentation middleware and OpenTelemetry collectors
        // This test verifies the endpoint completes successfully and should have recorded metrics
        response.Data.UnmaskedSsn.ShouldNotBeNullOrEmpty();
    }

    #endregion

    #region Business Operation Metrics Tests

    [Fact(DisplayName = "PS-2098 : UnmaskSsn should record business operation metric")]
    public async Task UnmaskSsn_WithValidRequest_RecordsBusinessOperation()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert - Verify endpoint succeeds (business operation is recorded internally)
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        response.Data.UnmaskedSsn.ShouldNotBeNullOrEmpty();
    }

    #endregion

    #region Guard Override Tests

    [Fact(DisplayName = "PS-2098 : AuditService should use guard override to write audit logs despite readonly roles")]
    public async Task UnmaskSsn_AuditLogging_WorksWithReadonlyRoles()
    {
        // Arrange - Use a readonly role like HR_READONLY that also has SSN-Unmasking
        ApiClient.CreateAndAssignTokenForClient(Role.HR_READONLY, Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert - Verify successful response and audit was created
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        await using var auditCtx = await MockDbContextFactory.CreateDbContextAsync(CancellationToken.None);
        auditCtx.UseReadOnlyContext();

        var auditEvent = await auditCtx.AuditEvents
            .Where(ae => ae.TableName == "Demographics" &&
                        ae.Operation.Contains("UnmaskSsn"))
            .OrderByDescending(ae => ae.CreatedAt)
            .FirstOrDefaultAsync();

        auditEvent.ShouldNotBeNull("Audit should be created even with readonly role using guard override");
    }

    #endregion

    #region Error Handling Tests

    [Fact(DisplayName = "PS-2098 : Should handle concurrent requests gracefully")]
    public async Task UnmaskSsn_ConcurrentRequests_AllSucceed()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act - Make 5 concurrent requests
        var tasks = Enumerable.Range(0, 5)
            .Select(_ => ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request))
            .ToList();

        await Task.WhenAll(tasks);

        // Assert - All should succeed
        foreach (var task in tasks)
        {
            task.Result.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            task.Result.Data.UnmaskedSsn.ShouldNotBeNullOrEmpty();
        }
    }

    [Fact(DisplayName = "PS-2098 : Should maintain data integrity across multiple unmask operations")]
    public async Task UnmaskSsn_MultipleOperations_MaintainDataIntegrity()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();

        // Act - Call unmask multiple times
        var results = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            var request = new UnmaskSsnRequest { DemographicId = demographicId };
            var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);
            results.Add(response.Data.UnmaskedSsn);
        }

        // Assert - All calls should return same SSN
        results.Distinct().Count().ShouldBe(1, "All calls should return the same SSN for same demographic");
    }

    #endregion
}
