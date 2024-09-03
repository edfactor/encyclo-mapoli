using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Base;
using FluentAssertions;

namespace Demoulas.ProfitSharing.UnitTests;

public class PayProfitServiceTests : IClassFixture<ApiTestBase<Program>>
{
    private readonly PayProfitClient _payProfitClient;

    public PayProfitServiceTests(ApiTestBase<Program> fixture)
    {
        _payProfitClient = new PayProfitClient(fixture.ApiClient);
    }

    [Fact(DisplayName = "Get all the profit!")]
    public async Task GetAllDemographicsTest()
    {
        _payProfitClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        PaginatedResponseDto<PayProfitResponseDto>? response = await _payProfitClient.GetAllProfits(new PaginationRequestDto(), cancellationToken: CancellationToken.None);

        response.Should().NotBeNull();
        response!.Results.Should().HaveCountGreaterOrEqualTo(100);
    }

    [Fact(DisplayName ="PayProfit auth check")]
    public async Task PayProfitAuthCheck()
    {
        _payProfitClient.CreateAndAssignTokenForClient();
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            _ = await _payProfitClient.GetAllProfits(new PaginationRequestDto(), cancellationToken: CancellationToken.None);
        });

    }
}
