using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services.ProfitShareEdit;

[Collection("SharedGlobalState")]
public class BeneficiaryTests : ApiTestBase<Program>
{
    private readonly Beneficiary _beneficiary;
    private readonly decimal _beneficiaryBalance = 1_000_000;
    private readonly ProfitDetail _profitDetail;
    private readonly IProfitShareEditService _service;

    public BeneficiaryTests()
    {
        // create mock database with just 1 bene
        _beneficiary = StockFactory.CreateBeneficiary();
        _profitDetail = StockFactory.CreateAllocation(1901, _beneficiary.Contact!.Ssn, _beneficiaryBalance);
        MockDbContextFactory = new ScenarioFactory { Beneficiaries = [_beneficiary], ProfitDetails = [_profitDetail] }.BuildMocks();

        _service = ServiceProvider?.GetRequiredService<IProfitShareEditService>()!;
    }

    [Fact]
    public async Task EnsureBeneficiaryHappyPath()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = 2024, EarningsPercent = 11 };

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(req, CancellationToken.None);

        // Assert
        response.Response.ShouldNotBeNull();

    }
}
