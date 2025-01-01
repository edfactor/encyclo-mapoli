using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Services.ProfitShareEdit;

public class EmployeeTests : ApiTestBase<Program>
{
    private readonly IProfitShareEditService _service;

    public EmployeeTests()
    {
        // create mock database with just 1 employee
        MockDbContextFactory = new ScenarioFactory().CreateOneEmployee().BuildMocks();

        _service = ServiceProvider?.GetRequiredService<IProfitShareEditService>()!;
    }

    [Fact]
    public async Task EnsureEmployeeHappyPath()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = await GetMaxProfitYearAsync(TestContext.Current.CancellationToken) };

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(req, CancellationToken.None);

        // Assert
        List<ProfitShareEditMemberRecordResponse> members = response.Response.Results.ToList();
        members.Count.Should().Be(1);


        // TBD Validate all Employee fields (they will be changing soon.)
        // Will be completed in PS-579
    }

    // --- Completing these tests is part of PS-579

    // Test Case: There are 9 code paths for employees based on ZeroConf earnings 

    // Test Case: employee who is also bene 

    // Test Case: validate totals computation 
}
