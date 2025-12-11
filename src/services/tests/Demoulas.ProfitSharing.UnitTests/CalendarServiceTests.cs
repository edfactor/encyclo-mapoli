using System.Globalization;
using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.Common.Data.Services.Entities.Contexts.EntityMapping.Data;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests;

[Collection("SharedGlobalState")]
public class CalendarServiceTests : ApiTestBase<Program>
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public CalendarServiceTests()
    {
        _dataContextFactory = MockDbContextFactory;
    }

    [Fact(DisplayName = "Check Calendar can be accessed")]
    public async Task CheckCalendarAccess()
    {
        long count = await _dataContextFactory.UseWarehouseContext(c => c.AccountingPeriods.LongCountAsync(CancellationToken.None));

        count.ShouldBe(CaldarRecordSeeder.Records.Count());
    }

    [InlineData("250101")] // 2025-01-01
    [InlineData("250203")] // 2025-02-03
    [InlineData("250509")] // 2025-05-09
    [InlineData("251009")] // 2025-10-09
    [InlineData("251209")] // 2025-12-09
    [Theory(DisplayName = "Find Weekending Date", Skip = "CaldarRecordSeeder from common library doesn't have calendar data for 2025. Test requires update when common library calendar data is extended.")]
    public async Task FindWeekendingDate(string sDate)
    {
        var date = DateOnly.ParseExact(sDate, "yyMMdd", CultureInfo.InvariantCulture);
        var calendarService = ServiceProvider?.GetRequiredService<ICalendarService>()!;

        var weekEndingDate = await calendarService.FindWeekendingDateFromDateAsync(date);

        weekEndingDate.ShouldBeGreaterThanOrEqualTo(date);

        // Verify that the weekEndingDate is a Saturday
        weekEndingDate.DayOfWeek.ShouldBe(DayOfWeek.Saturday);
    }

    [Fact(DisplayName = "Find Weekending Date - Invalid Date")]
    public Task FindWeekendingDate_InvalidDate()
    {
        var invalidDate = DateOnly.MaxValue;
        var calendarService = ServiceProvider?.GetRequiredService<ICalendarService>()!;
        Func<Task> act = async () => await calendarService.FindWeekendingDateFromDateAsync(invalidDate);
        return act.ShouldThrowAsync<Exception>();
    }

    [Fact(DisplayName = "Find Weekending Date - Valid Date")]
    public async Task FindWeekendingDate_ValidDate()
    {
        var validDate = DateOnly.ParseExact("250101", "yyMMdd", CultureInfo.InvariantCulture); // 2025-01-01
        var calendarService = ServiceProvider?.GetRequiredService<ICalendarService>()!;
        var weekEndingDate = await calendarService.FindWeekendingDateFromDateAsync(validDate);
        weekEndingDate.ShouldBeGreaterThanOrEqualTo(validDate);
        weekEndingDate.DayOfWeek.ShouldBe(DayOfWeek.Saturday);
    }

    [Fact(DisplayName = "PS-366 Get start and end dates for a provided fiscal year")]
    public async Task GetStartEndProvidedFiscalYear()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        TestResult<CalendarResponseDto> response =
            await ApiClient
                .GETAsync<CalendarRecordEndpoint, YearRequest, CalendarResponseDto>(new YearRequest { ProfitYear = 2023 });

        response.Result.ShouldNotBeNull();
        response.Result.FiscalBeginDate.ShouldNotBe(DateTimeOffset.MinValue.ToDateOnly());
        response.Result.FiscalEndDate.ShouldNotBe(DateTimeOffset.MinValue.ToDateOnly());
    }
}

public class CalendarServiceCacheTests
{
    [Fact]
    public async Task GetYearStartAndEndAccountingDatesAsync_ReturnsFromCache_IfPresent()
    {
        // Arrange
        var year = (short)2024;
        var expected = new CalendarResponseDto { FiscalBeginDate = DateTimeOffset.Parse("2024-01-01", CultureInfo.InvariantCulture).ToDateOnly(), FiscalEndDate = DateTimeOffset.Parse("2024-12-31", CultureInfo.InvariantCulture).ToDateOnly() };
        var cacheKey = $"CalendarService_YearDates_{year}";
        var distributedCache = new Mock<IDistributedCache>();
        distributedCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(JsonSerializer.SerializeToUtf8Bytes(expected));
        var dataContextFactory = new Mock<IProfitSharingDataContextFactory>();
        var accountingPeriodsService = new Mock<IAccountingPeriodsService>();
        var service = new Demoulas.ProfitSharing.Services.CalendarService(dataContextFactory.Object, accountingPeriodsService.Object, distributedCache.Object);

        // Act
        var result = await service.GetYearStartAndEndAccountingDatesAsync(year);

        // Assert
        Assert.Equal(expected.FiscalBeginDate, result.FiscalBeginDate);
        Assert.Equal(expected.FiscalEndDate, result.FiscalEndDate);
        distributedCache.Verify(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()), Times.Once);
        // Note: Cannot use VerifyNoOtherCalls() when interface has methods with optional parameters
        // as Moq's expression tree compilation fails with CS0854
        dataContextFactory.Verify(f => f.UseWarehouseContext(It.IsAny<Func<DemoulasCommonWarehouseContext, Task<CalendarResponseDto>>>()), Times.Never);
    }

    [Fact]
    public async Task GetYearStartAndEndAccountingDatesAsync_FallsBackToDb_IfCacheMiss()
    {
        // Arrange
        var year = (short)2024;
        var expected = new CalendarResponseDto { FiscalBeginDate = DateTimeOffset.Parse("2024-01-01", CultureInfo.InvariantCulture).ToDateOnly(), FiscalEndDate = DateTimeOffset.Parse("2024-12-31", CultureInfo.InvariantCulture).ToDateOnly() };
        var cacheKey = $"CalendarService_YearDates_{year}";
        var distributedCache = new Mock<IDistributedCache>();
        distributedCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        var dataContextFactory = new Mock<IProfitSharingDataContextFactory>();
        var accountingPeriodsService = new Mock<IAccountingPeriodsService>();
        dataContextFactory.Setup(f => f.UseWarehouseContext(It.IsAny<Func<DemoulasCommonWarehouseContext, Task<CalendarResponseDto>>>()))
            .ReturnsAsync(expected);
        var service = new Demoulas.ProfitSharing.Services.CalendarService(dataContextFactory.Object, accountingPeriodsService.Object, distributedCache.Object);

        // Act
        var result = await service.GetYearStartAndEndAccountingDatesAsync(year);

        // Assert
        Assert.Equal(expected.FiscalBeginDate, result.FiscalBeginDate);
        Assert.Equal(expected.FiscalEndDate, result.FiscalEndDate);
        distributedCache.Verify(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()), Times.Once);
        dataContextFactory.Verify(f => f.UseWarehouseContext(It.IsAny<Func<DemoulasCommonWarehouseContext, Task<CalendarResponseDto>>>()), Times.Once);
    }
}
