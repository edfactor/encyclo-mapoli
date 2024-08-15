using System.ComponentModel;
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

public class DemographicsServiceTests : IClassFixture<ApiTestBase<Program>>
{
    private readonly DemographicMapper _mapper;
    private readonly DemographicsClient _demographicsClient;

    public DemographicsServiceTests(ApiTestBase<Program> fixture)
    {
        _demographicsClient = new DemographicsClient(fixture.ApiClient);
        _mapper = new DemographicMapper(new AddressMapper(), 
            new ContactInfoMapper(), 
            new DepartmentMapper(), 
            new EmploymentTypeMapper(),
            new PayFrequencyMapper(), 
            new GenderMapper(), 
            new TerminationCodeMapper());
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

        ISet<DemographicResponseDto>? response = await _demographicsClient.AddDemographics(demographicsRequest, cancellationToken: CancellationToken.None);

        response.Should().NotBeNull();
        response.Should().HaveCount(count);
        _ = response!.Select(d => d.SSN.Should().MatchRegex(@"XXX-XX-\d{4}"));

        Dictionary<int, DemographicResponseDto> responseDict = response!.ToDictionary(k => k.BadgeNumber);

        demographics.ForEach(d =>
        {
            if (!responseDict.TryGetValue(d.BadgeNumber, out DemographicResponseDto? responseDto))
            {
                Assert.Fail($"Unable to find the matching BadgeNumber: {d.BadgeNumber}");
            }
            d.ShouldBeEquivalentTo(responseDto);
        });
    }
}
