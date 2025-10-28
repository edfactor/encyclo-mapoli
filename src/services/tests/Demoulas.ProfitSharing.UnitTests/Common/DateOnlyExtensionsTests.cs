using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Extensions;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Common;

[Description("PS-COVERAGE : DateOnlyExtensions unit tests")]
public sealed class DateOnlyExtensionsTests
{
    [Fact]
    [Description("PS-COVERAGE : DateOnly converts to DateTimeOffset with default time")]
    public void ToDateTimeOffset_WithDefaultTime_ReturnsCorrectOffset()
    {
        // Arrange
        var date = new DateOnly(2025, 10, 9);

        // Act
        var result = date.ToDateTimeOffset();

        // Assert
        result.Year.ShouldBe(2025);
        result.Month.ShouldBe(10);
        result.Day.ShouldBe(9);
        result.Hour.ShouldBe(0);
        result.Minute.ShouldBe(0);
        result.Second.ShouldBe(0);
    }

    [Fact]
    [Description("PS-COVERAGE : DateOnly converts to DateTimeOffset with specific time")]
    public void ToDateTimeOffset_WithSpecificTime_ReturnsCorrectDateTime()
    {
        // Arrange
        var date = new DateOnly(2025, 12, 25);
        var time = new TimeOnly(14, 30, 45);

        // Act
        var result = date.ToDateTimeOffset(time);

        // Assert
        result.Year.ShouldBe(2025);
        result.Month.ShouldBe(12);
        result.Day.ShouldBe(25);
        result.Hour.ShouldBe(14);
        result.Minute.ShouldBe(30);
        result.Second.ShouldBe(45);
    }

    [Fact]
    [Description("PS-COVERAGE : DateOnly converts with specific timezone")]
    public void ToDateTimeOffset_WithUtcTimeZone_ReturnsUtcOffset()
    {
        // Arrange
        var date = new DateOnly(2025, 6, 15);
        var time = new TimeOnly(12, 0, 0);
        var utcZone = TimeZoneInfo.Utc;

        // Act
        var result = date.ToDateTimeOffset(time, utcZone);

        // Assert
        result.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Fact]
    [Description("PS-COVERAGE : DateOnly converts with Eastern timezone")]
    public void ToDateTimeOffset_WithEasternTimeZone_ReturnsCorrectOffset()
    {
        // Arrange
        var date = new DateOnly(2025, 1, 15); // Winter (EST)
        var time = new TimeOnly(10, 0, 0);
        var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        // Act
        var result = date.ToDateTimeOffset(time, easternZone);

        // Assert
        result.Offset.ShouldBe(TimeSpan.FromHours(-5)); // EST is UTC-5
    }

    [Fact]
    [Description("PS-COVERAGE : DateOnly leap year date converts correctly")]
    public void ToDateTimeOffset_WithLeapYearDate_ReturnsCorrectDate()
    {
        // Arrange
        var date = new DateOnly(2024, 2, 29); // Leap year

        // Act
        var result = date.ToDateTimeOffset();

        // Assert
        result.Year.ShouldBe(2024);
        result.Month.ShouldBe(2);
        result.Day.ShouldBe(29);
    }

    [Fact]
    [Description("PS-COVERAGE : DateOnly min value converts correctly")]
    public void ToDateTimeOffset_WithMinValue_ReturnsMinDate()
    {
        // Arrange
        var date = DateOnly.MinValue;

        // Act
        var result = date.ToDateTimeOffset();

        // Assert
        result.Year.ShouldBe(1);
        result.Month.ShouldBe(1);
        result.Day.ShouldBe(1);
    }

    [Fact]
    [Description("PS-COVERAGE : DateOnly max value converts correctly")]
    public void ToDateTimeOffset_WithMaxValue_ReturnsMaxDate()
    {
        // Arrange
        var date = DateOnly.MaxValue;

        // Act
        var result = date.ToDateTimeOffset();

        // Assert
        result.Year.ShouldBe(9999);
        result.Month.ShouldBe(12);
        result.Day.ShouldBe(31);
    }
}
