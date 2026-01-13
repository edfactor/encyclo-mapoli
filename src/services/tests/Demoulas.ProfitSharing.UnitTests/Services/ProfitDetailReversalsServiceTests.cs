using System.ComponentModel;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Common;
using Demoulas.ProfitSharing.UnitTests.Common.Fakes;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

[Collection("SharedGlobalState")]
public class ProfitDetailReversalsServiceTests : ApiTestBase<Api.Program>
{
    private readonly Demographic _demographic1;
    private readonly Demographic _demographic2;
    private readonly ProfitDetail _profitDetail1;
    private readonly ProfitDetail _profitDetail2;
    private readonly Mock<ILogger<ProfitDetailReversalsService>> _loggerMock;
    private readonly Mock<IDemographicReaderService> _demographicReaderServiceMock;

    public ProfitDetailReversalsServiceTests()
    {
        // Setup demographic data
        _demographic1 = new DemographicFaker().UseSeed(1).Generate();
        _demographic1.BadgeNumber = 1001;
        _demographic1.Ssn = 123456789;

        _demographic2 = new DemographicFaker().UseSeed(2).Generate();
        _demographic2.BadgeNumber = 1002;
        _demographic2.Ssn = 987654321;

        var currentMonth = (byte)DateTime.Now.Month;
        var currentYear = (short)DateTime.Now.Year;

        // Setup profit detail data - using reversible profit code and current month/year
        _profitDetail1 = new ProfitDetailFaker([_demographic1, _demographic2]).UseSeed(1).Generate();
        _profitDetail1.Id = 1;
        _profitDetail1.Ssn = _demographic1.Ssn;
        _profitDetail1.ProfitYear = 2026; // Future year (not frozen)
        _profitDetail1.ProfitCodeId = 1; // Reversible code
        _profitDetail1.MonthToDate = currentMonth;
        _profitDetail1.YearToDate = currentYear;
        _profitDetail1.ReversedFromProfitDetailId = null;

        _profitDetail2 = new ProfitDetailFaker([_demographic1, _demographic2]).UseSeed(2).Generate();
        _profitDetail2.Id = 2;
        _profitDetail2.Ssn = _demographic1.Ssn;
        _profitDetail2.ProfitYear = 2026;
        _profitDetail2.ProfitCodeId = 1; // Reversible code
        _profitDetail2.MonthToDate = currentMonth;
        _profitDetail2.YearToDate = currentYear;
        _profitDetail2.ReversedFromProfitDetailId = null;

        _loggerMock = new Mock<ILogger<ProfitDetailReversalsService>>();
        _demographicReaderServiceMock = new Mock<IDemographicReaderService>();
    }

    #region Double-Reversal Protection Tests

