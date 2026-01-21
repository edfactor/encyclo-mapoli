using System.ComponentModel;
using Demoulas.ProfitSharing.Services.Services.Reports.PrintFormatting;
using Microsoft.Extensions.Configuration;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services.PrintFormatting;

public sealed class MicrFormatterFactoryTests
{
    [Fact(DisplayName = "MicrFormatterFactory - Missing Newtek account number should throw")]
    [Description("PS-1790 : MICR formatter requires configured Newtek account number")]
    public void GetFormatter_WhenNewtekAccountNumberMissing_ShouldThrow()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var sut = new MicrFormatterFactory(configuration);

        // Act
        var ex = Should.Throw<InvalidOperationException>(() => sut.GetFormatter("026004297"));

        // Assert
        ex.Message.ShouldContain("Printing:Micr:Newtek:AccountNumber");
    }

    [Fact(DisplayName = "MicrFormatterFactory - Unsupported routing should throw")]
    [Description("PS-1790 : MICR formatter rejects unsupported routing numbers")]
    public void GetFormatter_WhenRoutingUnsupported_ShouldThrow()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Printing:Micr:Newtek:AccountNumber"] = "123456",
            })
            .Build();

        var sut = new MicrFormatterFactory(configuration);

        // Act/Assert
        _ = Should.Throw<NotSupportedException>(() => sut.GetFormatter("000000000"));
    }

    [Fact(DisplayName = "MicrFormatterFactory - Account number should be digit-sanitized")]
    [Description("PS-1790 : MICR formatter strips non-digits from configured account number")]
    public void GetFormatter_WhenAccountNumberHasNonDigits_ShouldStripToDigits()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Printing:Micr:Newtek:AccountNumber"] = " 12-34 56 ",
            })
            .Build();

        var sut = new MicrFormatterFactory(configuration);
        var formatter = sut.GetFormatter("026004297");

        // Act
        var micr = formatter.FormatMicrLine(10001, 0m);

        // Assert
        micr.ShouldContain(" 1!2!3!4!5!6!<");
        micr.ShouldStartWith("<10001");
    }
}
