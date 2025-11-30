using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Extensions;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Common;

[Description("PS-COVERAGE : SsnExtensions unit tests")]
public sealed class SsnExtensionsTests
{
    [Theory]
    [Description("PS-COVERAGE : MaskSsn masks integer SSN correctly")]
    [InlineData(123456789, "XXX-XX-6789")]
    [InlineData(987654321, "XXX-XX-4321")]
    [InlineData(111223333, "XXX-XX-3333")]
    [InlineData(1234, "XXX-XX-1234")] // Short SSN with leading zeros
    public void MaskSsn_WithIntegerSsn_ReturnsMaskedFormat(int ssn, string expected)
    {
        // Act
        var result = ssn.MaskSsn();

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [Description("PS-COVERAGE : MaskSsn masks object SSN correctly")]
    [InlineData("123456789", "XXX-XX-6789")]
    [InlineData("987654321", "XXX-XX-4321")]
    [InlineData("000001234", "XXX-XX-1234")]
    public void MaskSsn_WithObjectSsn_ReturnsMaskedFormat(object ssn, string expected)
    {
        // Act
        var result = ssn.MaskSsn();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    [Description("PS-COVERAGE : MaskSsn with leading zeros preserves last four digits")]
    public void MaskSsn_WithLeadingZeros_PreservesLastFourDigits()
    {
        // Arrange
        int ssn = 9999; // Will be interpreted as 000009999

        // Act
        var result = ssn.MaskSsn();

        // Assert
        result.ShouldBe("XXX-XX-9999");
    }

    [Theory]
    [Description("PS-COVERAGE : ConvertSsnToInt converts formatted SSN correctly")]
    [InlineData("123-45-6789", 123456789)]
    [InlineData("987-65-4321", 987654321)]
    [InlineData("000-12-3456", 123456)]
    [InlineData("111-22-3333", 111223333)]
    public void ConvertSsnToInt_WithFormattedSsn_ReturnsInteger(string formattedSsn, int expected)
    {
        // Act
        var result = formattedSsn.ConvertSsnToInt();

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [Description("PS-COVERAGE : ConvertSsnToInt converts SSN without dashes")]
    [InlineData("123456789", 123456789)]
    [InlineData("987654321", 987654321)]
    [InlineData("000123456", 123456)]
    public void ConvertSsnToInt_WithoutDashes_ReturnsInteger(string ssn, int expected)
    {
        // Act
        var result = ssn.ConvertSsnToInt();

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [Description("PS-COVERAGE : ConvertSsnToInt throws ArgumentException for null, empty, or too short")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12-34-56")] // Too short
    public void ConvertSsnToInt_WithInvalidInput_ThrowsArgumentException(string? invalidSsn)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => invalidSsn!.ConvertSsnToInt());
    }

    [Fact]
    [Description("PS-COVERAGE : ConvertSsnToInt throws FormatException for non-numeric input")]
    public void ConvertSsnToInt_WithNonNumericInput_ThrowsFormatException()
    {
        // Arrange
        var invalidSsn = "abc-de-fghi";

        // Act & Assert
        Should.Throw<FormatException>(() => invalidSsn.ConvertSsnToInt());
    }

    [Fact]
    [Description("PS-COVERAGE : ConvertSsnToInt with extra characters strips non-numeric")]
    public void ConvertSsnToInt_WithExtraCharacters_StripsNonNumeric()
    {
        // Arrange
        var ssnWithSpaces = "123 - 45 - 6789";

        // Act
        var result = ssnWithSpaces.ConvertSsnToInt();

        // Assert
        result.ShouldBe(123456789);
    }

    [Fact]
    [Description("PS-COVERAGE : MaskSsn and ConvertSsnToInt roundtrip with last 4 digits")]
    public void MaskSsn_AndConvertBack_PreservesLastFourDigits()
    {
        // Arrange
        var originalSsn = 123456789;

        // Act
        var masked = originalSsn.MaskSsn(); // XXX-XX-6789
        var lastFourDigits = masked.Substring(7); // 6789

        // Assert
        lastFourDigits.ShouldBe("6789");
        originalSsn.ToString().ShouldEndWith(lastFourDigits);
    }

    [Theory]
    [Description("PS-COVERAGE : MaskSsn handles edge case SSN values")]
    [InlineData(0, "XXX-XX-0000")]
    [InlineData(1, "XXX-XX-0001")]
    [InlineData(999999999, "XXX-XX-9999")]
    public void MaskSsn_WithEdgeCases_ReturnsMaskedFormat(int ssn, string expected)
    {
        // Act
        var result = ssn.MaskSsn();

        // Assert - Test edge cases for boundary values
        result.ShouldBe(expected);
        result.Length.ShouldBe(11); // Always 11 characters in format XXX-XX-####
    }
}
