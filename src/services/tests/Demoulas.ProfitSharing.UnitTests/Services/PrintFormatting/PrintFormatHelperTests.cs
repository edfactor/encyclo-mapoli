using System.ComponentModel;
using System.Text;
using Demoulas.ProfitSharing.Services.Services.Reports.PrintFormatting;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services.PrintFormatting;

public sealed class PrintFormatHelperTests
{
    [Fact(DisplayName = "PrintFormatHelper - Header only for Xerox")]
    [Description("PS-2499 : AppendXeroxHeader only writes when Xerox mode enabled")]
    public void AppendXeroxHeader_ShouldOnlyWriteWhenXeroxEnabled()
    {
        var builder = new StringBuilder();

        PrintFormatHelper.AppendXeroxHeader(builder, "HEADER", false);
        builder.ToString().ShouldBeEmpty();

        PrintFormatHelper.AppendXeroxHeader(builder, "HEADER", true);
        builder.ToString().ShouldBe("HEADER");
    }

    [Fact(DisplayName = "PrintFormatHelper - Line only for Xerox")]
    [Description("PS-2499 : AppendXeroxLine only writes when Xerox mode enabled")]
    public void AppendXeroxLine_ShouldOnlyWriteWhenXeroxEnabled()
    {
        var builder = new StringBuilder();

        PrintFormatHelper.AppendXeroxLine(builder, "LINE", false);
        builder.ToString().ShouldBeEmpty();

        PrintFormatHelper.AppendXeroxLine(builder, "LINE", true);
        builder.ToString().ShouldBe($"LINE{Environment.NewLine}");
    }
}
