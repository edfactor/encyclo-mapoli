using Demoulas.ProfitSharing.Analyzers;
using Microsoft.CodeAnalysis;

namespace Demoulas.ProfitSharing.UnitTests.Analyzers;

/// <summary>
/// Unit tests for DSM010 - CommentTypeNameBusinessLogicAnalyzer.
/// Ensures business logic uses CommentType.Id instead of Name (which is user-editable).
/// </summary>
public class CommentTypeNameBusinessLogicAnalyzerTests
{
    [Fact]
    public Task Dsm010_ShouldFlagComparison_WhenCommentTypeNameEqualsString()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public bool IsTransferOut(CommentType commentType)
        {
            return commentType.Name == ""Transfer Out"";
        }
    }
}";

        var expected = AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.Diagnostic("DSM010")
            .WithSpan(14, 20, 14, 54)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments();

        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm010_ShouldFlagComparison_WhenStringEqualsCommentTypeName()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public bool IsTransferOut(CommentType commentType)
        {
            return ""Transfer Out"" == commentType.Name;
        }
    }
}";

        var expected = AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.Diagnostic("DSM010")
            .WithSpan(14, 20, 14, 54)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments();

        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm010_ShouldFlagComparison_WhenNotEqualsUsed()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public bool IsNotForfeit(CommentType commentType)
        {
            return commentType.Name != ""Forfeit"";
        }
    }
}";

        var expected = AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.Diagnostic("DSM010")
            .WithSpan(14, 20, 14, 49)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments();

        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm010_ShouldFlagSwitchExpression_WhenSwitchingOnCommentTypeName()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public string GetCategory(CommentType commentType)
        {
            return commentType.Name switch
            {
                ""Transfer Out"" => ""Outgoing"",
                ""Forfeit"" => ""Loss"",
                _ => ""Other""
            };
        }
    }
}";

        var expected = AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.Diagnostic("DSM010")
            .WithSpan(14, 20, 19, 14)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments();

        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm010_ShouldFlagSwitchStatement_WhenSwitchingOnCommentTypeName()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public void ProcessComment(CommentType commentType)
        {
            switch (commentType.Name)
            {
                case ""Transfer Out"":
                    // Handle transfer
                    break;
                case ""Forfeit"":
                    // Handle forfeit
                    break;
            }
        }
    }
}";

        var expected = AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.Diagnostic("DSM010")
            .WithSpan(14, 13, 22, 14)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments();

        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm010_ShouldFlagContains_WhenCommentTypeNameContainsString()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public bool HasTransfer(CommentType commentType)
        {
            return commentType.Name.Contains(""Transfer"");
        }
    }
}";

        var expected = AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.Diagnostic("DSM010")
            .WithSpan(14, 20, 14, 57)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments();

        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm010_ShouldFlagStartsWith_WhenCommentTypeNameStartsWithString()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public bool StartsWithTransfer(CommentType commentType)
        {
            return commentType.Name.StartsWith(""Transfer"");
        }
    }
}";

        var expected = AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.Diagnostic("DSM010")
            .WithSpan(14, 20, 14, 59)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments();

        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm010_ShouldFlagEndsWith_WhenCommentTypeNameEndsWithString()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public bool EndsWithOut(CommentType commentType)
        {
            return commentType.Name.EndsWith(""Out"");
        }
    }
}";

        var expected = AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.Diagnostic("DSM010")
            .WithSpan(14, 20, 14, 52)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments();

        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm010_ShouldFlagStringEquals_WhenUsingStaticMethod()
    {
        var source = @"
using System;

namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public bool IsTransferOut(CommentType commentType)
        {
            return String.Equals(commentType.Name, ""Transfer Out"");
        }
    }
}";

        var expected = AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.Diagnostic("DSM010")
            .WithSpan(16, 20, 16, 67)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments();

        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm010_ShouldFlagNestedPropertyAccess_WhenRecordContainsCommentType()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class ProfitDetail
    {
        public CommentType CommentType { get; set; }
    }

    public class TestService
    {
        public bool IsTransferOut(ProfitDetail detail)
        {
            return detail.CommentType.Name == ""Transfer Out"";
        }
    }
}";

        var expected = AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.Diagnostic("DSM010")
            .WithSpan(19, 20, 19, 61)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments();

        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task Dsm010_ShouldNotFlag_WhenCommentTypeIdUsed()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public bool IsTransferOut(CommentType commentType)
        {
            return commentType.Id == 1; // CORRECT - using ID
        }
    }
}";

        // No diagnostic expected - using Id is correct
        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Dsm010_ShouldNotFlag_WhenNameUsedForDisplay()
    {
        var source = @"
using System;

namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public void LogCommentType(CommentType commentType)
        {
            Console.WriteLine($""Comment type: {commentType.Name}""); // OK - display only
        }
    }
}";

        // No diagnostic expected - string interpolation for display is allowed
        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Dsm010_ShouldNotFlag_WhenNameAssignedToVariable()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class TestService
    {
        public string GetDisplayName(CommentType commentType)
        {
            var displayName = commentType.Name; // OK - simple assignment
            return displayName;
        }
    }
}";

        // No diagnostic expected - simple assignment for display is allowed
        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Dsm010_ShouldNotFlag_WhenComparingOtherProperties()
    {
        var source = @"
namespace Demoulas.ProfitSharing.Services.Test
{
    public class CommentType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class TestService
    {
        public bool HasDescription(CommentType commentType)
        {
            return commentType.Description == ""Some description""; // OK - not Name property
        }
    }
}";

        // No diagnostic expected - comparing other properties is allowed
        return AnalyzerVerifier<CommentTypeNameBusinessLogicAnalyzer>.VerifyAnalyzerAsync(source);
    }
}
