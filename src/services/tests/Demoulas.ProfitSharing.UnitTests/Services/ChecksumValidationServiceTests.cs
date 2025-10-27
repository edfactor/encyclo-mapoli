using System.ComponentModel;
using System.Security.Cryptography;
using System.Text.Json;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Services.Validation;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

[Description("Unit tests for ChecksumValidationService - caller-driven field validation")]
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
    [Description("All provided fields match archived checksums - returns IsValid true")]
    public async Task ValidateReportFieldsAsync_AllFieldsMatch_ReturnsIsValidTrue()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "YearEndBreakdown";

        // Caller-provided field values (what they're seeing in their UI/report)
        var fieldsToValidate = new Dictionary<string, decimal>
        {
            ["TotalAmount"] = 12345.67m,
            ["ParticipantCount"] = 100m
        };

        // Create archived checksum with matching hashes
        var keyFieldsChecksum = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>
        {
            new("TotalAmount", new KeyValuePair<decimal, byte[]>(
                12345.67m,
                SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(12345.67m)))),
            new("ParticipantCount", new KeyValuePair<decimal, byte[]>(
                100m,
                SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(100m))))
        };

        var archivedReport = new ReportChecksum
        {
            Id = 1,
            ProfitYear = profitYear,
            ReportType = reportType,
            RequestJson = "{}",
            ReportJson = "{}",
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
        var result = await _service.ValidateReportFieldsAsync(
            profitYear,
            reportType,
            fieldsToValidate,
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeTrue();
        result.Value.ProfitYear.ShouldBe(profitYear);
        result.Value.ReportType.ShouldBe(reportType);
        result.Value.FieldResults.Count.ShouldBe(2);
        result.Value.FieldResults["TotalAmount"].Matches.ShouldBeTrue();
        result.Value.FieldResults["ParticipantCount"].Matches.ShouldBeTrue();
        result.Value.MismatchedFields.ShouldBeEmpty();
        result.Value.Message.ShouldContain("All 2 field(s) match");
    }

    [Fact]
    [Description("One field doesn't match - returns IsValid false with mismatched field enumerated")]
    public async Task ValidateReportFieldsAsync_OneFieldMismatch_ReturnsIsValidFalse()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "YearEndBreakdown";

        // Caller provides values - one doesn't match archived
        var fieldsToValidate = new Dictionary<string, decimal>
        {
            ["TotalAmount"] = 12345.67m,    // Matches
            ["ParticipantCount"] = 150m     // DOESN'T match (archived is 100)
        };

        // Archived checksums
        var keyFieldsChecksum = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>
        {
            new("TotalAmount", new KeyValuePair<decimal, byte[]>(
                12345.67m,
                SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(12345.67m)))),
            new("ParticipantCount", new KeyValuePair<decimal, byte[]>(
                100m,  // Archived value
                SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(100m))))
        };

        var archivedReport = new ReportChecksum
        {
            Id = 1,
            ProfitYear = profitYear,
            ReportType = reportType,
            RequestJson = "{}",
            ReportJson = "{}",
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
        var result = await _service.ValidateReportFieldsAsync(
            profitYear,
            reportType,
            fieldsToValidate,
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeFalse();
        result.Value.FieldResults.Count.ShouldBe(2);
        result.Value.FieldResults["TotalAmount"].Matches.ShouldBeTrue();
        result.Value.FieldResults["ParticipantCount"].Matches.ShouldBeFalse();
        result.Value.MismatchedFields.Count.ShouldBe(1);
        result.Value.MismatchedFields.ShouldContain("ParticipantCount");
        result.Value.Message.ShouldContain("1 of 2 field(s) do not match");
    }

    [Fact]
    [Description("Field not found in archive - returns IsValid false with appropriate message")]
    public async Task ValidateReportFieldsAsync_FieldNotInArchive_ReturnsIsValidFalse()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "YearEndBreakdown";

        // Caller provides field that doesn't exist in archive
        var fieldsToValidate = new Dictionary<string, decimal>
        {
            ["TotalAmount"] = 12345.67m,
            ["NewField"] = 999m  // Not in archive
        };

        // Archived checksums (only has TotalAmount)
        var keyFieldsChecksum = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>
        {
            new("TotalAmount", new KeyValuePair<decimal, byte[]>(
                12345.67m,
                SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(12345.67m))))
        };

        var archivedReport = new ReportChecksum
        {
            Id = 1,
            ProfitYear = profitYear,
            ReportType = reportType,
            RequestJson = "{}",
            ReportJson = "{}",
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
        var result = await _service.ValidateReportFieldsAsync(
            profitYear,
            reportType,
            fieldsToValidate,
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeFalse();
        result.Value.FieldResults["NewField"].Matches.ShouldBeFalse();
        result.Value.FieldResults["NewField"].ArchivedChecksum.ShouldBeNull();
        result.Value.FieldResults["NewField"].Message.ShouldContain("not found in archived report");
        result.Value.MismatchedFields.ShouldContain("NewField");
    }

    [Fact]
    [Description("No archived report exists - returns EntityNotFound error")]
    public async Task ValidateReportFieldsAsync_NoArchivedReport_ReturnsEntityNotFound()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "YearEndBreakdown";

        var fieldsToValidate = new Dictionary<string, decimal>
        {
            ["TotalAmount"] = 12345.67m
        };

        // No archived reports
        var reportChecksums = new List<ReportChecksum>();
        var mockDbSet = reportChecksums.BuildMockDbSet();

        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _service.ValidateReportFieldsAsync(
            profitYear,
            reportType,
            fieldsToValidate,
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error!.Code.ShouldBe(104);  // EntityNotFound uses code 104
        result.Error.Description.ShouldContain("not found");
    }

    [Fact]
    [Description("Empty fields dictionary - returns validation error")]
    public async Task ValidateReportFieldsAsync_EmptyFields_ReturnsValidationError()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "YearEndBreakdown";
        var fieldsToValidate = new Dictionary<string, decimal>();  // Empty

        // Act
        var result = await _service.ValidateReportFieldsAsync(
            profitYear,
            reportType,
            fieldsToValidate,
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error!.Code.ShouldBe(400);  // Validation error code
        result.Error.ValidationErrors.ShouldNotBeNull();
        result.Error.ValidationErrors.ShouldContainKey(nameof(fieldsToValidate));
        var errorMessage = result.Error.ValidationErrors[nameof(fieldsToValidate)][0];
        errorMessage.ShouldContain("At least one field must be provided");
    }

    [Fact]
    [Description("Null report type - returns validation error")]
    public async Task ValidateReportFieldsAsync_NullReportType_ReturnsValidationError()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = null!;
        var fieldsToValidate = new Dictionary<string, decimal>
        {
            ["TotalAmount"] = 12345.67m
        };

        // Act
        var result = await _service.ValidateReportFieldsAsync(
            profitYear,
            reportType,
            fieldsToValidate,
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error!.Code.ShouldBe(400);  // Validation error code
        result.Error.ValidationErrors.ShouldNotBeNull();
        result.Error.ValidationErrors.ShouldContainKey(nameof(reportType));
        var errorMessage = result.Error.ValidationErrors[nameof(reportType)][0];
        errorMessage.ShouldContain("Report type cannot be null or empty");
    }

    [Fact]
    [Description("Multiple fields with mixed results - returns detailed per-field validation")]
    public async Task ValidateReportFieldsAsync_MixedResults_ReturnsDetailedResults()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "YearEndBreakdown";

        var fieldsToValidate = new Dictionary<string, decimal>
        {
            ["TotalAmount"] = 12345.67m,        // Matches
            ["ParticipantCount"] = 150m,        // Doesn't match (archived is 100)
            ["AverageDistribution"] = 123.45m,  // Matches
            ["NewField"] = 999m,                // Not in archive
            ["TotalHours"] = 2000m              // Matches
        };

        // Archived checksums (4 of 5 fields)
        var keyFieldsChecksum = new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>
        {
            new("TotalAmount", new KeyValuePair<decimal, byte[]>(
                12345.67m,
                SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(12345.67m)))),
            new("ParticipantCount", new KeyValuePair<decimal, byte[]>(
                100m,  // Different from provided value
                SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(100m)))),
            new("AverageDistribution", new KeyValuePair<decimal, byte[]>(
                123.45m,
                SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(123.45m)))),
            new("TotalHours", new KeyValuePair<decimal, byte[]>(
                2000m,
                SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(2000m))))
            // NewField intentionally missing
        };

        var archivedReport = new ReportChecksum
        {
            Id = 1,
            ProfitYear = profitYear,
            ReportType = reportType,
            RequestJson = "{}",
            ReportJson = "{}",
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
        var result = await _service.ValidateReportFieldsAsync(
            profitYear,
            reportType,
            fieldsToValidate,
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.IsValid.ShouldBeFalse();
        result.Value.FieldResults.Count.ShouldBe(5);

        // Check individual field results
        result.Value.FieldResults["TotalAmount"].Matches.ShouldBeTrue();
        result.Value.FieldResults["ParticipantCount"].Matches.ShouldBeFalse();
        result.Value.FieldResults["AverageDistribution"].Matches.ShouldBeTrue();
        result.Value.FieldResults["NewField"].Matches.ShouldBeFalse();
        result.Value.FieldResults["NewField"].ArchivedChecksum.ShouldBeNull();
        result.Value.FieldResults["TotalHours"].Matches.ShouldBeTrue();

        // Check mismatched fields list
        result.Value.MismatchedFields.Count.ShouldBe(2);
        result.Value.MismatchedFields.ShouldContain("ParticipantCount");
        result.Value.MismatchedFields.ShouldContain("NewField");
        result.Value.Message.ShouldContain("2 of 5 field(s) do not match");
    }

    [Fact]
    [Description("Database exception - returns unexpected error")]
    public async Task ValidateReportFieldsAsync_DatabaseException_ReturnsUnexpectedError()
    {
        // Arrange
        short profitYear = 2024;
        string reportType = "YearEndBreakdown";
        var fieldsToValidate = new Dictionary<string, decimal>
        {
            ["TotalAmount"] = 12345.67m
        };

        // Setup mock to throw exception
        _scenarioFactory.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ReportChecksums)
            .Throws(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _service.ValidateReportFieldsAsync(
            profitYear,
            reportType,
            fieldsToValidate,
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error!.Code.ShouldBe(900);
        result.Error.Description.ShouldContain("Failed to validate checksums");
    }
}
