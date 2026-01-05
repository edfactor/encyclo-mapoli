using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

/// <summary>
/// Unit tests for RmdsFactorService to verify RMD (Required Minimum Distribution) factor management.
/// Tests CRUD operations against seeded IRS Publication 590-B Uniform Lifetime Table data.
/// </summary>
[Collection("Administration Tests")]
public sealed class RmdsFactorServiceTests : ApiTestBase<Api.Program>
{
    private readonly IRmdsFactorService _rmdsService;

    public RmdsFactorServiceTests()
    {
        _rmdsService = this.ServiceProvider?.GetRequiredService<IRmdsFactorService>() 
            ?? throw new NullReferenceException("IRmdsFactorService not registered in DI container");
    }

    [Fact(DisplayName = "RmdsFactorService - Should return all RMD factors from database")]
    [Description("PS-2320 : Retrieves all IRS RMD life expectancy factors from seed data")]
    public async Task GetAllAsync_ShouldReturnAllRmdsFactors()
    {
        // Act
        var result = await _rmdsService.GetAllAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBeGreaterThanOrEqualTo(27); // Seed data has ages 73-99
    }

    [Fact(DisplayName = "RmdsFactorService - Should return RMDs ordered by age")]
    [Description("PS-2320 : Verifies results are sorted by age ascending")]
    public async Task GetAllAsync_ShouldReturnOrderedByAge()
    {
        // Act
        var result = await _rmdsService.GetAllAsync(CancellationToken.None);

        // Assert
        var ages = result.Select(r => r.Age).ToList();
        var sortedAges = ages.OrderBy(a => a).ToList();
        ages.ShouldBe(sortedAges);
    }

    [Fact(DisplayName = "RmdsFactorService - Should return correct DTO structure")]
    [Description("PS-2320 : Validates RmdsFactorDto structure")]
    public async Task GetAllAsync_ShouldReturnCorrectDtoStructure()
    {
        // Act
        var result = await _rmdsService.GetAllAsync(CancellationToken.None);

        // Assert
        result.ShouldAllBe(rmd =>
            rmd.Age >= 73 && rmd.Age <= 120 && // Valid age range
            rmd.Factor > 0 && rmd.Factor <= 100 // Valid factor range
        );
    }

    [Fact(DisplayName = "RmdsFactorService - Should retrieve specific RMD by age")]
    [Description("PS-2320 : Gets RMD factor for age 73 (first seeded record)")]
    public async Task GetByAgeAsync_ShouldReturnRmdForAge73()
    {
        // Arrange
        const byte age = 73;

        // Act
        var result = await _rmdsService.GetByAgeAsync(age, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Age.ShouldBe(age);
        result.Factor.ShouldBe(26.5m); // IRS factor for age 73
    }

    [Fact(DisplayName = "RmdsFactorService - Should return null for non-existent age")]
    [Description("PS-2320 : Validates null return when age not found")]
    public async Task GetByAgeAsync_ShouldReturnNullForNonExistentAge()
    {
        // Arrange
        const byte age = 120; // Beyond seeded data

        // Act
        var result = await _rmdsService.GetByAgeAsync(age, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
    }

    [Fact(DisplayName = "RmdsFactorService - Should add new RMD factor")]
    [Description("PS-2320 : Creates new RMD record for age not in seed data")]
    public async Task UpsertAsync_ShouldAddNewRmdsFactor()
    {
        // Arrange
        var request = new RmdsFactorRequest
        {
            Age = 100, // Age not in seed data (73-99)
            Factor = 6.3m
        };

        // Act
        var result = await _rmdsService.UpsertAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Age.ShouldBe(request.Age);
        result.Factor.ShouldBe(request.Factor);

        // Verify it was persisted
        var retrieved = await _rmdsService.GetByAgeAsync(request.Age, CancellationToken.None);
        retrieved.ShouldNotBeNull();
        retrieved!.Factor.ShouldBe(request.Factor);
    }

    [Fact(DisplayName = "RmdsFactorService - Should update existing RMD factor")]
    [Description("PS-2320 : Updates factor for existing age")]
    public async Task UpsertAsync_ShouldUpdateExistingRmdsFactor()
    {
        // Arrange - Use age 73 from seed data (original factor: 26.5)
        var request = new RmdsFactorRequest
        {
            Age = 73,
            Factor = 27.0m // Updated factor
        };

        // Act
        var result = await _rmdsService.UpsertAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Age.ShouldBe(request.Age);
        result.Factor.ShouldBe(request.Factor);

        // Verify update was persisted
        var retrieved = await _rmdsService.GetByAgeAsync(request.Age, CancellationToken.None);
        retrieved.ShouldNotBeNull();
        retrieved!.Factor.ShouldBe(27.0m);
    }

    [Fact(DisplayName = "RmdsFactorService - Should delete existing RMD factor")]
    [Description("PS-2320 : Removes RMD record by age")]
    public async Task DeleteAsync_ShouldDeleteExistingRmdsFactor()
    {
        // Arrange - Add a temporary record first
        var tempRequest = new RmdsFactorRequest
        {
            Age = 101,
            Factor = 5.9m
        };
        await _rmdsService.UpsertAsync(tempRequest, CancellationToken.None);

        // Act
        var deleteResult = await _rmdsService.DeleteAsync(tempRequest.Age, CancellationToken.None);

        // Assert
        deleteResult.ShouldBeTrue();

        // Verify it was deleted
        var retrieved = await _rmdsService.GetByAgeAsync(tempRequest.Age, CancellationToken.None);
        retrieved.ShouldBeNull();
    }

    [Fact(DisplayName = "RmdsFactorService - Should return false when deleting non-existent age")]
    [Description("PS-2320 : Validates delete returns false for missing record")]
    public async Task DeleteAsync_ShouldReturnFalseForNonExistentAge()
    {
        // Arrange
        const byte age = 120;

        // Act
        var result = await _rmdsService.DeleteAsync(age, CancellationToken.None);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact(DisplayName = "RmdsFactorService - Should support cancellation token")]
    [Description("PS-2320 : Verifies cancellation token support")]
    public async Task GetAllAsync_ShouldHandleCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var task = _rmdsService.GetAllAsync(cts.Token);

        // Assert - Should not throw when token is valid
        var result = await task;
        result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "RmdsFactorService - Should be consistent across multiple calls")]
    [Description("PS-2320 : Verifies consistent query results")]
    public async Task GetAllAsync_ShouldBeConsistentAcrossMultipleCalls()
    {
        // Act
        var result1 = await _rmdsService.GetAllAsync(CancellationToken.None);
        var result2 = await _rmdsService.GetAllAsync(CancellationToken.None);

        // Assert
        result1.Count.ShouldBe(result2.Count);
        result1.Select(r => r.Age).OrderBy(a => a)
            .ShouldBe(result2.Select(r => r.Age).OrderBy(a => a));
    }

