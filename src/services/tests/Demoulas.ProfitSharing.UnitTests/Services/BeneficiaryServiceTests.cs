using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

[Collection("SharedGlobalState")]
public sealed class BeneficiaryServiceTests : ApiTestBase<Program>
{
    private readonly (Demographic demographic, List<PayProfit> payprofit) _demographic;
    private readonly Beneficiary _beneficiary;
    private readonly IBeneficiaryService _service;

    public BeneficiaryServiceTests()
    {
        _demographic = StockFactory.CreateEmployee(2024);
        _beneficiary = StockFactory.CreateBeneficiary();
        MockDbContextFactory = new ScenarioFactory { Demographics = [_demographic.demographic], Beneficiaries = [_beneficiary] }.BuildMocks();
        _service = ServiceProvider?.GetRequiredService<IBeneficiaryService>()!;
    }

    [Fact]
    public async Task UpdateBeneficiary()
    {
        var initialSsn = _beneficiary.Contact!.Ssn;

        var req = UpdateBeneficiaryRequest.SampleRequest();
        req.Id = _beneficiary.Id;
        req.Relationship = "2nd Cousin";

        await _service.UpdateBeneficiaryAsync(req, CancellationToken.None);
        _beneficiary.Relationship.ShouldBe("2nd Cousin");
        _beneficiary.Contact.Ssn.ShouldBe(initialSsn);
    }
}
