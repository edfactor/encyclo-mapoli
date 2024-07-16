using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.UnitTests.Base;
using FluentAssertions;

namespace Demoulas.ProfitSharing.UnitTests;

public class PayProfitServiceTests : IClassFixture<ApiTestBase<Program>>
{
    //private readonly PayProfitMapper _mapper;
    private readonly PayProfitClient _payProfitClient;

    public PayProfitServiceTests(ApiTestBase<Program> fixture)
    {
        _payProfitClient = new PayProfitClient(fixture.ApiClient);
      //  _mapper = new PayProfitMapper();
    }

    [Fact(DisplayName = "Get all the profit!")]
    public async Task GetAllDemographicsTest()
    {
        PaginatedResponseDto<PayProfitResponseDto>? response = await _payProfitClient.GetAllProfits(new PaginationRequestDto(), cancellationToken: CancellationToken.None);

        response.Should().NotBeNull();
        response!.Results.Should().HaveCountGreaterOrEqualTo(100);
    }
}
