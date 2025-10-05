using System.ComponentModel;
using System.Security.Cryptography;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Validation;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Services;

[Description("PS-XXXX: Unit tests for ChecksumValidationService")]
public class ChecksumValidationServiceTests
{
    private readonly ScenarioDataContextFactory _scenarioFactory;
    private readonly Mock<ILogger<ChecksumValidationService>> _mockLogger;
    private readonly ChecksumValidationService _service;

    public ChecksumValidationServiceTests()
    {
        _scenarioFactory = new ScenarioDataContextFactory();
        _mockLogger = new Mock<ILogger<ChecksumValidationService>>();
        _service = new ChecksumValidationService(_scenarioFactory, _mockLogger.Object);
    }

    [Fact]
    [Description("PS-XXXX: Valid checksum - returns IsValid true when archived and recalculated checksums match")]
    public async Task ValidateReportChecksumAsync_ValidChecksum_ReturnsIsValidTrue()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "YearEndBreakdown";

        // Create test data
        var reportData = new
        {
            TotalAmount = 12345.67m,
            ParticipantCount = 100m
        };

        var reportJson = JsonSerializer.Serialize(reportData);

        // Build KeyFieldsChecksumJson to match what the service will extract
        // The service will extract all decimal properties and hash them
        var keyFieldsChecksum = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>();

