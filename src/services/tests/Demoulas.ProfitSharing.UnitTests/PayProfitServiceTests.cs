using System.ComponentModel;
using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services.Mappers;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using Demoulas.ProfitSharing.UnitTests.Fakes;
using FluentAssertions;

namespace Demoulas.ProfitSharing.UnitTests;

public class PayProfitServiceTests : IClassFixture<ApiTestBase<Program>>
{
    private readonly DemographicMapper _mapper;
    private readonly PayProfitClient _demographicsClient;

    public PayProfitServiceTests(ApiTestBase<Program> fixture)
    {
        _demographicsClient = new PayProfitClient(fixture.ApiClient);
        _mapper = new DemographicMapper(new AddressMapper(), 
            new ContactInfoMapper(), 
            new DepartmentMapper(), 
            new EmploymentTypeMapper(),
            new PayFrequencyMapper(), 
            new GenderMapper(), 
            new TerminationCodeMapper());
    }

    [Fact(DisplayName = "Get all the profit!")]
    public async Task GetAllDemographicsTest()
    {
        PaginatedResponseDto<DemographicsResponseDto>? response = await _demographicsClient.GetAllProfits(new PaginationRequestDto(), cancellationToken: CancellationToken.None);

        response.Should().NotBeNull();
        response!.Results.Should().HaveCountGreaterOrEqualTo(100);
        _ = response!.Results.Select(d=> d.SSN.Should().MatchRegex(@"XXX-XX-\d{4}"));
    }
}
