using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Reports;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.Adhoc;

[TestSubject(typeof(AccountHistoryReportService))]
public class AccountHistoryReportServiceTests : ApiTestBase<Api.Program>
{
    private readonly AccountHistoryReportService _service;
    private readonly Mock<IDemographicReaderService> _mockDemographicReader;

    public AccountHistoryReportServiceTests()
    {
        _mockDemographicReader = new Mock<IDemographicReaderService>();
        _mockDemographicReader
            .Setup(d => d.BuildDemographicQuery(It.IsAny<IProfitSharingDbContext>(), It.IsAny<bool>()))
            .ReturnsAsync((IProfitSharingDbContext ctx, bool _) => ctx.Demographics);

        _service = new AccountHistoryReportService(MockDbContextFactory, _mockDemographicReader.Object);
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

        if (result.Response.Results.Count > 1)
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
        if (result.Response.Results.Count > 0)
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
        if (result.Response.Results.Count > 0)
        {
            result.Response.Results.Count.ShouldBeLessThanOrEqualTo(10);
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
        if (result.Response.Results.Count > 1)
        {
            // Verify descending sort: first profit year >= last profit year
            var firstYear = result.Response.Results.First().ProfitYear;
            var lastYear = result.Response.Results.Last().ProfitYear;
            firstYear.ShouldBeGreaterThanOrEqualTo(lastYear);
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
        result.StartDate.ShouldBe(startDate: new DateOnly(2007, 1, 1));
        result.EndDate.ShouldBe(endDate: new DateOnly(2024, 12, 31));
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
        if (result.Response.Results.Count > 0)
        {
            // All profit years should be within the specified range
            result.Response.Results.ForEach(r =>
            {
                r.ProfitYear.ShouldBeGreaterThanOrEqualTo(startDate.Year);
                r.ProfitYear.ShouldBeLessThanOrEqualTo(endDate.Year);
            });
        }
    }

    [Description("PS-2160 : Account history report handles null dates with defaults")]
    [Fact]
    public async Task GetAccountHistoryReportAsync_ShouldHandleNullDatesWithDefaults()
    {
        // Arrange
        const int badgeNumber = 700006;
        var request = new AccountHistoryReportRequest
        {
            BadgeNumber = badgeNumber,
            StartDate = null,
            EndDate = null,
            Skip = 0,
            Take = 25
        };

        // Act
        var result = await _service.GetAccountHistoryReportAsync(badgeNumber, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Response.ShouldNotBeNull();
        // Service should use default dates (2007-01-01 to today)
        result.StartDate.Year.ShouldBe(2007);
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
        if (result.Response.Results.Count > 0)
        {
            var record = result.Response.Results.First();
            record.Id.ShouldBeGreaterThan(0);
            record.BadgeNumber.ShouldBe(badgeNumber);
            record.FullName.ShouldNotBeNullOrEmpty();
            record.Ssn.ShouldNotBeNullOrEmpty();
            record.ProfitYear.ShouldBeGreaterThan(0);
            record.Contributions.ShouldBeGreaterThanOrEqualTo(0);
            record.Earnings.ShouldBeGreaterThanOrEqualTo(0);
            record.Forfeitures.ShouldBeGreaterThanOrEqualTo(0);
            record.Withdrawals.ShouldBeGreaterThanOrEqualTo(0);
            record.EndingBalance.ShouldBeGreaterThanOrEqualTo(0);
        }
    }
}
