using System.ComponentModel;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Fakes;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="ForfeitureAdjustmentService"/>.
/// </summary>
[Collection("SharedGlobalState")]
public class ForfeitureAdjustmentServiceTests : ApiTestBase<Api.Program>
{
    private readonly TotalService _totalService;
    private readonly Demographic _demographic1;
    private readonly Demographic _demographic2;
    private readonly ProfitDetail _profitDetail1;
    private readonly ProfitDetail _profitDetail2;
    private readonly PayProfit _payProfit1;
    private readonly PayProfit _payProfit2;

    public ForfeitureAdjustmentServiceTests()
    {
        // Initialize telemetry for testing
        _ = EndpointTelemetry.BusinessOperationsTotal;

        // Get real TotalService instance from DI container (sealed class, cannot mock)
        _totalService = ServiceProvider?.GetRequiredService<TotalService>()!;

        // Create test data using Fakers
        _demographic1 = new DemographicFaker().UseSeed(1).Generate();
        _demographic1.Id = 1;
        _demographic1.OracleHcmId = 1;

        _demographic2 = new DemographicFaker().UseSeed(2).Generate();
        _demographic2.Id = 2;
        _demographic2.OracleHcmId = 2;

        var profitDetailFaker = new ProfitDetailFaker([_demographic1, _demographic2]).UseSeed(1);
        _profitDetail1 = profitDetailFaker.Generate();
        _profitDetail1.Id = 1;
        _profitDetail1.Ssn = _demographic1.Ssn;
        _profitDetail1.ProfitYear = 2025;
        _profitDetail1.ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id;
        _profitDetail1.Contribution = 1000m;
        _profitDetail1.Earnings = 500m;
        _profitDetail1.Forfeiture = 200m;

        _profitDetail2 = profitDetailFaker.Generate();
        _profitDetail2.Id = 2;
        _profitDetail2.Ssn = _demographic2.Ssn;
        _profitDetail2.ProfitYear = 2025;
        _profitDetail2.ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id;
        _profitDetail2.Contribution = -500m;
        _profitDetail2.Earnings = -250m;
        _profitDetail2.Forfeiture = -100m;

        var payProfitFaker = new PayProfitFaker([_demographic1, _demographic2]).UseSeed(1);
        _payProfit1 = payProfitFaker.Generate();
        _payProfit1.DemographicId = _demographic1.Id;
        _payProfit1.ProfitYear = 2025;
        _payProfit1.Etva = 5000m;
        _payProfit1.VestingScheduleId = VestingSchedule.Constants.NewPlan;
        _payProfit1.HasForfeited = false;

        _payProfit2 = payProfitFaker.Generate();
        _payProfit2.DemographicId = _demographic2.Id;
        _payProfit2.ProfitYear = 2025;
        _payProfit2.Etva = 3000m;
        _payProfit2.VestingScheduleId = VestingSchedule.Constants.NewPlan;
        _payProfit2.HasForfeited = false;
    }

    #region GetSuggestedForfeitureAmount Tests