        // Manually build what the service would extract (TotalAmount and ParticipantCount)
        var totalAmountHash = SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(12345.67m));
        keyFieldsChecksum.Add(new KeyValuePair<string, KeyValuePair<decimal, byte[]>>(
            "TotalAmount",
            new KeyValuePair<decimal, byte[]>(12345.67m, totalAmountHash)
        ));

        var participantCountHash = SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(100m));
        keyFieldsChecksum.Add(new KeyValuePair<string, KeyValuePair<decimal, byte[]>>(
            "ParticipantCount",
            new KeyValuePair<decimal, byte[]>(100m, participantCountHash)
        ));

        var archivedReport = new ReportChecksum
        {
            Id = 1,
            ProfitYear = profitYear,
            ReportType = reportType,
            RequestJson = "{}",
            ReportJson = reportJson,
            KeyFieldsChecksumJson = keyFieldsChecksum,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            UserName = "test-user"
        };

        var reportChecksums = new List<ReportChecksum> { archivedReport };
        var mockDbSet = reportChecksums.BuildMockDbSet();

        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _service.ValidateReportChecksumAsync(profitYear, reportType, CancellationToken.None);

        // Assert
        if (!result.IsSuccess)
        {
            // Debug output to see what went wrong
            var errorMsg = result.Error?.Description ?? "null";
            var errorCode = result.Error?.Code.ToString() ?? "null";
            throw new Exception($"Test failed: IsSuccess={result.IsSuccess}, Error={errorMsg}, ErrorCode={errorCode}");
        }

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ProfitYear.ShouldBe(profitYear);
        result.Value.ReportType.ShouldBe(reportType);
        result.Value.IsValid.ShouldBeTrue();
        result.Value.Message.ShouldContain("match");
        result.Value.ArchivedChecksum.ShouldNotBeNullOrEmpty();
        result.Value.CurrentChecksum.ShouldNotBeNullOrEmpty();
        result.Value.ArchivedChecksum.ShouldBe(result.Value.CurrentChecksum);
    }

    [Fact]
    [Description("PS-XXXX: Data drift detected - returns IsValid false when checksums don't match")]
    public async Task ValidateReportChecksumAsync_DataDrift_ReturnsIsValidFalse()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "YearEndBreakdown";

        // Archived checksum with different values
        var archivedKeyFieldsChecksum = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>
        {
            new("TotalAmount", new KeyValuePair<decimal, byte[]>(12345.67m, new byte[] { 1, 2, 3 }))
        };

        // Current data has different values (data drift!)
        var reportData = new
        {
            TotalAmount = 99999.99m, // Changed!
        };

        var archivedReport = new ReportChecksum
        {
            Id = 1,
            ProfitYear = profitYear,
            ReportType = reportType,
            RequestJson = "{}",
            ReportJson = JsonSerializer.Serialize(reportData),
            KeyFieldsChecksumJson = archivedKeyFieldsChecksum,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            UserName = "test-user"
        };

        var reportChecksums = new List<ReportChecksum> { archivedReport };
        var mockDbSet = reportChecksums.BuildMockDbSet();

        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _service.ValidateReportChecksumAsync(profitYear, reportType, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeFalse();
        result.Value.Message.ShouldContain("do not match");
        result.Value.ArchivedChecksum.ShouldNotBe(result.Value.CurrentChecksum);
    }

    [Fact]
    [Description("PS-XXXX: No archived report - returns EntityNotFound error")]
    public async Task ValidateReportChecksumAsync_NoArchivedReport_ReturnsNotFoundError()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "NonExistentReport";

        var reportChecksums = new List<ReportChecksum>(); // Empty list
        var mockDbSet = reportChecksums.BuildMockDbSet();

        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _service.ValidateReportChecksumAsync(profitYear, reportType, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.Description.ShouldContain("not found");
    }

    [Fact]
    [Description("PS-XXXX: Multiple archived reports - uses most recent by CreatedAtUtc")]
    public async Task ValidateReportChecksumAsync_MultipleReports_UsesMostRecent()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "YearEndBreakdown";

        var keyFieldsChecksum = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>
        {
            new("TotalAmount", new KeyValuePair<decimal, byte[]>(12345.67m, new byte[] { 1, 2, 3 }))
        };

        var reportData = new { TotalAmount = 12345.67m };

        var olderReport = new ReportChecksum
        {
            Id = 1,
            ProfitYear = profitYear,
            ReportType = reportType,
            RequestJson = "{}",
            ReportJson = JsonSerializer.Serialize(reportData),
            KeyFieldsChecksumJson = keyFieldsChecksum,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-10), // Older
            UserName = "test-user"
        };

        var newerReport = new ReportChecksum
        {
            Id = 2,
            ProfitYear = profitYear,
            ReportType = reportType,
            RequestJson = "{}",
            ReportJson = JsonSerializer.Serialize(reportData),
            KeyFieldsChecksumJson = keyFieldsChecksum,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1), // Newer - should be used
            UserName = "test-user"
        };

        var reportChecksums = new List<ReportChecksum> { olderReport, newerReport };
        var mockDbSet = reportChecksums.BuildMockDbSet();

        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _service.ValidateReportChecksumAsync(profitYear, reportType, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ArchivedAt.ShouldBe(newerReport.CreatedAtUtc);
    }

    [Fact]
    [Description("PS-XXXX: Batch validation - validates all reports and returns results list")]
    public async Task ValidateAllReportsAsync_MultipleReports_ReturnsAllResults()
    {
        // Arrange
        var validChecksum = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>
        {
            new("Amount", new KeyValuePair<decimal, byte[]>(100m, new byte[] { 1, 2, 3 }))
        };

        var report1 = new ReportChecksum
        {
            Id = 1,
            ProfitYear = 2024,
            ReportType = "Report1",
            RequestJson = "{}",
            ReportJson = JsonSerializer.Serialize(new { Amount = 100m }),
            KeyFieldsChecksumJson = validChecksum,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            UserName = "test-user"
        };

        var report2 = new ReportChecksum
        {
            Id = 2,
            ProfitYear = 2024,
            ReportType = "Report2",
            RequestJson = "{}",
            ReportJson = JsonSerializer.Serialize(new { Amount = 100m }),
            KeyFieldsChecksumJson = validChecksum,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            UserName = "test-user"
        };

        var reportChecksums = new List<ReportChecksum> { report1, report2 };
        var mockDbSet = reportChecksums.BuildMockDbSet();

        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _service.ValidateAllReportsAsync(profitYear: null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value.All(r => r.IsValid).ShouldBeTrue();
    }

    [Fact]
    [Description("PS-XXXX: Batch validation with year filter - only validates specified year")]
    public async Task ValidateAllReportsAsync_WithYearFilter_OnlyValidatesSpecifiedYear()
    {
        // Arrange
        var validChecksum = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>
        {
            new("Amount", new KeyValuePair<decimal, byte[]>(100m, new byte[] { 1, 2, 3 }))
        };

        var report2024 = new ReportChecksum
        {
            Id = 1,
            ProfitYear = 2024,
            ReportType = "Report1",
            RequestJson = "{}",
            ReportJson = JsonSerializer.Serialize(new { Amount = 100m }),
            KeyFieldsChecksumJson = validChecksum,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            UserName = "test-user"
        };

        var report2023 = new ReportChecksum
        {
            Id = 2,
            ProfitYear = 2023,
            ReportType = "Report2",
            RequestJson = "{}",
            ReportJson = JsonSerializer.Serialize(new { Amount = 100m }),
            KeyFieldsChecksumJson = validChecksum,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            UserName = "test-user"
        };

        var reportChecksums = new List<ReportChecksum> { report2024, report2023 };
        var mockDbSet = reportChecksums.BuildMockDbSet();

        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _service.ValidateAllReportsAsync(profitYear: 2024, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].ProfitYear.ShouldBe((short)2024);
    }

    [Fact]
    [Description("PS-XXXX: Batch validation continues on individual errors")]
    public async Task ValidateAllReportsAsync_IndividualError_ContinuesProcessing()
    {
        // Arrange
        var validChecksum = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>
        {
            new("Amount", new KeyValuePair<decimal, byte[]>(100m, new byte[] { 1, 2, 3 }))
        };

        var validReport = new ReportChecksum
        {
            Id = 1,
            ProfitYear = 2024,
            ReportType = "ValidReport",
            RequestJson = "{}",
            ReportJson = JsonSerializer.Serialize(new { Amount = 100m }),
            KeyFieldsChecksumJson = validChecksum,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            UserName = "test-user"
        };

        // Invalid JSON that will cause deserialization error
        var invalidReport = new ReportChecksum
        {
            Id = 2,
            ProfitYear = 2024,
            ReportType = "InvalidReport",
            RequestJson = "{}",
            ReportJson = "invalid-json{", // Invalid JSON
            KeyFieldsChecksumJson = validChecksum,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            UserName = "test-user"
        };

        var reportChecksums = new List<ReportChecksum> { validReport, invalidReport };
        var mockDbSet = reportChecksums.BuildMockDbSet();

        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _service.ValidateAllReportsAsync(profitYear: null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBeGreaterThanOrEqualTo(1); // At least the valid report
        result.Value.Any(r => r.ReportType == "ValidReport").ShouldBeTrue();
    }

    [Fact]
    [Description("PS-XXXX: Empty database - returns empty list")]
    public async Task ValidateAllReportsAsync_NoReports_ReturnsEmptyList()
    {
        // Arrange
        var reportChecksums = new List<ReportChecksum>(); // Empty
        var mockDbSet = reportChecksums.BuildMockDbSet();

        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _service.ValidateAllReportsAsync(profitYear: null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
    }

    [Fact]
    [Description("PS-XXXX: Complex nested JSON - extracts decimal properties correctly")]
    public async Task ValidateReportChecksumAsync_ComplexNestedJson_ExtractsDecimalsCorrectly()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "ComplexReport";

        var keyFieldsChecksum = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>
        {
            new("Level1.Amount", new KeyValuePair<decimal, byte[]>(100m, new byte[] { 1, 2, 3 })),
            new("Level1.Nested.Total", new KeyValuePair<decimal, byte[]>(250.50m, new byte[] { 4, 5, 6 }))
        };

        var complexReportData = new
        {
            Level1 = new
            {
                Amount = 100m,
                Name = "Test",
                Nested = new
                {
                    Total = 250.50m,
                    Description = "Nested total"
                }
            }
        };

        var archivedReport = new ReportChecksum
        {
            Id = 1,
            ProfitYear = profitYear,
            ReportType = reportType,
            RequestJson = "{}",
            ReportJson = JsonSerializer.Serialize(complexReportData),
            KeyFieldsChecksumJson = keyFieldsChecksum,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            UserName = "test-user"
        };

        var reportChecksums = new List<ReportChecksum> { archivedReport };
        var mockDbSet = reportChecksums.BuildMockDbSet();

        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _service.ValidateReportChecksumAsync(profitYear, reportType, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeTrue();
    }

    [Fact]
    [Description("PS-XXXX: Null or empty report type - returns validation error")]
    public async Task ValidateReportChecksumAsync_NullReportType_ReturnsValidationError()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = null!;

        // Act
        var result = await _service.ValidateReportChecksumAsync(profitYear, reportType, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
    }

    [Fact]
    [Description("PS-XXXX: Database exception - returns unexpected error")]
    public async Task ValidateReportChecksumAsync_DatabaseException_ReturnsUnexpectedError()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "TestReport";

        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Throws(new Exception("Database connection failed"));

        // Act
        var result = await _service.ValidateReportChecksumAsync(profitYear, reportType, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.Description.ShouldContain("Database connection failed");
    }
}
