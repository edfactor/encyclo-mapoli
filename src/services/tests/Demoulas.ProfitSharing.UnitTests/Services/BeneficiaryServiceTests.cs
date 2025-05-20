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
    public async Task CreateNewBeneficiaryAndContact()
    {
        var newBeneficiaryReq = new CreateBeneficiaryRequest()
        {
            EmployeeBadgeNumber = _demographic.demographic.BadgeNumber,
            BeneficiarySsn = 111223333,
            Relationship = "Son",
            KindId = 'P',
            Percentage = 100,
            DateOfBirth = new DateOnly(2004, 12, 22),
            Street = "5 Maple St",
            City = "Tewksbury",
            State = "MA",
            PostalCode = "01862",
            CountryIso = "US",
            FirstName = "Mickey",
            LastName = "Smith",
            MiddleName = "Joshua",
            PhoneNumber = "5085552526",
            MobileNumber = "5085555553",
            EmailAddress = "mickey@gmail.com"
        };

        var resp = await _service.CreateBeneficiary(newBeneficiaryReq, CancellationToken.None);
        resp.Should().NotBeNull();
        resp.PsnSuffix.Should().Be(1000);
        resp.ContactExisted.Should().BeFalse();
    }

    [Fact]
    public async Task CreateNewBeneficiaryButContactExists()
    {
        var newBeneficiaryReq = new CreateBeneficiaryRequest()
        {
            EmployeeBadgeNumber = _demographic.demographic.BadgeNumber,
            BeneficiarySsn = _beneficiary.Contact!.Ssn,
            Relationship = "Son",
            KindId = 'P',
            Percentage = 100,
            DateOfBirth = new DateOnly(2004, 12, 22),
            Street = "5 Maple St",
            City = "Tewksbury",
            State = "MA",
            PostalCode = "01862",
            CountryIso = "US",
            FirstName = "Mickey",
            LastName = "Smith",
            MiddleName = "Joshua",
            PhoneNumber = "5085552526",
            MobileNumber = "5085555553",
            EmailAddress = "mickey@gmail.com"
        };

        var resp = await _service.CreateBeneficiary(newBeneficiaryReq, CancellationToken.None);
        resp.Should().NotBeNull();
        resp.PsnSuffix.Should().Be(1000);
        resp.ContactExisted.Should().BeFalse();
    }

    [Fact]
    public async Task CreateNewBeneficiaryWithFirstLevelContact()
    {
        var newBeneficiaryReq = new CreateBeneficiaryRequest()
        {
            EmployeeBadgeNumber = _demographic.demographic.BadgeNumber,
            BeneficiarySsn = _beneficiary.Contact!.Ssn,
            Relationship = "Son",
            KindId = 'P',
            Percentage = 100,
            DateOfBirth = new DateOnly(2004, 12, 22),
            Street = "5 Maple St",
            City = "Tewksbury",
            State = "MA",
            PostalCode = "01862",
            CountryIso = "US",
            FirstName = "Mickey",
            LastName = "Smith",
            MiddleName = "Joshua",
            PhoneNumber = "5085552526",
            MobileNumber = "5085555553",
            EmailAddress = "mickey@gmail.com"
        };

        var resp = await _service.CreateBeneficiary(newBeneficiaryReq, CancellationToken.None);
        resp.Should().NotBeNull();
        resp.PsnSuffix.Should().Be(1000);
        resp.ContactExisted.Should().BeFalse();

        newBeneficiaryReq.FirstLevelBeneficiaryNumber = 1;
        newBeneficiaryReq.BeneficiarySsn = 111223334;
        resp = await _service.CreateBeneficiary(newBeneficiaryReq, CancellationToken.None);
        resp.Should().NotBeNull();
        resp.PsnSuffix.Should().Be(1100);
    }

    [Fact]
    public async Task CreateNewBeneficiaryWithSecondLevelContact()
    {
        var newBeneficiaryReq = new CreateBeneficiaryRequest()
        {
            EmployeeBadgeNumber = _demographic.demographic.BadgeNumber,
            BeneficiarySsn = _beneficiary.Contact!.Ssn,
            Relationship = "Son",
            KindId = 'P',
            Percentage = 100,
            DateOfBirth = new DateOnly(2004, 12, 22),
            Street = "5 Maple St",
            City = "Tewksbury",
            State = "MA",
            PostalCode = "01862",
            CountryIso = "US",
            FirstName = "Mickey",
            LastName = "Smith",
            MiddleName = "Joshua",
            PhoneNumber = "5085552526",
            MobileNumber = "5085555553",
            EmailAddress = "mickey@gmail.com"
        };

        var resp = await _service.CreateBeneficiary(newBeneficiaryReq, CancellationToken.None);
        resp.Should().NotBeNull();
        resp.PsnSuffix.Should().Be(1000);
        resp.ContactExisted.Should().BeFalse();

        newBeneficiaryReq.FirstLevelBeneficiaryNumber = 1;
        newBeneficiaryReq.BeneficiarySsn = 111223334;
        resp = await _service.CreateBeneficiary(newBeneficiaryReq, CancellationToken.None);
        resp.Should().NotBeNull();
        resp.PsnSuffix.Should().Be(1100);

        newBeneficiaryReq.SecondLevelBeneficiaryNumber = 1;
        newBeneficiaryReq.BeneficiarySsn = 111223335;
        resp = await _service.CreateBeneficiary(newBeneficiaryReq, CancellationToken.None);
        resp.Should().NotBeNull();
        resp.PsnSuffix.Should().Be(1110);
    }

    [Fact]
    public async Task CreateNewBeneficiaryWithThirdLevelContact()
    {
        var newBeneficiaryReq = new CreateBeneficiaryRequest()
        {
            EmployeeBadgeNumber = _demographic.demographic.BadgeNumber,
            BeneficiarySsn = _beneficiary.Contact!.Ssn,
            Relationship = "Son",
            KindId = 'P',
            Percentage = 100,
            DateOfBirth = new DateOnly(2004, 12, 22),
            Street = "5 Maple St",
            City = "Tewksbury",
            State = "MA",
            PostalCode = "01862",
            CountryIso = "US",
            FirstName = "Mickey",
            LastName = "Smith",
            MiddleName = "Joshua",
            PhoneNumber = "5085552526",
            MobileNumber = "5085555553",
            EmailAddress = "mickey@gmail.com"
        };

        var resp = await _service.CreateBeneficiary(newBeneficiaryReq, CancellationToken.None);
        resp.Should().NotBeNull();
        resp.PsnSuffix.Should().Be(1000);
        resp.ContactExisted.Should().BeFalse();

        newBeneficiaryReq.FirstLevelBeneficiaryNumber = 1;
        newBeneficiaryReq.BeneficiarySsn = 111223334;
        resp = await _service.CreateBeneficiary(newBeneficiaryReq, CancellationToken.None);
        resp.Should().NotBeNull();
        resp.PsnSuffix.Should().Be(1100);

        newBeneficiaryReq.SecondLevelBeneficiaryNumber = 1;
        newBeneficiaryReq.BeneficiarySsn = 111223335;
        resp = await _service.CreateBeneficiary(newBeneficiaryReq, CancellationToken.None);
        resp.Should().NotBeNull();
        resp.PsnSuffix.Should().Be(1110);

        newBeneficiaryReq.ThirdLevelBeneficiaryNumber = 1;
        newBeneficiaryReq.BeneficiarySsn = 111223336;
        resp = await _service.CreateBeneficiary(newBeneficiaryReq, CancellationToken.None);
        resp.Should().NotBeNull();
        resp.PsnSuffix.Should().Be(1111);
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