    [Fact]
    [Description("PS-2543 : Should reject reversal when profit detail has already been reversed")]
    public async Task ReverseProfitDetailsAsync_WhenProfitDetailAlreadyReversed_ReturnsValidationFailure()
    {
        // Arrange
        // Create a reversal record that points to _profitDetail1, indicating it was already reversed
        var existingReversal = new ProfitDetailFaker([_demographic1]).UseSeed(10).Generate();
        existingReversal.Id = 100;
        existingReversal.Ssn = _demographic1.Ssn;
        existingReversal.ProfitYear = 2026;
        existingReversal.CommentTypeId = CommentType.Constants.Reversal.Id;
        existingReversal.ReversedFromProfitDetailId = _profitDetail1.Id; // This reversal came from _profitDetail1

        var mockDbContextFactory = (ScenarioDataContextFactory)new ScenarioFactory
        {
            Demographics = [_demographic1, _demographic2],
            ProfitDetails = [_profitDetail1, _profitDetail2, existingReversal],
            FrozenStates = [new FrozenState { Id = 1, ProfitYear = 2024, AsOfDateTime = DateTimeOffset.UtcNow, CreatedDateTime = DateTimeOffset.UtcNow }]
        }.BuildMocks();

        var service = new ProfitDetailReversalsService(mockDbContextFactory, _demographicReaderServiceMock.Object, _loggerMock.Object);

        // Act - Try to reverse _profitDetail1 again
        var result = await service.ReverseProfitDetailsAsync([_profitDetail1.Id], CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error!.ValidationErrors.ShouldContainKey("profitDetailIds");
        var errorMessage = result.Error.ValidationErrors["profitDetailIds"][0];
        errorMessage.ShouldContain("already been reversed");
        errorMessage.ShouldContain(_profitDetail1.Id.ToString());
    }

    [Fact]
    [Description("PS-2543 : Should reject batch reversal when any profit detail has already been reversed")]
    public async Task ReverseProfitDetailsAsync_WhenAnyInBatchAlreadyReversed_ReturnsValidationFailure()
    {
        // Arrange
        // Create a reversal record that points to _profitDetail1 only
        var existingReversal = new ProfitDetailFaker([_demographic1]).UseSeed(10).Generate();
        existingReversal.Id = 100;
        existingReversal.Ssn = _demographic1.Ssn;
        existingReversal.ProfitYear = 2026;
        existingReversal.CommentTypeId = CommentType.Constants.Reversal.Id;
        existingReversal.ReversedFromProfitDetailId = _profitDetail1.Id;

        var mockDbContextFactory = (ScenarioDataContextFactory)new ScenarioFactory
        {
            Demographics = [_demographic1, _demographic2],
            ProfitDetails = [_profitDetail1, _profitDetail2, existingReversal],
            FrozenStates = [new FrozenState { Id = 1, ProfitYear = 2024, AsOfDateTime = DateTimeOffset.UtcNow, CreatedDateTime = DateTimeOffset.UtcNow }]
        }.BuildMocks();

        var service = new ProfitDetailReversalsService(mockDbContextFactory, _demographicReaderServiceMock.Object, _loggerMock.Object);

        // Act - Try to reverse both _profitDetail1 (already reversed) and _profitDetail2 (not reversed)
        var result = await service.ReverseProfitDetailsAsync([_profitDetail1.Id, _profitDetail2.Id], CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error!.ValidationErrors.ShouldContainKey("profitDetailIds");
        var errorMessage = result.Error.ValidationErrors["profitDetailIds"][0];
        errorMessage.ShouldContain("already been reversed");
        errorMessage.ShouldContain(_profitDetail1.Id.ToString());
        // _profitDetail2 should not be in the error since it hasn't been reversed
        errorMessage.ShouldNotContain(_profitDetail2.Id.ToString());
    }

    [Fact]
    [Description("PS-2543 : Should allow reversal chain (reversing a reversal is allowed)")]
    public async Task ReverseProfitDetailsAsync_WhenReversingAReversal_ShouldNotBlockAsDoubleReversal()
    {
        // Arrange
        var currentMonth = (byte)DateTime.Now.Month;
        var currentYear = (short)DateTime.Now.Year;

        // Create a reversal record that is itself a reversal of another record
        var reversalRecord = new ProfitDetailFaker([_demographic1]).UseSeed(10).Generate();
        reversalRecord.Id = 100;
        reversalRecord.Ssn = _demographic1.Ssn;
        reversalRecord.ProfitYear = 2026;
        reversalRecord.ProfitCodeId = 1; // Reversible
        reversalRecord.MonthToDate = currentMonth;
        reversalRecord.YearToDate = currentYear;
        reversalRecord.CommentTypeId = CommentType.Constants.Reversal.Id;
        reversalRecord.ReversedFromProfitDetailId = _profitDetail1.Id; // This is a reversal of _profitDetail1

        var mockDbContextFactory = (ScenarioDataContextFactory)new ScenarioFactory
        {
            Demographics = [_demographic1, _demographic2],
            ProfitDetails = [_profitDetail1, reversalRecord], // reversalRecord is available to reverse
            FrozenStates = [new FrozenState { Id = 1, ProfitYear = 2024, AsOfDateTime = DateTimeOffset.UtcNow, CreatedDateTime = DateTimeOffset.UtcNow }]
        }.BuildMocks();

        var service = new ProfitDetailReversalsService(mockDbContextFactory, _demographicReaderServiceMock.Object, _loggerMock.Object);

        // Act - Try to reverse the reversal record (this should be allowed - reversal chains are OK)
        var result = await service.ReverseProfitDetailsAsync([reversalRecord.Id], CancellationToken.None);

        // Assert
        // The reversal record itself has not been reversed by another record, so this should not fail with "already reversed"
        if (!result.IsSuccess && result.Error?.ValidationErrors != null)
        {
            var hasAlreadyReversedError = result.Error.ValidationErrors.Values
                .SelectMany(v => v)
                .Any(msg => msg.Contains("already been reversed", StringComparison.OrdinalIgnoreCase));
            hasAlreadyReversedError.ShouldBeFalse("Reversing a reversal record should not be blocked as double-reversal");
        }
    }

    [Fact]
    [Description("PS-2543 : Should return validation error when null or empty IDs provided")]
    public async Task ReverseProfitDetailsAsync_WithEmptyIds_ReturnsValidationFailure()
    {
        // Arrange
        var mockDbContextFactory = (ScenarioDataContextFactory)new ScenarioFactory
        {
            Demographics = [_demographic1],
            ProfitDetails = [_profitDetail1]
        }.BuildMocks();

        var service = new ProfitDetailReversalsService(mockDbContextFactory, _demographicReaderServiceMock.Object, _loggerMock.Object);

        // Act
        var result = await service.ReverseProfitDetailsAsync([], CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
    }

    [Fact]
    [Description("PS-2543 : Should return validation error when IDs exceed max batch size")]
    public async Task ReverseProfitDetailsAsync_WithTooManyIds_ReturnsValidationFailure()
    {
        // Arrange
        var mockDbContextFactory = (ScenarioDataContextFactory)new ScenarioFactory
        {
            Demographics = [_demographic1],
            ProfitDetails = [_profitDetail1]
        }.BuildMocks();

        var service = new ProfitDetailReversalsService(mockDbContextFactory, _demographicReaderServiceMock.Object, _loggerMock.Object);

        // Create array with more than 1000 IDs
        var tooManyIds = Enumerable.Range(1, 1001).ToArray();

        // Act
        var result = await service.ReverseProfitDetailsAsync(tooManyIds, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error!.ValidationErrors.ShouldContainKey("profitDetailIds");
        result.Error.ValidationErrors["profitDetailIds"][0].ShouldContain("1000");
    }

    #endregion
}
