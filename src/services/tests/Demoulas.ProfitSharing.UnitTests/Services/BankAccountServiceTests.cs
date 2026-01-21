using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

/// <summary>
/// Unit tests for BankAccountService covering CRUD operations, primary account logic, and validation.
/// Tests ensure proper masking of sensitive account numbers and primary account management.
/// </summary>
[Collection("SharedGlobalState")]
[Description("PS-1790 : Bank account service tests for modernized banking model")]
public sealed class BankAccountServiceTests : ApiTestBase<Program>
{
    private readonly Bank _testBank;
    private readonly BankAccount _primaryAccount;
    private readonly BankAccount _secondaryAccount;
    private readonly IBankAccountService _service;

    public BankAccountServiceTests()
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

        _primaryAccount = new BankAccount
        {
            Id = 1,
            BankId = 1,
            RoutingNumber = "011401533",
            AccountNumber = "1234567890",
            IsPrimary = true,
            IsDisabled = false,
            EffectiveDate = DateOnly.FromDateTime(DateTime.Today),
            CreatedAtUtc = DateTimeOffset.UtcNow,
            CreatedBy = "TestUser",
            Bank = _testBank
        };

        _secondaryAccount = new BankAccount
        {
            Id = 2,
            BankId = 1,
            RoutingNumber = "011401533",
            AccountNumber = "9876543210",
            IsPrimary = false,
            IsDisabled = false,
            EffectiveDate = DateOnly.FromDateTime(DateTime.Today),
            CreatedAtUtc = DateTimeOffset.UtcNow,
            CreatedBy = "TestUser",
            Bank = _testBank
        };

        MockDbContextFactory = new ScenarioFactory
        {
            Banks = [_testBank],
            BankAccounts = [_primaryAccount, _secondaryAccount]
        }.BuildMocks();

        _service = ServiceProvider?.GetRequiredService<IBankAccountService>()!;
    }

    [Fact]
    [Description("PS-1790 : GetByBankIdAsync should return all active accounts for a bank")]
    public async Task GetByBankIdAsync_ShouldReturnActiveAccounts()
    {
        // Act
        var result = await _service.GetByBankIdAsync(1, includeDisabled: false, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var accounts = result.Value;
        accounts.ShouldNotBeNull();
        accounts.Count.ShouldBe(2);
        accounts[0].IsPrimary.ShouldBeTrue(); // Primary should be first
        accounts[0].AccountNumber.ShouldContain("***"); // Should be masked
    }

    [Fact]
    [Description("PS-1790 : GetByBankIdAsync should exclude disabled accounts by default")]
    public async Task GetByBankIdAsync_ShouldExcludeDisabledAccounts()
    {
        // Arrange
        _secondaryAccount.IsDisabled = true;

        // Act
        var result = await _service.GetByBankIdAsync(1, includeDisabled: false, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Id.ShouldBe(1); // Only primary account
    }

    [Fact]
    [Description("PS-1790 : GetByBankIdAsync should include disabled accounts when requested")]
    public async Task GetByBankIdAsync_ShouldIncludeDisabledWhenRequested()
    {
        // Arrange
        _secondaryAccount.IsDisabled = true;

        // Act
        var result = await _service.GetByBankIdAsync(1, includeDisabled: true, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2); // Both accounts
    }

    [Fact]
    [Description("PS-1790 : GetByIdAsync should return account with masked account number")]
    public async Task GetByIdAsync_ShouldReturnMaskedAccount()
    {
        // Act
        var result = await _service.GetByIdAsync(1, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var account = result.Value;
        account.ShouldNotBeNull();
        account.Id.ShouldBe(1);
        account.BankName.ShouldBe("Test Bank");
        account.AccountNumber.ShouldContain("***"); // Sensitive data masked
        account.AccountNumber.ShouldNotBe("1234567890"); // Not plain text
    }

    [Fact]
    [Description("PS-1790 : GetByIdAsync should return NotFound for nonexistent account")]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Act
        var result = await _service.GetByIdAsync(999, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(135); // BankAccountNotFound
    }

    [Fact]
    [Description("PS-1790 : Account numbers should be masked in DTOs for security")]
    public async Task AccountNumbers_ShouldBeMasked_InAllResponses()
    {
        // Act
        var listResult = await _service.GetByBankIdAsync(1, false, CancellationToken.None);
        var singleResult = await _service.GetByIdAsync(1, CancellationToken.None);

        // Assert
        listResult.IsSuccess.ShouldBeTrue();
        listResult.Value.ShouldNotBeNull();
        singleResult.IsSuccess.ShouldBeTrue();
        singleResult.Value.ShouldNotBeNull();

        // Verify masking in list response
        foreach (var account in listResult.Value)
        {
            account.AccountNumber.ShouldContain("***");
            // Masked account number should be present and contain masking characters
        }

        // Verify masking in single response
        singleResult.Value.AccountNumber.ShouldContain("***");
    }

    [Fact]
    [Description("PS-1790 : Primary account should be ordered first in list results")]
    public async Task GetByBankIdAsync_ShouldOrderPrimaryAccountFirst()
    {
        // Act
        var result = await _service.GetByBankIdAsync(1, false, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBeGreaterThan(0);
        result.Value[0].IsPrimary.ShouldBeTrue();
        result.Value[0].Id.ShouldBe(_primaryAccount.Id);
    }
}
