using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Contracts;

[Description("PS-COVERAGE : Result<T> record unit tests")]
public sealed class ResultTests
{
    private sealed record TestDto(string Value, int Count);

    [Fact]
    [Description("PS-COVERAGE : Success result contains value")]
    public void Success_WithValue_CreatesSuccessResult()
    {
        // Arrange
        var dto = new TestDto("test", 42);

        // Act
        var result = Result<TestDto>.Success(dto);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(dto);
        result.Error.ShouldBeNull();
    }

    [Fact]
    [Description("PS-COVERAGE : Success with null value throws ArgumentNullException")]
    public void Success_WithNullValue_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => Result<TestDto>.Success(null!));
    }

    [Fact]
    [Description("PS-COVERAGE : Failure result contains error")]
    public void Failure_WithError_CreatesFailureResult()
    {
        // Arrange
        var error = Error.EmployeeNotFound;

        // Act
        var result = Result<TestDto>.Failure(error);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsError.ShouldBeTrue();
        result.Error.ShouldBe(error);
        result.Value.ShouldBeNull();
    }

    [Fact]
    [Description("PS-COVERAGE : Failure with null error throws ArgumentNullException")]
    public void Failure_WithNullError_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => Result<TestDto>.Failure(null!));
    }

    [Fact]
    [Description("PS-COVERAGE : ValidationFailure creates result with validation errors")]
    public void ValidationFailure_WithErrors_CreatesValidationFailureResult()
    {
        // Arrange
        var validationErrors = new Dictionary<string, string[]>
        {
            { "Field1", new[] { "Error 1" } },
            { "Field2", new[] { "Error 2", "Error 3" } }
        };

        // Act
        var result = Result<TestDto>.ValidationFailure(validationErrors);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsError.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error!.Code.ShouldBe(400);
        result.Error.Description.ShouldBe("Validation error");
        result.Error.ValidationErrors.ShouldBe(validationErrors);
    }

    [Fact]
    [Description("PS-COVERAGE : Match executes onSuccess for success result")]
    public void Match_WithSuccessResult_ExecutesOnSuccess()
    {
        // Arrange
        var dto = new TestDto("test", 42);
        var result = Result<TestDto>.Success(dto);

        // Act
        var output = result.Match(
            onSuccess: value => $"Success: {value.Value}",
            onError: error => $"Error: {error.Detail}"
        );

        // Assert
        output.ShouldBe("Success: test");
    }

    [Fact]
    [Description("PS-COVERAGE : Match executes onError for failure result")]
    public void Match_WithFailureResult_ExecutesOnError()
    {
        // Arrange
        var error = Error.EmployeeNotFound;
        var result = Result<TestDto>.Failure(error);

        // Act
        var output = result.Match(
            onSuccess: value => $"Success: {value.Value}",
            onError: problemDetails => $"Error: {problemDetails.Detail}"
        );

        // Assert
        output.ShouldBe("Error: Employee not found");
    }

    [Fact]
    [Description("PS-COVERAGE : ToHttpResult with success returns Ok")]
    public void ToHttpResult_WithSuccess_ReturnsOk()
    {
        // Arrange
        var dto = new TestDto("test", 42);
        var result = Result<TestDto>.Success(dto);

        // Act
        var httpResult = result.ToHttpResult();

        // Assert
        httpResult.Result.ShouldBeOfType<Ok<TestDto>>();
        var okResult = (Ok<TestDto>)httpResult.Result;
        okResult.Value.ShouldBe(dto);
    }

    [Fact]
    [Description("PS-COVERAGE : ToHttpResult with matching not-found error returns NotFound")]
    public void ToHttpResult_WithMatchingNotFoundError_ReturnsNotFound()
    {
        // Arrange
        var notFoundError = Error.EmployeeNotFound;
        var result = Result<TestDto>.Failure(notFoundError);

        // Act
        var httpResult = result.ToHttpResult(Error.EmployeeNotFound, Error.DistributionNotFound);

        // Assert
        httpResult.Result.ShouldBeOfType<NotFound>();
    }

    [Fact]
    [Description("PS-COVERAGE : ToHttpResult with non-matching error returns Problem")]
    public void ToHttpResult_WithNonMatchingError_ReturnsProblem()
    {
        // Arrange
        var error = Error.Unexpected("Something went wrong");
        var result = Result<TestDto>.Failure(error);

        // Act
        var httpResult = result.ToHttpResult(Error.EmployeeNotFound);

        // Assert
        httpResult.Result.ShouldBeOfType<ProblemHttpResult>();
    }

    [Fact]
    [Description("PS-COVERAGE : ToHttpResult with validation failure returns Problem")]
    public void ToHttpResult_WithValidationFailure_ReturnsProblem()
    {
        // Arrange
        var validationErrors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Required" } }
        };
        var result = Result<TestDto>.ValidationFailure(validationErrors);

        // Act
        var httpResult = result.ToHttpResult();

        // Assert
        httpResult.Result.ShouldBeOfType<ProblemHttpResult>();
    }

    [Fact]
    [Description("PS-COVERAGE : Implicit conversion with success returns Ok")]
    public void ImplicitConversion_WithSuccess_ReturnsOk()
    {
        // Arrange
        var dto = new TestDto("test", 42);
        var result = Result<TestDto>.Success(dto);

        // Act
        Results<Ok<TestDto>, NotFound, ProblemHttpResult> httpResult = result;

        // Assert
        httpResult.Result.ShouldBeOfType<Ok<TestDto>>();
        var okResult = (Ok<TestDto>)httpResult.Result;
        okResult.Value.ShouldBe(dto);
    }

    [Fact]
    [Description("PS-COVERAGE : Implicit conversion with failure returns Problem")]
    public void ImplicitConversion_WithFailure_ReturnsProblem()
    {
        // Arrange
        var error = Error.EmployeeNotFound;
        var result = Result<TestDto>.Failure(error);

        // Act
        Results<Ok<TestDto>, NotFound, ProblemHttpResult> httpResult = result;

        // Assert
        httpResult.Result.ShouldBeOfType<ProblemHttpResult>();
    }

    [Fact]
    [Description("PS-COVERAGE : Multiple not-found errors match correctly")]
    public void ToHttpResult_WithMultipleNotFoundErrors_MatchesAnyOfThem()
    {
        // Arrange
        var error = Error.DistributionNotFound;
        var result = Result<TestDto>.Failure(error);

        // Act
        var httpResult = result.ToHttpResult(
            Error.EmployeeNotFound,
            Error.DistributionNotFound,
            Error.CalendarYearNotFound
        );

        // Assert
        httpResult.Result.ShouldBeOfType<NotFound>();
    }

    [Fact]
    [Description("PS-COVERAGE : ToHttpResult with no not-found errors specified returns Problem for any error")]
    public void ToHttpResult_WithNoNotFoundErrorsSpecified_ReturnsProblemForError()
    {
        // Arrange
        var error = Error.EmployeeNotFound;
        var result = Result<TestDto>.Failure(error);

        // Act
        var httpResult = result.ToHttpResult();

        // Assert
        httpResult.Result.ShouldBeOfType<ProblemHttpResult>();
    }

    [Fact]
    [Description("PS-COVERAGE : IsError property returns opposite of IsSuccess")]
    public void IsError_ReturnsOppositeOfIsSuccess()
    {
        // Arrange
        var successResult = Result<TestDto>.Success(new TestDto("test", 1));
        var failureResult = Result<TestDto>.Failure(Error.EmployeeNotFound);

        // Assert
        successResult.IsSuccess.ShouldBeTrue();
        successResult.IsError.ShouldBeFalse();

        failureResult.IsSuccess.ShouldBeFalse();
        failureResult.IsError.ShouldBeTrue();
    }
}
