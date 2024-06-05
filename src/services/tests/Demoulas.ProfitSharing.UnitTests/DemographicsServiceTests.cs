using System.ComponentModel;
using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.IntegrationTests.Base;
using Demoulas.ProfitSharing.IntegrationTests.Fakes;
using Demoulas.ProfitSharing.Services.Mappers;
using FluentAssertions;

namespace Demoulas.ProfitSharing.IntegrationTests;

public class DemographicsServiceTests : IClassFixture<ApiTestBase<Program>>
{
    private readonly ApiTestBase<Program> _fixture;
    private readonly DemographicMapper _mapper;
    private readonly DemographicsClient _demographicsClient;

    public DemographicsServiceTests(ApiTestBase<Program> fixture)
    {
        _fixture = fixture;
        _demographicsClient = new DemographicsClient(_fixture.ApiClient);
        _mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());
    }

    [Fact(DisplayName = "Get all demographics")]
    public async Task GetAllDemographicsTest()
    {
        PaginatedResponseDto<DemographicsResponseDto>? response = await _demographicsClient.GetAllDemographics(new PaginationRequestDto(), cancellationToken: CancellationToken.None);

        response.Should().NotBeNull();
        response!.Results.Should().HaveCountGreaterOrEqualTo(100);
    }

    [Theory(DisplayName = "Add new demographics")]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    [Description("https://demoulas.atlassian.net/browse/PS-82")]
    public async Task AddNewDemographicsTest(int count)
    {
        List<Demographic>? demographics = new DemographicFaker().Generate(count);
        IEnumerable<DemographicsRequestDto> demographicsRequest = _mapper.MapToRequest(demographics);

        ISet<DemographicsResponseDto>? response = await _demographicsClient.AddDemographics(demographicsRequest, cancellationToken: CancellationToken.None);

        response.Should().NotBeNull();
        response.Should().HaveCount(count);
    }
}
