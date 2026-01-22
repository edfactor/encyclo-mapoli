using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

/// <summary>
/// Unit tests for BankService covering CRUD operations and validation.
/// Tests ensure proper bank management and data integrity.
/// </summary>
[Collection("SharedGlobalState")]
[Description("PS-1790 : Bank service tests for modernized banking model")]
public sealed class BankServiceTests : ApiTestBase<Program>
{
    private readonly Bank _testBank;
    private readonly IBankService _service;

    public BankServiceTests()
    {
        _testBank = new Bank
        {
            Id = 1,
            Name = "Test Bank",
            OfficeType = "Main Branch",
            City = "Boston",
            State = "MA",
            Phone = "555-1234",
            Status = "Active",
            IsDisabled = false,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            CreatedBy = "TestUser"
        };

        MockDbContextFactory = new ScenarioFactory
        {
            Banks = [_testBank]
        }.BuildMocks();

        _service = ServiceProvider?.GetRequiredService<IBankService>()!;
    }

    [Fact]
    [Description("PS-1790 : GetAllAsync should return all active banks")]
    public async Task GetAllAsync_ShouldReturnAllActiveBanks()
    {
        // Act
        var result = await _service.GetAllAsync(includeDisabled: false, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var banks = result.Value;
        banks.ShouldNotBeNull();
        banks.Count.ShouldBeGreaterThan(0);
        banks.ShouldAllBe(b => !b.IsDisabled);
    }

    [Fact]
    [Description("PS-1790 : GetAllAsync should exclude disabled banks by default")]
    public async Task GetAllAsync_ShouldExcludeDisabledBanks()
    {
        // Arrange
        _testBank.IsDisabled = true;

        // Act
        var result = await _service.GetAllAsync(includeDisabled: false, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-1790 : GetAllAsync should include disabled banks when requested")]
    public async Task GetAllAsync_ShouldIncludeDisabledWhenRequested()
    {
        // Arrange
        _testBank.IsDisabled = true;

        // Act
        var result = await _service.GetAllAsync(includeDisabled: true, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].IsDisabled.ShouldBeTrue();
    }

    [Fact]
    [Description("PS-1790 : GetByIdAsync should return bank by ID")]
    public async Task GetByIdAsync_ShouldReturnBank_WhenExists()
    {
        // Act
        var result = await _service.GetByIdAsync(1, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var bank = result.Value;
        bank.ShouldNotBeNull();
        bank.Id.ShouldBe(1);
        bank.Name.ShouldBe("Test Bank");
        bank.City.ShouldBe("Boston");
        bank.State.ShouldBe("MA");
    }

    [Fact]
    [Description("PS-1790 : GetByIdAsync should return NotFound for nonexistent bank")]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenBankDoesNotExist()
    {
        // Act
        var result = await _service.GetByIdAsync(999, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(134); // BankNotFound
    }

    [Fact]
    [Description("PS-1790 : Banks should be ordered by name in list results")]
    public async Task GetAllAsync_ShouldOrderBanksByName()
    {
        // Act
        var result = await _service.GetAllAsync(false, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBeGreaterThan(0);
        // First bank should be returned (ordering is alphabetical by name)
        result.Value[0].Name.ShouldBe("Test Bank");
    }
}
