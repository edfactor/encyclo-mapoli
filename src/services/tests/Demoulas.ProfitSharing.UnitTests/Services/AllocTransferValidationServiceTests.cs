using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Services.Validation;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Services;

[Description("PS-1721 : Unit tests for AllocTransferValidationService - ALLOC/PAID ALLOC transfer validation")]
public class AllocTransferValidationServiceTests
{
    public AllocTransferValidationServiceTests()
    {
        // Initialize telemetry before running tests
        EndpointTelemetry.Initialize();
    }

    [Fact]
    [Description("PS-1721 : Balanced transfers with MockDataContextFactory - returns valid group")]
    public async Task ValidateAllocTransfersAsync_WithMockData_ReturnsValidGroup()
    {
        // Arrange
        var factory = MockDataContextFactory.InitializeForTesting();
        var logger = new Mock<ILogger<AllocTransferValidationService>>();
        var service = new AllocTransferValidationService(factory, logger.Object);

        // Act  
        var result = await service.ValidateAllocTransfersAsync(2024, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        var group = result.Value;
        group.GroupName.ShouldBe("ALLOC/PAID ALLOC Transfers");
        group.Validations.Count.ShouldBe(3);
        group.Priority.ShouldBe("Critical");

        // Verify all expected validation fields exist
        group.Validations.Any(v => v.FieldName == "IncomingAllocations").ShouldBeTrue();
        group.Validations.Any(v => v.FieldName == "OutgoingAllocations").ShouldBeTrue();
        group.Validations.Any(v => v.FieldName == "NetAllocTransfer").ShouldBeTrue();
    }

    [Fact]
    [Description("PS-1721 : Non-existent year returns zero balances")]
    public async Task ValidateAllocTransfersAsync_NonExistentYear_ReturnsZeroBalances()
    {
        // Arrange
        var factory = MockDataContextFactory.InitializeForTesting();
        var logger = new Mock<ILogger<AllocTransferValidationService>>();
        var service = new AllocTransferValidationService(factory, logger.Object);

        // Using a year that doesn't exist in mock data
        short profitYear = 1999;

        // Act
        var result = await service.ValidateAllocTransfersAsync(profitYear, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        var group = result.Value;
        group.IsValid.ShouldBeTrue();

        // All validations should show zero for non-existent year
        var incomingValidation = group.Validations.FirstOrDefault(v => v.FieldName == "IncomingAllocations");
        incomingValidation.ShouldNotBeNull();
        incomingValidation.CurrentValue.ShouldBe(0m);

        var outgoingValidation = group.Validations.FirstOrDefault(v => v.FieldName == "OutgoingAllocations");
        outgoingValidation.ShouldNotBeNull();
        outgoingValidation.CurrentValue.ShouldBe(0m);

        var netTransferValidation = group.Validations.FirstOrDefault(v => v.FieldName == "NetAllocTransfer");
        netTransferValidation.ShouldNotBeNull();
        netTransferValidation.CurrentValue.ShouldBe(0m);
    }

    [Fact]
    [Description("PS-1721 : Validation group structure is correct")]
    public async Task ValidateAllocTransfersAsync_VerifiesGroupStructure()
    {
        // Arrange
        var factory = MockDataContextFactory.InitializeForTesting();
        var logger = new Mock<ILogger<AllocTransferValidationService>>();
        var service = new AllocTransferValidationService(factory, logger.Object);

        // Act
        var result = await service.ValidateAllocTransfersAsync(2024, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();

        var group = result.Value!;
        group.GroupName.ShouldBe("ALLOC/PAID ALLOC Transfers");
        group.Priority.ShouldBe("Critical");
        group.ValidationRule.ShouldNotBeNull();
        group.ValidationRule.ShouldContain("Sum(ALLOC) + Sum(PAID ALLOC) must equal 0");
        group.Description.ShouldNotBeNull();
        group.Description.ShouldContain("code 6");
        group.Description.ShouldContain("code 5");
        group.Validations.Count.ShouldBe(3);
    }
}