    [Fact]
    [Description("PS-1331 : Employee not found returns failure")]
    public async Task GetSuggestedForfeitureAmount_EmployeeNotFound_ReturnsFailure()
    {
        // Arrange
        ScenarioDataContextFactory mockDbContextFactory = (ScenarioDataContextFactory)new ScenarioFactory
        {
            Demographics = [_demographic1, _demographic2]
        }.BuildMocks();

        var frozenServiceMock = new Mock<IFrozenService>(MockBehavior.Strict);
        frozenServiceMock.Setup(f => f.GetActiveFrozenDemographicAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FrozenStateResponse
            {
                Id = 1,
                ProfitYear = 2025,
                IsActive = true,
                AsOfDateTime = DateTimeOffset.UtcNow,
                CreatedDateTime = DateTimeOffset.UtcNow
            });

        var demographicReaderMock = new Mock<IDemographicReaderService>(MockBehavior.Strict);
        demographicReaderMock.Setup(d => d.BuildDemographicQueryAsync(It.IsAny<IProfitSharingDbContext>(), It.IsAny<bool>()))
            .ReturnsAsync(mockDbContextFactory.ProfitSharingDbContext.Object.Demographics.AsQueryable());

        var mockAppUser = new Mock<IAppUser>();
        var mockAuditService = new Mock<IProfitSharingAuditService>();

        var service = new ForfeitureAdjustmentService(
            mockDbContextFactory,
            _totalService,
            demographicReaderMock.Object,
            TimeProvider.System,
            mockAppUser.Object,
            mockAuditService.Object);

        var request = new SuggestedForfeitureAdjustmentRequest
        {
            Badge = 999999999 // Non-existent badge
        };

        // Act
        var result = await service.GetSuggestedForfeitureAmount(request, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
    }

    [Fact]
    [Description("PS-XXXX : Search by badge number succeeds")]
    public async Task GetSuggestedForfeitureAmount_SearchByBadge_Succeeds()
    {
        // Arrange
        ScenarioDataContextFactory mockDbContextFactory = (ScenarioDataContextFactory)new ScenarioFactory
        {
            Demographics = [_demographic1, _demographic2],
            PayProfits = [_payProfit1, _payProfit2],
            ProfitDetails = [_profitDetail1, _profitDetail2]
        }.BuildMocks();

        var frozenServiceMock = new Mock<IFrozenService>(MockBehavior.Strict);
        frozenServiceMock.Setup(f => f.GetActiveFrozenDemographicAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FrozenStateResponse
            {
                Id = 1,
                ProfitYear = 2025,
                IsActive = true,
                AsOfDateTime = DateTimeOffset.UtcNow,
                CreatedDateTime = DateTimeOffset.UtcNow
            });

        var demographicReaderMock = new Mock<IDemographicReaderService>(MockBehavior.Strict);
        demographicReaderMock.Setup(d => d.BuildDemographicQueryAsync(It.IsAny<IProfitSharingDbContext>(), It.IsAny<bool>()))
            .ReturnsAsync(mockDbContextFactory.ProfitSharingDbContext.Object.Demographics.AsQueryable());

        var mockAppUser = new Mock<IAppUser>();
        var mockAuditService = new Mock<IProfitSharingAuditService>();

        var service = new ForfeitureAdjustmentService(
            mockDbContextFactory,
            _totalService,
            demographicReaderMock.Object,
            TimeProvider.System,
            mockAppUser.Object,
            mockAuditService.Object
        );

        var request = new SuggestedForfeitureAdjustmentRequest
        {
            Badge = _demographic1.BadgeNumber
        };

        // Act
        var result = await service.GetSuggestedForfeitureAmount(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BadgeNumber.ShouldBe(_demographic1.BadgeNumber);
    }

    [Fact]
    [Description("PS-XXXX : Class action forfeiture is excluded from last transaction lookup")]
    public async Task GetSuggestedForfeitureAmount_ExcludesClassActionForfeiture_CalculatesBasedOnVesting()
    {
        // Arrange
        // Create a class action forfeiture record (should be ignored)
        var classActionProfitDetail = new ProfitDetailFaker([_demographic1, _demographic2]).UseSeed(10).Generate();
        classActionProfitDetail.Id = 100;
        classActionProfitDetail.Ssn = _demographic1.Ssn;
        classActionProfitDetail.ProfitYear = 2025;
        classActionProfitDetail.ProfitCodeId = 2; // Forfeiture code
        classActionProfitDetail.Forfeiture = 1000m; // Positive forfeit amount
        classActionProfitDetail.CommentTypeId = CommentType.Constants.ForfeitClassAction; // Class action - should be excluded

        ScenarioDataContextFactory mockDbContextFactory = (ScenarioDataContextFactory)new ScenarioFactory
        {
            Demographics = [_demographic1, _demographic2],
            PayProfits = [_payProfit1, _payProfit2],
            ProfitDetails = [_profitDetail1, _profitDetail2, classActionProfitDetail]
        }.BuildMocks();

        var frozenServiceMock = new Mock<IFrozenService>(MockBehavior.Strict);
        frozenServiceMock.Setup(f => f.GetActiveFrozenDemographicAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FrozenStateResponse
            {
                Id = 1,
                ProfitYear = 2025,
                IsActive = true,
                AsOfDateTime = DateTimeOffset.UtcNow,
                CreatedDateTime = DateTimeOffset.UtcNow
            });

        var demographicReaderMock = new Mock<IDemographicReaderService>(MockBehavior.Strict);
        demographicReaderMock.Setup(d => d.BuildDemographicQueryAsync(It.IsAny<IProfitSharingDbContext>(), It.IsAny<bool>()))
            .ReturnsAsync(mockDbContextFactory.ProfitSharingDbContext.Object.Demographics.AsQueryable());

        var mockAppUser = new Mock<IAppUser>();
        var mockAuditService = new Mock<IProfitSharingAuditService>();

        var service = new ForfeitureAdjustmentService(
            mockDbContextFactory,
            _totalService,
            demographicReaderMock.Object,
            TimeProvider.System,
            mockAppUser.Object,
            mockAuditService.Object
        );

        var request = new SuggestedForfeitureAdjustmentRequest
        {
            Badge = _demographic1.BadgeNumber
        };

        // Act
        var result = await service.GetSuggestedForfeitureAmount(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BadgeNumber.ShouldBe(_demographic1.BadgeNumber);
        // The suggested amount should NOT be the negative of the class action forfeiture
        // It should be calculated based on vesting instead (or be 0 if no other forfeitures exist)
        result.Value.SuggestedForfeitAmount.ShouldNotBe(-1000m);
    }

    #endregion

    #region UpdateForfeitureAdjustmentAsync Tests

    [Fact]
    [Description("PS-XXXX : Zero amount returns failure")]
    public async Task UpdateForfeitureAdjustmentAsync_ZeroAmount_ReturnsFailure()
    {
        // Arrange
        MockDbContextFactory = new ScenarioFactory
        {
            Demographics = [_demographic1]
        }.BuildMocks();

        var demographicReaderMock = new Mock<IDemographicReaderService>(MockBehavior.Strict);

        var mockAppUser = new Mock<IAppUser>();
        var mockAuditService = new Mock<IProfitSharingAuditService>();

        var service = new ForfeitureAdjustmentService(
            MockDbContextFactory,
            _totalService,
            demographicReaderMock.Object,
            TimeProvider.System,
            mockAppUser.Object,
            mockAuditService.Object
        );


        var request = new ForfeitureAdjustmentUpdateRequest
        {
            BadgeNumber = _demographic1.BadgeNumber,
            ForfeitureAmount = 0m, // Invalid zero amount
            ClassAction = false
        };

        // Act
        var result = await service.UpdateForfeitureAdjustmentAsync(request, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
    }

    [Fact]
    [Description("PS-XXXX : Employee not found returns failure")]
    public async Task UpdateForfeitureAdjustmentAsync_EmployeeNotFound_ReturnsFailure()
    {
        // Arrange
        ScenarioDataContextFactory mockDbContextFactory = (ScenarioDataContextFactory)new ScenarioFactory
        {
            Demographics = [_demographic1]
        }.BuildMocks();

        var frozenServiceMock = new Mock<IFrozenService>(MockBehavior.Strict);
        frozenServiceMock.Setup(f => f.GetActiveFrozenDemographicAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FrozenStateResponse
            {
                Id = 1,
                ProfitYear = 2025,
                IsActive = true,
                AsOfDateTime = DateTimeOffset.UtcNow,
                CreatedDateTime = DateTimeOffset.UtcNow
            });

        var demographicReaderMock = new Mock<IDemographicReaderService>(MockBehavior.Strict);
        demographicReaderMock.Setup(d => d.BuildDemographicQueryAsync(It.IsAny<IProfitSharingDbContext>(), It.IsAny<bool>()))
            .ReturnsAsync(mockDbContextFactory.ProfitSharingDbContext.Object.Demographics.AsQueryable());

        var mockAppUser = new Mock<IAppUser>();
        var mockAuditService = new Mock<IProfitSharingAuditService>();

        var service = new ForfeitureAdjustmentService(
            mockDbContextFactory,
            _totalService,
            demographicReaderMock.Object,
            TimeProvider.System,
            mockAppUser.Object,
            mockAuditService.Object
        );

        var request = new ForfeitureAdjustmentUpdateRequest
        {
            BadgeNumber = 999999, // Non-existent badge
            ForfeitureAmount = 1000m,
            ClassAction = false
        };

        // Act
        var result = await service.UpdateForfeitureAdjustmentAsync(request, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
    }

    [Fact]
    [Description("PS-2543 : Should reject un-forfeit when profit detail has already been reversed")]
    public async Task UpdateForfeitureAdjustmentAsync_WhenProfitDetailAlreadyReversed_ReturnsFailure()
    {
        // Arrange
        // Create a forfeiture record that we want to un-forfeit
        var forfeitureRecord = new ProfitDetailFaker([_demographic1]).UseSeed(20).Generate();
        forfeitureRecord.Id = 200;
        forfeitureRecord.Ssn = _demographic1.Ssn;
        forfeitureRecord.ProfitYear = 2025;
        forfeitureRecord.ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id;
        forfeitureRecord.Forfeiture = 1000m;
        forfeitureRecord.CommentTypeId = CommentType.Constants.Forfeit.Id;
        forfeitureRecord.ReversedFromProfitDetailId = null;

        // Create an existing reversal that already points to the forfeiture record
        var existingReversal = new ProfitDetailFaker([_demographic1]).UseSeed(21).Generate();
        existingReversal.Id = 201;
        existingReversal.Ssn = _demographic1.Ssn;
        existingReversal.ProfitYear = 2025;
        existingReversal.ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id;
        existingReversal.Forfeiture = -1000m;
        existingReversal.CommentTypeId = CommentType.Constants.Unforfeit.Id;
        existingReversal.ReversedFromProfitDetailId = forfeitureRecord.Id; // This reversal came from the forfeiture

        ScenarioDataContextFactory mockDbContextFactory = (ScenarioDataContextFactory)new ScenarioFactory
        {
            Demographics = [_demographic1, _demographic2],
            PayProfits = [_payProfit1, _payProfit2],
            ProfitDetails = [_profitDetail1, _profitDetail2, forfeitureRecord, existingReversal]
        }.BuildMocks();

        var frozenServiceMock = new Mock<IFrozenService>(MockBehavior.Strict);
        frozenServiceMock.Setup(f => f.GetActiveFrozenDemographicAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FrozenStateResponse
            {
                Id = 1,
                ProfitYear = 2025,
                IsActive = true,
                AsOfDateTime = DateTimeOffset.UtcNow,
                CreatedDateTime = DateTimeOffset.UtcNow
            });

        var demographicReaderMock = new Mock<IDemographicReaderService>(MockBehavior.Strict);
        demographicReaderMock.Setup(d => d.BuildDemographicQueryAsync(It.IsAny<IProfitSharingDbContext>(), It.IsAny<bool>()))
            .ReturnsAsync(mockDbContextFactory.ProfitSharingDbContext.Object.Demographics.AsQueryable());

        // Forfeiture adjustment is always live (using wall clock year.)
        var fixedTime = new DateTimeOffset(2025, 12, 15, 10, 30, 0, TimeSpan.Zero);
        TimeProvider timeProvider = new FakeTimeProvider(fixedTime);

        var mockAppUser = new Mock<IAppUser>();
        var mockAuditService = new Mock<IProfitSharingAuditService>();

        var service = new ForfeitureAdjustmentService(
            mockDbContextFactory,
            _totalService,
            demographicReaderMock.Object,
            timeProvider,
            mockAppUser.Object,
            mockAuditService.Object
        );

        var request = new ForfeitureAdjustmentUpdateRequest
        {
            BadgeNumber = _demographic1.BadgeNumber,
            ForfeitureAmount = -1000m, // Un-forfeit (negative = un-forfeit)
            ClassAction = false,
            OffsettingProfitDetailId = forfeitureRecord.Id // Try to reverse this already-reversed record
        };

        // Act
        var result = await service.UpdateForfeitureAdjustmentAsync(request, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(132); // ProfitDetailAlreadyReversed error code
        result.Error.Description.ShouldContain("already been reversed");
    }

    [Fact]
    [Description("PS-2543 : Should allow un-forfeit when profit detail has not been reversed")]
    public async Task UpdateForfeitureAdjustmentAsync_WhenProfitDetailNotReversed_Succeeds()
    {
        // Arrange
        // Create a forfeiture record that has not been reversed
        var forfeitureRecord = new ProfitDetailFaker([_demographic1]).UseSeed(30).Generate();
        forfeitureRecord.Id = 300;
        forfeitureRecord.Ssn = _demographic1.Ssn;
        forfeitureRecord.ProfitYear = 2025;
        forfeitureRecord.ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id;
        forfeitureRecord.Forfeiture = 1000m;
        forfeitureRecord.CommentTypeId = CommentType.Constants.Forfeit.Id;
        forfeitureRecord.ReversedFromProfitDetailId = null;

        ScenarioDataContextFactory mockDbContextFactory = (ScenarioDataContextFactory)new ScenarioFactory
        {
            Demographics = [_demographic1, _demographic2],
            PayProfits = [_payProfit1, _payProfit2],
            ProfitDetails = [_profitDetail1, _profitDetail2, forfeitureRecord]
        }.BuildMocks();

        var frozenServiceMock = new Mock<IFrozenService>(MockBehavior.Strict);
        frozenServiceMock.Setup(f => f.GetActiveFrozenDemographicAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FrozenStateResponse
            {
                Id = 1,
                ProfitYear = 2025,
                IsActive = true,
                AsOfDateTime = DateTimeOffset.UtcNow,
                CreatedDateTime = DateTimeOffset.UtcNow
            });

        var demographicReaderMock = new Mock<IDemographicReaderService>(MockBehavior.Strict);
        demographicReaderMock.Setup(d => d.BuildDemographicQueryAsync(It.IsAny<IProfitSharingDbContext>(), It.IsAny<bool>()))
            .ReturnsAsync(mockDbContextFactory.ProfitSharingDbContext.Object.Demographics.AsQueryable());

        var mockAppUser = new Mock<IAppUser>();
        var mockAuditService = new Mock<IProfitSharingAuditService>();

        var service = new ForfeitureAdjustmentService(
            mockDbContextFactory,
            _totalService,
            demographicReaderMock.Object,
            TimeProvider.System,
            mockAppUser.Object,
            mockAuditService.Object
        );

        var request = new ForfeitureAdjustmentUpdateRequest
        {
            BadgeNumber = _demographic1.BadgeNumber,
            ForfeitureAmount = -1000m, // Un-forfeit (negative = un-forfeit)
            ClassAction = false,
            OffsettingProfitDetailId = forfeitureRecord.Id // This record has not been reversed
        };

        // Act
        var result = await service.UpdateForfeitureAdjustmentAsync(request, CancellationToken.None);

        // Assert
        // The call should not fail due to "already reversed" error
        // It may fail for other reasons (vesting balance, etc.), but not for double-reversal
        if (result.IsError)
        {
            result.Error!.Code.ShouldNotBe(132, "Should not fail with 'already reversed' error");
        }
    }

    #endregion

    #region UpdateForfeitureAdjustmentBulkAsync Tests

    [Fact]
    [Description("PS-XXXX : Empty list returns success")]
    public async Task UpdateForfeitureAdjustmentBulkAsync_EmptyList_ReturnsSuccess()
    {
        // Arrange
        MockDbContextFactory = new ScenarioFactory
        {
            Demographics = [_demographic1]
        }.BuildMocks();

        var demographicReaderMock = new Mock<IDemographicReaderService>(MockBehavior.Strict);

        var mockAppUser = new Mock<IAppUser>();
        var mockAuditService = new Mock<IProfitSharingAuditService>();

        var service = new ForfeitureAdjustmentService(
            MockDbContextFactory,
            _totalService,
            demographicReaderMock.Object,
            TimeProvider.System,
            mockAppUser.Object,
            mockAuditService.Object
        );

        var requests = new List<ForfeitureAdjustmentUpdateRequest>();

        // Act
        var result = await service.UpdateForfeitureAdjustmentBulkAsync(requests, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    #endregion
}
