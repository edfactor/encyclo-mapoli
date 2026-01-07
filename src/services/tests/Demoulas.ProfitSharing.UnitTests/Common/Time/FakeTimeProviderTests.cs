using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Time;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Common.Time;

/// <summary>
/// Unit tests for <see cref="FakeTimeProvider"/> and <see cref="FakeTimeConfiguration"/>.
/// </summary>
public sealed class FakeTimeProviderTests
{
    #region FakeTimeConfiguration Tests

    [Fact]
    [Description("PS-XXXX : FakeTimeConfiguration defaults to disabled")]
    public void FakeTimeConfiguration_DefaultsToDisabled()
    {
        // Arrange & Act
        var config = new FakeTimeConfiguration();

        // Assert
        config.Enabled.ShouldBeFalse();
        config.FixedDateTime.ShouldBeNull();
        config.TimeZone.ShouldBeNull();
        config.AdvanceTime.ShouldBeFalse();
    }

    [Fact]
    [Description("PS-XXXX : Validate returns empty when configuration is valid")]
    public void Validate_ValidConfiguration_ReturnsEmpty()
    {
        // Arrange
        var config = new FakeTimeConfiguration
        {
            Enabled = true,
            FixedDateTime = "2025-12-15T10:00:00",
            TimeZone = "Eastern Standard Time",
            AdvanceTime = false
        };

        // Act
        var errors = config.Validate();

        // Assert
        errors.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Validate returns error for invalid date format")]
    public void Validate_InvalidDateFormat_ReturnsError()
    {
        // Arrange
        var config = new FakeTimeConfiguration
        {
            Enabled = true,
            FixedDateTime = "not-a-date"
        };

        // Act
        var errors = config.Validate();

        // Assert
        errors.Count.ShouldBe(1);
        errors[0].ShouldContain("not a valid date/time format");
    }

    [Fact]
    [Description("PS-XXXX : Validate returns error for invalid time zone")]
    public void Validate_InvalidTimeZone_ReturnsError()
    {
        // Arrange
        var config = new FakeTimeConfiguration
        {
            Enabled = true,
            FixedDateTime = "2025-12-15T10:00:00",
            TimeZone = "Invalid/TimeZone"
        };

        // Act
        var errors = config.Validate();

        // Assert
        errors.Count.ShouldBe(1);
        errors[0].ShouldContain("not a valid time zone identifier");
    }

    [Fact]
    [Description("PS-XXXX : GetParsedFixedDateTime returns parsed date for valid input")]
    public void GetParsedFixedDateTime_ValidDate_ReturnsParsedDate()
    {
        // Arrange
        var config = new FakeTimeConfiguration
        {
            FixedDateTime = "2025-12-15T10:30:00"
        };

        // Act
        var result = config.GetParsedFixedDateTime();

        // Assert
        result.ShouldNotBeNull();
        result.Value.Year.ShouldBe(2025);
        result.Value.Month.ShouldBe(12);
        result.Value.Day.ShouldBe(15);
        result.Value.Hour.ShouldBe(10);
        result.Value.Minute.ShouldBe(30);
    }

    [Fact]
    [Description("PS-XXXX : GetParsedFixedDateTime returns null for empty string")]
    public void GetParsedFixedDateTime_EmptyString_ReturnsNull()
    {
        // Arrange
        var config = new FakeTimeConfiguration
        {
            FixedDateTime = ""
        };

        // Act
        var result = config.GetParsedFixedDateTime();

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    [Description("PS-XXXX : GetTimeZone returns local time zone when not configured")]
    public void GetTimeZone_NotConfigured_ReturnsLocal()
    {
        // Arrange
        var config = new FakeTimeConfiguration();

        // Act
        var result = config.GetTimeZone();

        // Assert
        result.ShouldBe(TimeZoneInfo.Local);
    }

    [Fact]
    [Description("PS-XXXX : GetTimeZone returns configured time zone")]
    public void GetTimeZone_ValidTimeZone_ReturnsConfigured()
    {
        // Arrange
        var config = new FakeTimeConfiguration
        {
            TimeZone = "Eastern Standard Time"
        };

        // Act
        var result = config.GetTimeZone();

        // Assert
        result.Id.ShouldBe("Eastern Standard Time");
    }

    #endregion

    #region FakeTimeProvider Tests

    [Fact]
    [Description("PS-XXXX : FakeTimeProvider returns frozen time when AdvanceTime is false")]
    public void FakeTimeProvider_FrozenTime_ReturnsSameTime()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 12, 15, 10, 0, 0, TimeSpan.FromHours(-5));
        var provider = new FakeTimeProvider(fixedTime, advanceTime: false);

        // Act - multiple calls should return the same frozen time
        var time1 = provider.GetUtcNow();
        var time2 = provider.GetUtcNow();
        var time3 = provider.GetUtcNow();

        // Assert
        time1.ShouldBe(time2);
        time2.ShouldBe(time3);
        time1.ShouldBe(fixedTime);
    }

