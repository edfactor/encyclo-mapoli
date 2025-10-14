using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Master;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints;

public class MasterInquiryEmployeeDetailsTests : ApiTestBase<Api.Program>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly short profitYear = 2024; // Default profit year for tests

    public MasterInquiryEmployeeDetailsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact(DisplayName = "PS-433: Employee Details - End Profit Year")]
    public async Task GetEmployeeDetailsWithEndProfitYear()
    {
        // Use FINANCEMANAGER and EXECUTIVEADMIN to get unmasked data for all employees including executives
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER, Role.EXECUTIVEADMIN);
        var request = new MasterInquiryRequest { EndProfitYear = 2023, Skip = 0, Take = 25 };
        var response = await ApiClient.POSTAsync<MasterInquirySearchEndpoint, MasterInquiryRequest, PaginatedResponseDto<MemberDetails>>(request);
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
    }

    [Fact(DisplayName = "PS-433: Employee Details - Profit Code")]
    public async Task GetEmployeeDetailsWithProfitCode()
    {
        // Use FINANCEMANAGER and EXECUTIVEADMIN to get unmasked data for all employees including executives
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER, Role.EXECUTIVEADMIN);
        var request = new MasterInquiryRequest { ProfitCode = 1, Skip = 0, Take = 25, ProfitYear = profitYear }; // Use a valid ProfitCode constant as needed
        var response = await ApiClient.POSTAsync<MasterInquirySearchEndpoint, MasterInquiryRequest, PaginatedResponseDto<MemberDetails>>(request);
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Response.EnsureSuccessStatusCode();
    }

    [Fact(DisplayName = "PS-433: Employee Details - Contribution Amount")]
    public async Task GetEmployeeDetailsWithContributionAmount()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest { ContributionAmount = 100.0m, Skip = 0, Take = 25, ProfitYear = profitYear };
        var response = await ApiClient.POSTAsync<MasterInquirySearchEndpoint, MasterInquiryRequest, PaginatedResponseDto<MemberDetails>>(request);
        response.ShouldNotBeNull();

        if (!response.Response.IsSuccessStatusCode)
        {
            var content = await response.Response.Content.ReadAsStringAsync();
            try
            {
                var problem = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(content);
                _testOutputHelper.WriteLine($"API returned error: {problem?.Title} - {problem?.Detail}");
            }
            catch
            {
                _testOutputHelper.WriteLine($"API returned error: {content}");
            }
        }

        response.Response.EnsureSuccessStatusCode();
    }

    [Fact(DisplayName = "PS-433: Employee Details - Earnings Amount")]
    public async Task GetEmployeeDetailsWithEarningsAmount()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest { EarningsAmount = 0.0m, Skip = 0, Take = 25, ProfitYear = profitYear };
        var response = await ApiClient.POSTAsync<MasterInquirySearchEndpoint, MasterInquiryRequest, PaginatedResponseDto<MemberDetails>>(request);
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
    }

    [Fact(DisplayName = "PS-433: Employee Details - Payment Amount")]
    public async Task GetEmployeeDetailsWithPaymentAmount()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest { PaymentAmount = 0.0m, Skip = 0, Take = 25, ProfitYear = profitYear };
        var response = await ApiClient.POSTAsync<MasterInquirySearchEndpoint, MasterInquiryRequest, PaginatedResponseDto<MemberDetails>>(request);
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
    }

    [Fact(DisplayName = "PS-433: Employee Details - Social Security")]
    public async Task GetEmployeeDetailsWithSocialSecurity()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest { Ssn = 100000000, Skip = 0, Take = 25, ProfitYear = profitYear };
        var response = await ApiClient.POSTAsync<MasterInquirySearchEndpoint, MasterInquiryRequest, PaginatedResponseDto<MemberDetails>>(request);
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
    }

    [Fact(DisplayName = "PS-433: Employee Details - All Filters")]
    public async Task GetEmployeeDetailsWithAllFilters()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest
        {
            EndProfitYear = 2024,
            ContributionAmount = 0,
            EarningsAmount = 0,
            ForfeitureAmount = 0,
            PaymentAmount = 0,
            ProfitCode = 1, // Use a valid ProfitCode constant as needed
            Ssn = 000000000,
            Skip = 0,
            Take = 25,
            ProfitYear = profitYear
        };
        var response = await ApiClient.POSTAsync<MasterInquirySearchEndpoint, MasterInquiryRequest, PaginatedResponseDto<MemberDetails>>(request);
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
    }

    [Fact(DisplayName = "Master Inquiry Search - Empty result returns 200 (Success)")]
    public async Task MasterInquiryEmptyResultReturnsOk()
    {
        // Arrange: Provide two filters to satisfy validator (ProfitYear + Ssn) but pick an improbable Ssn to force empty result.
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest
        {
            ProfitYear = profitYear,
            Ssn = 999999998, // Intentionally improbable SSN to yield no matches; adjust if future fixture data collides.
            Skip = 0,
            Take = 10
        };

        // Act
        var response = await ApiClient.POSTAsync<MasterInquirySearchEndpoint, MasterInquiryRequest, PaginatedResponseDto<MemberDetails>>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK, "Empty filtered result sets must return 200 per endpoint contract.");
        response.Result.Results.ShouldNotBeNull();
        response.Result.Results.Count().ShouldBe(0, "Expected zero results for an SSN that should not exist in seeded test data.");
    }
}
