using System.ComponentModel;
using Demoulas.ProfitSharing.Services.Services.Reports.PrintFormatting;
using Microsoft.Extensions.Configuration;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services.PrintFormatting;

public sealed class DjdeDirectiveOptionsTests
{
    [Fact(DisplayName = "DjdeDirectiveOptions - Defaults are set")]
    [Description("PS-2499 : Default DJDE directives are defined for print outputs")]
    public void DjdeDirectiveOptions_ShouldHaveDefaults()
    {
        var options = new DjdeDirectiveOptions();

        options.CertificateHeader.ShouldBe("\fDJDE JDE=PROFNEW,JDL=DFLT5,END,;\r");
        options.TerminatedLettersHeader.ShouldBe("DJDE JDE=QPS003,JDL=PAYROL,END,;");
        options.ProfitsOver73Header.ShouldBe("DJDE JDE=QPS073,JDL=PAYROL,END,;");
        options.TerminatedLettersPrinterControls.ShouldNotBeNull();
        options.TerminatedLettersPrinterControls.Length.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "DjdeDirectiveOptions - Config binding overrides values")]
    [Description("PS-2499 : DJDE directives can be overridden via configuration")]
    public void DjdeDirectiveOptions_ShouldBindFromConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{DjdeDirectiveOptions.SectionName}:CertificateHeader"] = "CERT-HEADER",
                [$"{DjdeDirectiveOptions.SectionName}:TerminatedLettersHeader"] = "TERM-HEADER",
                [$"{DjdeDirectiveOptions.SectionName}:ProfitsOver73Header"] = "OVER73-HEADER",
                [$"{DjdeDirectiveOptions.SectionName}:TerminatedLettersPrinterControls:0"] = "CTRL-1",
                [$"{DjdeDirectiveOptions.SectionName}:TerminatedLettersPrinterControls:1"] = "CTRL-2",
            })
            .Build();

        var options = new DjdeDirectiveOptions();
        config.GetSection(DjdeDirectiveOptions.SectionName).Bind(options);

        options.CertificateHeader.ShouldBe("CERT-HEADER");
        options.TerminatedLettersHeader.ShouldBe("TERM-HEADER");
        options.ProfitsOver73Header.ShouldBe("OVER73-HEADER");
        options.TerminatedLettersPrinterControls.ShouldContain("CTRL-1");
        options.TerminatedLettersPrinterControls.ShouldContain("CTRL-2");
    }
}
