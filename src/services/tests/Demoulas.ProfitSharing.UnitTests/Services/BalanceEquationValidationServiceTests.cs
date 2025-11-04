using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Services.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

[Description("PS-1721 : Unit tests for BalanceEquationValidationService - PAY444 balance equation validation")]
public class BalanceEquationValidationServiceTests
{
    private readonly Mock<ILogger<BalanceEquationValidationService>> _logger;
    private readonly BalanceEquationValidationService _service;

    public BalanceEquationValidationServiceTests()
    {
        // Initialize telemetry before running tests
        EndpointTelemetry.Initialize();

        _logger = new Mock<ILogger<BalanceEquationValidationService>>();
        _service = new BalanceEquationValidationService(_logger.Object);
    }

    [Fact]
    [Description("PS-1721 : Valid balance equation - returns valid group")]
    public async Task ValidateBalanceEquationAsync_ValidEquation_ReturnsValidGroup()
    {
        // Arrange
        short profitYear = 2024;
        var currentValues = new Dictionary<string, decimal>
        {
            { "PAY444.BeginningBalance", 100000m },
            { "PAY444.CONTRIB", 50000m },
            { "PAY444.ALLOC", 10000m },
            { "PAY444.DISTRIB", 20000m },
            { "PAY444.PAIDALLOC", 5000m },
            { "PAY444.EARNINGS", 15000m },
            { "PAY444.FORFEITS", 2000m },
            // Ending = 100000 + 50000 + 10000 - 20000 - 5000 + 15000 - 2000 = 148000
            { "PAY444.EndingBalance", 148000m }
        };

        // Act
        var result = await _service.ValidateBalanceEquationAsync(profitYear, currentValues, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        var group = result.Value;
        group.GroupName.ShouldBe("Balance Equation");
        group.IsValid.ShouldBeTrue();
        group.Priority.ShouldBe("Critical");
        group.ValidationRule.ShouldNotBeNull();
        group.ValidationRule.ShouldContain("Rule 5");

        // Verify all component validations exist
        group.Validations.Count.ShouldBe(8); // 7 components + calculated ending balance
        group.Validations.Any(v => v.FieldName == "BeginningBalance").ShouldBeTrue();
        group.Validations.Any(v => v.FieldName == "Contributions").ShouldBeTrue();
        group.Validations.Any(v => v.FieldName == "ALLOC").ShouldBeTrue();
        group.Validations.Any(v => v.FieldName == "Distributions").ShouldBeTrue();
        group.Validations.Any(v => v.FieldName == "PAID ALLOC").ShouldBeTrue();
        group.Validations.Any(v => v.FieldName == "Earnings").ShouldBeTrue();
        group.Validations.Any(v => v.FieldName == "Forfeitures").ShouldBeTrue();
        group.Validations.Any(v => v.FieldName == "CalculatedEndingBalance").ShouldBeTrue();

        // Verify calculated ending balance validation
        var calculatedValidation = group.Validations!.First(v => v.FieldName == "CalculatedEndingBalance");
        calculatedValidation.IsValid.ShouldBeTrue();
        calculatedValidation.CurrentValue.ShouldBe(148000m);
        calculatedValidation.ExpectedValue.ShouldBe(148000m);
        calculatedValidation.Variance.ShouldBe(0m);
        calculatedValidation.Message.ShouldNotBeNull();
        calculatedValidation.Message.ShouldContain("✅");
    }

    [Fact]
    [Description("PS-1721 : Invalid balance equation with variance - returns invalid group")]
    public async Task ValidateBalanceEquationAsync_InvalidEquation_ReturnsInvalidGroup()
    {
        // Arrange
        short profitYear = 2024;
        var currentValues = new Dictionary<string, decimal>
        {
            { "PAY444.BeginningBalance", 100000m },
            { "PAY444.CONTRIB", 50000m },
            { "PAY444.ALLOC", 10000m },
            { "PAY444.DISTRIB", 20000m },
            { "PAY444.PAIDALLOC", 5000m },
            { "PAY444.EARNINGS", 15000m },
            { "PAY444.FORFEITS", 2000m },
            // Calculated: 148000, but expected is different
            { "PAY444.EndingBalance", 150000m }
        };

        // Act
        var result = await _service.ValidateBalanceEquationAsync(profitYear, currentValues, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        var group = result.Value;
        group.IsValid.ShouldBeFalse();

        // Verify calculated ending balance validation shows mismatch
        var calculatedValidation = group.Validations!.First(v => v.FieldName == "CalculatedEndingBalance");
        calculatedValidation.IsValid.ShouldBeFalse();
        calculatedValidation.CurrentValue.ShouldBe(148000m); // Calculated
        calculatedValidation.ExpectedValue.ShouldBe(150000m); // Expected from data
        calculatedValidation.Variance.ShouldBe(-2000m); // 148000 - 150000
        calculatedValidation.Message.ShouldNotBeNull();
        calculatedValidation.Message.ShouldContain("⚠️");
        calculatedValidation.Message.ShouldContain("MISMATCH");
    }

    [Fact]
    [Description("PS-1721 : Missing field values default to zero")]
    public async Task ValidateBalanceEquationAsync_MissingFields_DefaultsToZero()
    {
        // Arrange
        short profitYear = 2024;
        var currentValues = new Dictionary<string, decimal>
        {
            { "PAY444.BeginningBalance", 100000m },
            { "PAY444.EndingBalance", 100000m }
            // All other fields missing - should default to 0
        };

        // Act
        var result = await _service.ValidateBalanceEquationAsync(profitYear, currentValues, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        var group = result.Value;

        // Calculated: 100000 + 0 + 0 - 0 - 0 + 0 - 0 = 100000
        // Should match expected 100000
        group.IsValid.ShouldBeTrue();

        var calculatedValidation = group.Validations!.First(v => v.FieldName == "CalculatedEndingBalance");
        calculatedValidation.CurrentValue.ShouldBe(100000m);
        calculatedValidation.ExpectedValue.ShouldBe(100000m);
    }

    [Fact]
    [Description("PS-1721 : Component validations have correct report code")]
    public async Task ValidateBalanceEquationAsync_ComponentValidations_HaveCorrectReportCode()
    {
        // Arrange
        short profitYear = 2024;
        var currentValues = new Dictionary<string, decimal>
        {
            { "PAY444.BeginningBalance", 1000m },
            { "PAY444.CONTRIB", 500m },
            { "PAY444.ALLOC", 100m },
            { "PAY444.DISTRIB", 200m },
            { "PAY444.PAIDALLOC", 50m },
            { "PAY444.EARNINGS", 150m },
            { "PAY444.FORFEITS", 20m },
            { "PAY444.EndingBalance", 1480m }
        };

        // Act
        var result = await _service.ValidateBalanceEquationAsync(profitYear, currentValues, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var group = result.Value;

        // All validations should have PAY444 report code
        foreach (var validation in group.Validations!)
        {
            validation.ReportCode.ShouldBe("PAY444");
        }
    }

    [Fact]
    [Description("PS-1721 : Penny rounding differences are tolerated")]
    public async Task ValidateBalanceEquationAsync_PennyDifference_IsValid()
    {
        // Arrange
        short profitYear = 2024;
        var currentValues = new Dictionary<string, decimal>
        {
            { "PAY444.BeginningBalance", 100000.00m },
            { "PAY444.CONTRIB", 50000.33m },
            { "PAY444.ALLOC", 10000.00m },
            { "PAY444.DISTRIB", 20000.00m },
            { "PAY444.PAIDALLOC", 5000.00m },
            { "PAY444.EARNINGS", 15000.00m },
            { "PAY444.FORFEITS", 2000.00m },
            // Calculated: 148000.33, expected has penny rounding
            { "PAY444.EndingBalance", 148000.33m }
        };

        // Act
        var result = await _service.ValidateBalanceEquationAsync(profitYear, currentValues, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var group = result.Value;

        // Should be valid even with penny differences
        group.IsValid.ShouldBeTrue();

        var calculatedValidation = group.Validations!.First(v => v.FieldName == "CalculatedEndingBalance");
        calculatedValidation.Variance.ShouldNotBeNull();
        Math.Abs(calculatedValidation.Variance.Value).ShouldBeLessThan(0.01m);
    }

    [Fact]
    [Description("PS-1721 : Validation group structure is complete")]
    public async Task ValidateBalanceEquationAsync_VerifiesGroupStructure()
    {
        // Arrange
        short profitYear = 2024;
        var currentValues = new Dictionary<string, decimal>
        {
            { "PAY444.BeginningBalance", 1000m },
            { "PAY444.EndingBalance", 1000m }
        };

        // Act
        var result = await _service.ValidateBalanceEquationAsync(profitYear, currentValues, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var group = result.Value;

        // Verify group properties
        group.GroupName.ShouldNotBeNullOrWhiteSpace();
        group.Description.ShouldNotBeNullOrWhiteSpace();
        group.Description.ShouldNotBeNull();
        group.Description.ShouldContain(profitYear.ToString());
        group.Summary.ShouldNotBeNullOrWhiteSpace();
        group.Summary.ShouldNotBeNull();
        group.Priority.ShouldNotBeNullOrWhiteSpace();
        group.Priority.ShouldNotBeNull();
        group.ValidationRule.ShouldNotBeNullOrWhiteSpace();
        group.ValidationRule.ShouldNotBeNull();
        group.ValidationRule.ShouldContain("Balance Matrix Rule 5");
        group.Validations.ShouldNotBeNull();
        group.Validations.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    [Description("PS-1721 : Negative values in equation are handled correctly")]
    public async Task ValidateBalanceEquationAsync_NegativeValues_CalculatesCorrectly()
    {
        // Arrange
        short profitYear = 2024;
        var currentValues = new Dictionary<string, decimal>
        {
            { "PAY444.BeginningBalance", 100000m },
            { "PAY444.CONTRIB", 0m },
            { "PAY444.ALLOC", 0m },
            { "PAY444.DISTRIB", 50000m }, // Large distribution
            { "PAY444.PAIDALLOC", 0m },
            { "PAY444.EARNINGS", -10000m }, // Loss
            { "PAY444.FORFEITS", 0m },
            // Ending = 100000 + 0 + 0 - 50000 - 0 + (-10000) - 0 = 40000
            { "PAY444.EndingBalance", 40000m }
        };

        // Act
        var result = await _service.ValidateBalanceEquationAsync(profitYear, currentValues, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var group = result.Value;
        group.IsValid.ShouldBeTrue();

        var calculatedValidation = group.Validations!.First(v => v.FieldName == "CalculatedEndingBalance");
        calculatedValidation.CurrentValue.ShouldBe(40000m);
        calculatedValidation.ExpectedValue.ShouldBe(40000m);
    }

    [Fact]
    [Description("PS-1721 : Empty dictionary returns default values")]
    public async Task ValidateBalanceEquationAsync_EmptyDictionary_UsesDefaults()
    {
        // Arrange
        short profitYear = 2024;
        var currentValues = new Dictionary<string, decimal>();

        // Act
        var result = await _service.ValidateBalanceEquationAsync(profitYear, currentValues, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var group = result.Value;

        // With all defaults at 0, calculated = 0 and expected = 0, should be valid
        group.IsValid.ShouldBeTrue();

        var calculatedValidation = group.Validations!.First(v => v.FieldName == "CalculatedEndingBalance");
        calculatedValidation.CurrentValue.ShouldBe(0m);
        calculatedValidation.ExpectedValue.ShouldBe(0m);
    }
}
