using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Services;
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
        req.BeneficiarySsn = null;

        await _service.UpdateBeneficiary(req, CancellationToken.None);
        _beneficiary.Relationship.Should().Be("2nd Cousin");
        _beneficiary.Contact.Ssn.Should().Be(initialSsn);

        int newSsn = 999887777;
        req.BeneficiarySsn = newSsn;
        await _service.UpdateBeneficiary(req, CancellationToken.None);
        _beneficiary.Contact.Ssn.Should().Be(newSsn);

    }
}
