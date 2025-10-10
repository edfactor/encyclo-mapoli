using System.ComponentModel;
using System.Net;
using Demoulas.ProfitSharing.Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Contracts;

[Description("PS-COVERAGE : Error record unit tests")]
public sealed class ErrorTests
{
    [Fact]
    [Description("PS-COVERAGE : Validation error creates with errors dictionary")]
    public void Validation_CreatesErrorWithValidationErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Field1", new[] { "Error 1", "Error 2" } },
            { "Field2", new[] { "Error 3" } }
        };

        // Act
        var error = Error.Validation(errors);

        // Assert
        error.Code.ShouldBe(400);
        error.Description.ShouldBe("Validation error");
        error.ValidationErrors.ShouldBe(errors);
        error.ValidationErrors.Count.ShouldBe(2);
    }

    [Fact]
    [Description("PS-COVERAGE : EmployeeNotFound returns correct error")]
    public void EmployeeNotFound_ReturnsCorrectError()
    {
        // Act
        var error = Error.EmployeeNotFound;

        // Assert
        error.Code.ShouldBe(100);
        error.Description.ShouldBe("Employee not found");
        error.ValidationErrors.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-COVERAGE : CalendarYearNotFound returns correct error")]
    public void CalendarYearNotFound_ReturnsCorrectError()
    {
        // Act
        var error = Error.CalendarYearNotFound;

        // Assert
        error.Code.ShouldBe(101);
        error.Description.ShouldBe("Calendar year not found");
    }

    [Fact]
    [Description("PS-COVERAGE : DistributionNotFound returns correct error")]
    public void DistributionNotFound_ReturnsCorrectError()
    {
        // Act
        var error = Error.DistributionNotFound;

        // Assert
        error.Code.ShouldBe(102);
        error.Description.ShouldBe("Distribution not found");
    }

    [Fact]
    [Description("PS-COVERAGE : BadgeNumberNotFound returns correct error")]
    public void BadgeNumberNotFound_ReturnsCorrectError()
    {
        // Act
        var error = Error.BadgeNumberNotFound;

        // Assert
        error.Code.ShouldBe(103);
        error.Description.ShouldBe("Badge number not found");
    }

    [Theory]
    [Description("PS-COVERAGE : EntityNotFound creates dynamic error message")]
    [InlineData("Product", "Product not found")]
    [InlineData("Order", "Order not found")]
    [InlineData("Invoice", "Invoice not found")]
    public void EntityNotFound_WithEntityName_CreatesCorrectDescription(string entityName, string expectedDescription)
    {
        // Act
        var error = Error.EntityNotFound(entityName);

        // Assert
        error.Code.ShouldBe(104);
        error.Description.ShouldBe(expectedDescription);
    }

    [Fact]
    [Description("PS-COVERAGE : NoPayProfitsDataAvailable returns correct error")]
    public void NoPayProfitsDataAvailable_ReturnsCorrectError()
    {
        // Act
        var error = Error.NoPayProfitsDataAvailable;

        // Assert
        error.Code.ShouldBe(105);
        error.Description.ShouldBe("No PayProfits data available in the system");
    }

    [Fact]
    [Description("PS-COVERAGE : MilitaryContributionDuplicate returns correct error")]
    public void MilitaryContributionDuplicate_ReturnsCorrectError()
    {
        // Act
        var error = Error.MilitaryContributionDuplicate;

        // Assert
        error.Code.ShouldBe(111);
        error.Description.ShouldBe("A regular military contribution already exists for this year");
    }

    [Fact]
    [Description("PS-COVERAGE : MilitaryContributionInvalidYear returns correct error")]
    public void MilitaryContributionInvalidYear_ReturnsCorrectError()
    {
        // Act
        var error = Error.MilitaryContributionInvalidYear;

        // Assert
        error.Code.ShouldBe(112);
        error.Description.ShouldBe("Military contribution year is invalid");
    }

    [Fact]
    [Description("PS-COVERAGE : MilitaryContributionInvalidAmount returns correct error")]
    public void MilitaryContributionInvalidAmount_ReturnsCorrectError()
    {
        // Act
        var error = Error.MilitaryContributionInvalidAmount;

        // Assert
        error.Code.ShouldBe(113);
        error.Description.ShouldBe("Military contribution amount must be greater than zero");
    }

    [Fact]
    [Description("PS-COVERAGE : MilitaryContributionEmployeeNotActive returns correct error")]
    public void MilitaryContributionEmployeeNotActive_ReturnsCorrectError()
    {
        // Act
        var error = Error.MilitaryContributionEmployeeNotActive;

        // Assert
        error.Code.ShouldBe(114);
        error.Description.ShouldBe("Employee is not active as of the contribution date");
    }

    [Fact]
    [Description("PS-COVERAGE : MilitaryContributionEmployeeNotEligible returns correct error")]
    public void MilitaryContributionEmployeeNotEligible_ReturnsCorrectError()
    {
        // Act
        var error = Error.MilitaryContributionEmployeeNotEligible;

        // Assert
        error.Code.ShouldBe(115);
        error.Description.ShouldBe("Employee is not eligible for military contributions");
    }

    [Fact]
    [Description("PS-COVERAGE : SourceDemographicNotFound returns correct error")]
    public void SourceDemographicNotFound_ReturnsCorrectError()
    {
        // Act
        var error = Error.SourceDemographicNotFound;

        // Assert
        error.Code.ShouldBe(106);
        error.Description.ShouldBe("Source demographic not found");
    }

    [Fact]
    [Description("PS-COVERAGE : DestinationDemographicNotFound returns correct error")]
    public void DestinationDemographicNotFound_ReturnsCorrectError()
    {
        // Act
        var error = Error.DestinationDemographicNotFound;

        // Assert
        error.Code.ShouldBe(107);
        error.Description.ShouldBe("Destination demographic not found");
    }

    [Fact]
    [Description("PS-COVERAGE : BothDemographicsNotFound returns correct error")]
    public void BothDemographicsNotFound_ReturnsCorrectError()
    {
        // Act
        var error = Error.BothDemographicsNotFound;

        // Assert
        error.Code.ShouldBe(108);
        error.Description.ShouldBe("Both source and destination demographics not found");
    }

    [Fact]
    [Description("PS-COVERAGE : SameDemographicMerge returns correct error")]
    public void SameDemographicMerge_ReturnsCorrectError()
    {
        // Act
        var error = Error.SameDemographicMerge;

        // Assert
        error.Code.ShouldBe(109);
        error.Description.ShouldBe("Cannot merge demographic with itself");
    }

    [Theory]
    [Description("PS-COVERAGE : MergeOperationFailed creates dynamic error message")]
    [InlineData("Database error", "Merge operation failed: Database error")]
    [InlineData("Network timeout", "Merge operation failed: Network timeout")]
    public void MergeOperationFailed_WithMessage_CreatesCorrectDescription(string message, string expectedDescription)
    {
        // Act
        var error = Error.MergeOperationFailed(message);

        // Assert
        error.Code.ShouldBe(110);
        error.Description.ShouldBe(expectedDescription);
    }

    [Theory]
    [Description("PS-COVERAGE : Unexpected creates dynamic error message")]
    [InlineData("Something went wrong")]
    [InlineData("Null reference exception")]
    public void Unexpected_WithMessage_CreatesCorrectError(string message)
    {
        // Act
        var error = Error.Unexpected(message);

        // Assert
        error.Code.ShouldBe(900);
        error.Description.ShouldBe(message);
    }

    [Fact]
    [Description("PS-COVERAGE : Error implicitly converts to ProblemDetails")]
    public void Error_ImplicitlyConvertsToProblemDetails()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Required" } }
        };
        var error = Error.Validation(errors);

        // Act
        ProblemDetails problemDetails = error;

        // Assert
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("Validation Failed");
        problemDetails.Detail.ShouldBe("Validation error");
        problemDetails.Status.ShouldBe((int)HttpStatusCode.BadRequest);
        problemDetails.Extensions["errors"].ShouldBe(errors);
    }

    [Fact]
    [Description("PS-COVERAGE : Error with empty validation errors converts to ProblemDetails")]
    public void Error_WithEmptyValidationErrors_ConvertsToProblemDetails()
    {
        // Arrange
        var error = Error.EmployeeNotFound;

        // Act
        ProblemDetails problemDetails = error;

        // Assert
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("Validation Failed");
        problemDetails.Detail.ShouldBe("Employee not found");
        problemDetails.Status.ShouldBe((int)HttpStatusCode.BadRequest);
        var errorsExtension = problemDetails.Extensions["errors"] as Dictionary<string, string[]>;
        errorsExtension.ShouldNotBeNull();
        errorsExtension.ShouldBeEmpty();
    }
}