    [Fact]
    [Description("PS-XXXX : FakeTimeProvider with advancing time has correct starting point")]
    public void FakeTimeProvider_AdvancingTime_StartsAtConfiguredTime()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 12, 15, 10, 0, 0, TimeSpan.FromHours(-5));
        var provider = new FakeTimeProvider(fixedTime, advanceTime: true);

        // Act
        var time1 = provider.GetUtcNow();

        // Assert - should be very close to the configured start time
        time1.ShouldBeGreaterThanOrEqualTo(fixedTime);
        (time1 - fixedTime).TotalSeconds.ShouldBeLessThan(1); // Within 1 second of start time
    }

    [Fact]
    [Description("PS-XXXX : FakeTimeProvider IsFakeTime returns true")]
    public void FakeTimeProvider_IsFakeTime_ReturnsTrue()
    {
        // Act & Assert
        FakeTimeProvider.IsFakeTime.ShouldBeTrue();
    }

    [Fact]
    [Description("PS-XXXX : FakeTimeProvider initialized from configuration")]
    public void FakeTimeProvider_FromConfiguration_UsesConfiguredTime()
    {
        // Arrange
        var config = new FakeTimeConfiguration
        {
            Enabled = true,
            FixedDateTime = "2025-12-15T10:00:00",
            TimeZone = "Eastern Standard Time",
            AdvanceTime = false
        };

        // Act
        var provider = new FakeTimeProvider(config);
        var utcNow = provider.GetUtcNow();

        // Assert
        // 10:00 AM EST = 15:00 UTC
        // Note: DateTimeOffset properties return the value with offset, use UtcDateTime for UTC values
        utcNow.UtcDateTime.Year.ShouldBe(2025);
        utcNow.UtcDateTime.Month.ShouldBe(12);
        utcNow.UtcDateTime.Day.ShouldBe(15);
        utcNow.UtcDateTime.Hour.ShouldBe(15); // 10:00 EST + 5 hours = 15:00 UTC
    }

    #endregion

    #region TimeProviderExtensions Tests

    [Fact]
    [Description("PS-XXXX : IsFakeTime extension returns true for FakeTimeProvider")]
    public void IsFakeTime_FakeTimeProvider_ReturnsTrue()
    {
        // Arrange
        TimeProvider provider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        // Act & Assert
        provider.IsFakeTime().ShouldBeTrue();
    }

    [Fact]
    [Description("PS-XXXX : IsFakeTime extension returns false for system TimeProvider")]
    public void IsFakeTime_SystemTimeProvider_ReturnsFalse()
    {
        // Arrange
        TimeProvider provider = TimeProvider.System;

        // Act & Assert
        provider.IsFakeTime().ShouldBeFalse();
    }

    [Fact]
    [Description("PS-XXXX : GetLocalDateOnly returns correct DateOnly")]
    public void GetLocalDateOnly_ReturnsCorrectDate()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 12, 15, 10, 30, 0, TimeSpan.Zero);
        TimeProvider provider = new FakeTimeProvider(fixedTime);

        // Act
        var result = provider.GetLocalDateOnly();

        // Assert
        result.Year.ShouldBe(2025);
        result.Month.ShouldBe(12);
        result.Day.ShouldBe(15);
    }

    [Fact]
    [Description("PS-XXXX : GetLocalYear returns correct year")]
    public void GetLocalYear_ReturnsCorrectYear()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 12, 15, 10, 30, 0, TimeSpan.Zero);
        TimeProvider provider = new FakeTimeProvider(fixedTime);

        // Act
        var result = provider.GetLocalYear();

        // Assert
        result.ShouldBe(2025);
    }

    [Fact]
    [Description("PS-XXXX : GetLocalYearAsShort returns correct short year")]
    public void GetLocalYearAsShort_ReturnsCorrectShortYear()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 12, 15, 10, 30, 0, TimeSpan.Zero);
        TimeProvider provider = new FakeTimeProvider(fixedTime);

        // Act
        var result = provider.GetLocalYearAsShort();

        // Assert
        result.ShouldBe((short)2025);
    }

    [Fact]
    [Description("PS-XXXX : GetLocalMonth returns correct month")]
    public void GetLocalMonth_ReturnsCorrectMonth()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 12, 15, 10, 30, 0, TimeSpan.Zero);
        TimeProvider provider = new FakeTimeProvider(fixedTime);

        // Act
        var result = provider.GetLocalMonth();

        // Assert
        result.ShouldBe(12);
    }

    [Fact]
    [Description("PS-XXXX : GetLocalMonthAsByte returns correct byte month")]
    public void GetLocalMonthAsByte_ReturnsCorrectByteMonth()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 12, 15, 10, 30, 0, TimeSpan.Zero);
        TimeProvider provider = new FakeTimeProvider(fixedTime);

        // Act
        var result = provider.GetLocalMonthAsByte();

        // Assert
        result.ShouldBe((byte)12);
    }

    #endregion
}
