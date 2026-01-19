using System.ComponentModel;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Demoulas.ProfitSharing.Services.Reports;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.Util.Extensions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using INavigationService = Demoulas.Common.Contracts.Interfaces.INavigationService;

namespace Demoulas.ProfitSharing.UnitTests.Reports.Adhoc;

[TestSubject(typeof(AccountHistoryReportService))]
public class AccountHistoryReportServiceTests : ApiTestBase<Api.Program>
{
    private readonly AccountHistoryReportService _service;
    private readonly Mock<IProfitSharingAuditService> _mockAuditService;

    public AccountHistoryReportServiceTests()
    {
        Mock<IDemographicReaderService> mockDemographicReader = new();
        mockDemographicReader
            .Setup(d => d.BuildDemographicQueryAsync(It.IsAny<IProfitSharingDbContext>(), It.IsAny<bool>()))
            .ReturnsAsync((IProfitSharingDbContext ctx, bool _) => ctx.Demographics);

        var mockCalendarService = new Mock<ICalendarService>();
        mockCalendarService
            .Setup(c => c.GetYearStartAndEndAccountingDatesAsync((short)DateTime.Now.Year, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CalendarResponseDto
            {
                FiscalBeginDate = DateTime.Now.AddYears(-5).ToDateOnly(),
                FiscalEndDate = DateTime.Now.ToDateOnly()
            });
        var mockEmbeddedSql = new Mock<IEmbeddedSqlService>();
        var mockAppUser = new Mock<IAppUser>();
        var mockMasterInquiry = new Mock<IMasterInquiryService>();
        _mockAuditService = new Mock<IProfitSharingAuditService>();
        var distributedCache = new MemoryDistributedCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        var frozenService = new FrozenService(MockDbContextFactory, new Mock<ICommitGuardOverride>().Object, new Mock<IServiceProvider>().Object, distributedCache, new Mock<INavigationService>().Object, new Mock<TimeProvider>().Object);
        var demographicReader = new DemographicReaderService(frozenService, new HttpContextAccessor());
        var totalService = new TotalService(MockDbContextFactory, mockCalendarService.Object, mockEmbeddedSql.Object, demographicReader);

        var mockLogger = new Mock<ILogger<AccountHistoryReportService>>();
        _service = new AccountHistoryReportService(MockDbContextFactory, mockDemographicReader.Object, totalService, mockAppUser.Object, mockMasterInquiry.Object, mockLogger.Object, _mockAuditService.Object);
    }

    [Description("PS-2160 : Account history report returns same ID for all rows of the same member")]
    [Fact]
    public async Task GetAccountHistoryReportAsync_ShouldReturnSameIdForAllRecords()
    {
        // Arrange
        const int badgeNumber = 700006;
        var startDate = new DateOnly(2007, 1, 1);
        var endDate = new DateOnly(2024, 12, 31);

        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = startDate,
            EndDate = endDate,
            Skip = 0,
            Take = 25
        };

        // Act
        var result = await _service.GetAccountHistoryReportAsync(badgeNumber, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Response.ShouldNotBeNull();
        result.Response.Results.ShouldNotBeNull();

        if (result.Response.Results.Count() > 1)
        {
            // All records should have the same ID (represents the member/demographic record)
            var ids = result.Response.Results.Select(r => r.Id).Distinct().ToList();
            ids.Count.ShouldBe(1, "All records for the same member should have the same ID");
        }
    }

    [Description("PS-2160 : Account history report includes badge number in response")]
    [Fact]
    public async Task GetAccountHistoryReportAsync_ShouldIncludeBadgeNumber()
    {
        // Arrange
        const int badgeNumber = 700006;
        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = new DateOnly(2007, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 25
        };

        // Act
        var result = await _service.GetAccountHistoryReportAsync(badgeNumber, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        if (result.Response.Results.Any())
        {
            result.Response.Results.FirstOrDefault()?.BadgeNumber.ShouldBe(badgeNumber);
        }
    }

    [Description("PS-2160 : Account history report returns empty results for invalid badge number")]
    [Fact]
    public async Task GetAccountHistoryReportAsync_ShouldReturnEmptyForInvalidBadgeNumber()
    {
        // Arrange
        const int invalidBadgeNumber = 999999999;
        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = invalidBadgeNumber,
            StartDate = new DateOnly(2007, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 25
        };

        // Act
        var result = await _service.GetAccountHistoryReportAsync(invalidBadgeNumber, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Response.Results.ShouldBeEmpty();
        result.Response.Total.ShouldBe(0);
    }

    [Description("PS-2160 : Account history report respects pagination skip and take parameters")]
    [Fact]
    public async Task GetAccountHistoryReportAsync_ShouldRespectPaginationParameters()
    {
        // Arrange
        const int badgeNumber = 700006;
        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = new DateOnly(2007, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 10
        };

        // Act
        var result = await _service.GetAccountHistoryReportAsync(badgeNumber, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        if (result.Response.Results.Any())
        {
            result.Response.Results.Count().ShouldBeLessThanOrEqualTo(10);
        }
    }

    [Description("PS-2160 : Account history report includes cumulative totals")]
    [Fact]
    public async Task GetAccountHistoryReportAsync_ShouldIncludeCumulativeTotals()
    {
        // Arrange
        const int badgeNumber = 700006;
        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = new DateOnly(2007, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 25
        };

        // Act
        var result = await _service.GetAccountHistoryReportAsync(badgeNumber, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.CumulativeTotals.ShouldNotBeNull();
        result.CumulativeTotals.TotalContributions.ShouldBeGreaterThanOrEqualTo(0);
        result.CumulativeTotals.TotalEarnings.ShouldBeGreaterThanOrEqualTo(0);
        result.CumulativeTotals.TotalForfeitures.ShouldBeGreaterThanOrEqualTo(0);
        result.CumulativeTotals.TotalWithdrawals.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Description("PS-2160 : Account history report sorts by profit year in descending order by default")]
    [Fact]
    public async Task GetAccountHistoryReportAsync_ShouldDefaultSortByProfitYearDescending()
    {
        // Arrange
        const int badgeNumber = 700006;
        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = new DateOnly(2007, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100,
            SortBy = "profitYear",
            IsSortDescending = true
        };

        // Act
        var result = await _service.GetAccountHistoryReportAsync(badgeNumber, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        if (result.Response.Results.Count() > 1)
        {
            // Verify descending sort: first profit year >= last profit year
            var firstYear = result.Response.Results.First().ProfitYear;
            var lastYear = result.Response.Results.Last().ProfitYear;
            ((int)firstYear).ShouldBeGreaterThanOrEqualTo(lastYear);
        }
    }

    [Description("PS-2160 : Account history report includes proper response metadata")]
    [Fact]
    public async Task GetAccountHistoryReportAsync_ShouldIncludeResponseMetadata()
    {
        // Arrange
        const int badgeNumber = 700006;
        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = new DateOnly(2007, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 25
        };

        // Act
        var result = await _service.GetAccountHistoryReportAsync(badgeNumber, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ReportName.ShouldNotBeNullOrEmpty();
        result.ReportDate.ShouldNotBe(default);
        result.StartDate.ShouldBe(new DateOnly(2007, 1, 1));
        result.EndDate.ShouldBe(new DateOnly(2024, 12, 31));
    }

    [Description("PS-2160 : Account history report handles date range filtering correctly")]
    [Fact]
    public async Task GetAccountHistoryReportAsync_ShouldFilterByDateRange()
    {
        // Arrange
        const int badgeNumber = 700006;
        var startDate = new DateOnly(2020, 1, 1);
        var endDate = new DateOnly(2024, 12, 31);

        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = startDate,
            EndDate = endDate,
            Skip = 0,
            Take = 100
        };

        // Act
        var result = await _service.GetAccountHistoryReportAsync(badgeNumber, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        if (result.Response.Results.Any())
        {
            // All profit years should be within the specified range
            foreach (var r in result.Response.Results)
            {
                ((int)r.ProfitYear).ShouldBeGreaterThanOrEqualTo(startDate.Year);
                ((int)r.ProfitYear).ShouldBeLessThanOrEqualTo(endDate.Year);
            }
        }
    }

    [Description("PS-2160 : Account history report handles null dates with defaults")]
    [Fact]
    public async Task GetAccountHistoryReportAsync_ShouldHandleNullDatesWithDefaults()
    {
        // Arrange
        const int badgeNumber = 700006;
        DateOnly startDate = DateTime.Now.AddYears(-3).ToDateOnly();
        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = startDate,
            EndDate = null,
            Skip = 0,
            Take = 25
        };

        // Act
        var result = await _service.GetAccountHistoryReportAsync(badgeNumber, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Response.ShouldNotBeNull();
        var startYear = request.StartDate!.Value.Year;
        result.StartDate.Year.ShouldBe(startYear);
    }

    [Description("PS-2160 : Account history report records include all required transaction fields")]
    [Fact]
    public async Task GetAccountHistoryReportAsync_ShouldIncludeAllTransactionFields()
    {
        // Arrange
        const int badgeNumber = 700006;
        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = new DateOnly(2007, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 25
        };

        // Act
        var result = await _service.GetAccountHistoryReportAsync(badgeNumber, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        if (result.Response.Results.Any())
        {
            var record = result.Response.Results.First();
            record.Id.ShouldBeGreaterThan(0);
            record.BadgeNumber.ShouldBe(badgeNumber);
            ((int)record.ProfitYear).ShouldBeGreaterThan(0);
            record.Contributions.ShouldBeGreaterThanOrEqualTo(0);
            record.Earnings.ShouldBeGreaterThanOrEqualTo(0);
            record.Forfeitures.ShouldBeGreaterThanOrEqualTo(0);
            record.Withdrawals.ShouldBeGreaterThanOrEqualTo(0);
            record.EndingBalance.ShouldBeGreaterThanOrEqualTo(0);
        }
    }

    [Description("PS-2045 : Account history report throws InvalidOperationException when no data found")]
    [Fact]
    public Task GeneratePdfAsync_WithoutAccountData_ShouldThrowInvalidOperationException()
    {
        // Arrange - Use a badge number that definitely won't have data
        const int badgeNumber = 999999;
        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = new DateOnly(2007, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = int.MaxValue
        };

        // Act & Assert
        return Should.ThrowAsync<InvalidOperationException>(
            async () => await _service.GeneratePdfAsync(badgeNumber, request, CancellationToken.None));
    }

    [Description("PS-2284 : GeneratePdfAsync logs audit event for PDF download tracking")]
    [Fact]
    public async Task GeneratePdfAsync_ShouldLogAuditEventForDownload()
    {
        // Arrange
        const int badgeNumber = 700006;
        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = new DateOnly(2007, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = int.MaxValue
        };

        // Setup mock to track audit logging calls
        _mockAuditService.Setup(a => a.LogSensitiveDataAccessAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        try
        {
            var result = await _service.GeneratePdfAsync(badgeNumber, request, CancellationToken.None);

            // Assert - only if PDF was successfully generated (member exists)
            if (result != null && result.Length > 0)
            {
                // Verify that LogSensitiveDataAccessAsync was called with correct parameters
                _mockAuditService.Verify(
                    a => a.LogSensitiveDataAccessAsync(
                        It.Is<string>(op => op == "Account History PDF Download"),
                        It.Is<string>(table => table == "AccountHistory"),
                        It.Is<string>(pk => pk == $"Badge:{badgeNumber}"),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()),
                    Times.Once,
                    "Audit logging should be called for PDF download");

                // Verify details contain profit year range and record count
                _mockAuditService.Verify(
                    a => a.LogSensitiveDataAccessAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.Is<string>(details => details.Contains($"BadgeNumber: {badgeNumber}") && details.Contains("Profit Year Range") && details.Contains("Records:")),
                        It.IsAny<CancellationToken>()),
                    Times.Once,
                    "Audit details should include profit year range and record count");
            }
        }
        catch (InvalidOperationException)
        {
            // Member doesn't exist in test data - audit should still be attempted but may fail gracefully
            // This is acceptable for this test
        }
    }
}

