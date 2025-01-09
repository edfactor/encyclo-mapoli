using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Services.ProfitShareEdit;

public class BeneficiaryTests : ApiTestBase<Program>
{
    private readonly IProfitShareEditService _service;

    public BeneficiaryTests()
    {
        // create mock database with just 1 bene
        MockDbContextFactory = new ScenarioFactory { Beneficiaries = [StockFactory.CreateBeneficiary()] }.BuildMocks();

        _service = ServiceProvider?.GetRequiredService<IProfitShareEditService>()!;
    }

    [Fact]
    public async Task EnsureBeneficiaryHappyPath()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = 2024 };

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(req, CancellationToken.None);

        // Assert
        response.Response.Results.Count().Should().Be(1);

        // TBD Validate all Bene fields (they will be changing as the service is fully implemented)
        // Will be completed in PS-579
    }
    
}