    [Fact(DisplayName = "RmdsFactorService - Should have known IRS factor for age 73")]
    [Description("PS-2320 : Validates seed data accuracy for first RMD age")]
    public async Task GetByAgeAsync_ShouldHaveCorrectFactorForAge73()
    {
        // Act
        var result = await _rmdsService.GetByAgeAsync(73, CancellationToken.None);

        // Assert - IRS Publication 590-B Uniform Lifetime Table
        result.ShouldNotBeNull();
        result.Factor.ShouldBe(26.5m);
    }

    [Fact(DisplayName = "RmdsFactorService - Should have known IRS factor for age 80")]
    [Description("PS-2320 : Validates seed data accuracy for mid-range age")]
    public async Task GetByAgeAsync_ShouldHaveCorrectFactorForAge80()
    {
        // Act
        var result = await _rmdsService.GetByAgeAsync(80, CancellationToken.None);

        // Assert - IRS Publication 590-B Uniform Lifetime Table
        result.ShouldNotBeNull();
        result.Factor.ShouldBe(20.2m);
    }

    [Fact(DisplayName = "RmdsFactorService - Should have known IRS factor for age 99")]
    [Description("PS-2320 : Validates seed data accuracy for last seeded age")]
    public async Task GetByAgeAsync_ShouldHaveCorrectFactorForAge99()
    {
        // Act
        var result = await _rmdsService.GetByAgeAsync(99, CancellationToken.None);

        // Assert - IRS Publication 590-B Uniform Lifetime Table
        result.ShouldNotBeNull();
        result.Factor.ShouldBe(6.8m);
    }

    [Fact(DisplayName = "RmdsFactorService - Should handle boundary age values")]
    [Description("PS-2320 : Tests minimum and maximum age boundaries")]
    public async Task GetByAgeAsync_ShouldHandleBoundaryAges()
    {
        // Act
        var minAge = await _rmdsService.GetByAgeAsync(73, CancellationToken.None);
        var maxAge = await _rmdsService.GetByAgeAsync(99, CancellationToken.None);
        var belowMin = await _rmdsService.GetByAgeAsync(72, CancellationToken.None);
        var aboveMax = await _rmdsService.GetByAgeAsync(120, CancellationToken.None);

        // Assert
        minAge.ShouldNotBeNull();
        maxAge.ShouldNotBeNull();
        belowMin.ShouldBeNull(); // Below minimum seeded age
        aboveMax.ShouldBeNull(); // Above maximum seeded age
    }

    [Fact(DisplayName = "RmdsFactorService - Should support multiple sequential operations")]
    [Description("PS-2320 : Validates CRUD operation sequence")]
    public async Task UpsertAsync_ShouldSupportMultipleOperations()
    {
        // Arrange
        const byte testAge = 102;

        // Act & Assert - Add
        var addRequest = new RmdsFactorRequest { Age = testAge, Factor = 5.5m };
        var addResult = await _rmdsService.UpsertAsync(addRequest, CancellationToken.None);
        addResult.Factor.ShouldBe(5.5m);

        // Act & Assert - Update
        var updateRequest = new RmdsFactorRequest { Age = testAge, Factor = 5.6m };
        var updateResult = await _rmdsService.UpsertAsync(updateRequest, CancellationToken.None);
        updateResult.Factor.ShouldBe(5.6m);

        // Act & Assert - Delete
        var deleteResult = await _rmdsService.DeleteAsync(testAge, CancellationToken.None);
        deleteResult.ShouldBeTrue();

        // Act & Assert - Verify deletion
        var finalCheck = await _rmdsService.GetByAgeAsync(testAge, CancellationToken.None);
        finalCheck.ShouldBeNull();
    }
}
