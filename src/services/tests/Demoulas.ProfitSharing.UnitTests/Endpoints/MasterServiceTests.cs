using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Master;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using FluentAssertions;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints;
public class MasterServiceTests : ApiTestBase<Program>
{
   [Fact(DisplayName ="PS-433: Profit Master Inquiry - Start Profit Year")]
    public async Task GetMasterInquiryWithStartProfitYear()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest() { StartProfitYear = 2023, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryEndpoint, MasterInquiryRequest, ReportResponseBase<MasterInquiryResponseDto>> (request);
        response.Should().NotBeNull();
    }

    [Fact(DisplayName = "PS-433: Profit Master Inquiry - End Profit Year")]
    public async Task GetMasterInquiryWithEndProfitYear()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest() { EndProfitYear = 2023, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryEndpoint, MasterInquiryRequest, ReportResponseBase<MasterInquiryResponseDto>>(request);
        response.Should().NotBeNull();
    }

    [Fact(DisplayName = "PS-433: Profit Master Inquiry - Start Profit Month")]
    public async Task GetMasterInquiryWithStartProfitMonth()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest() { StartProfitMonth = 1, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryEndpoint, MasterInquiryRequest, ReportResponseBase<MasterInquiryResponseDto>>(request);
        response.Should().NotBeNull();
    }

    [Fact(DisplayName = "PS-433: Profit Master Inquiry - End Profit Month")]
    public async Task GetMasterInquiryWithEndProfitMonth()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest() { EndProfitMonth = 12, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryEndpoint, MasterInquiryRequest, ReportResponseBase<MasterInquiryResponseDto>>(request);
        response.Should().NotBeNull();
    }

    [Fact(DisplayName = "PS-433: Profit Master Inquiry - Profit Code")]
    public async Task GetMasterInquiryWithProfitCode()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest() { ProfitCode = ProfitCode.Constants.IncomingContributions.Id, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryEndpoint, MasterInquiryRequest, ReportResponseBase<MasterInquiryResponseDto>>(request);
        response.Should().NotBeNull();
    }

    [Fact(DisplayName = "PS-433: Profit Master Inquiry - Contribution Amount")]
    public async Task GetMasterInquiryWithContributionAmount()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest() { ContributionAmount = (decimal?)100.0, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryEndpoint, MasterInquiryRequest, ReportResponseBase<MasterInquiryResponseDto>>(request);
        response.Should().NotBeNull();
    }

    [Fact(DisplayName = "PS-433: Profit Master Inquiry - Earnings Amount")]
    public async Task GetMasterInquiryWithEarningsAmount()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest() { EarningsAmount = (decimal?) 0.0, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryEndpoint, MasterInquiryRequest, ReportResponseBase<MasterInquiryResponseDto>>(request);
        response.Should().NotBeNull();
    }

    [Fact(DisplayName = "PS-433: Profit Master Inquiry - Payment Amount")]
    public async Task GetMasterInquiryWithPaymentAmount()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest() { PaymentAmount = 0, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryEndpoint, MasterInquiryRequest, ReportResponseBase<MasterInquiryResponseDto>>(request);
        response.Should().NotBeNull();
    }

    [Fact(DisplayName = "PS-433: Profit Master Inquiry - Social Security")]
    public async Task GetMasterInquiryWithSocialSecurity()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest() { SocialSecurity = 000000000, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryEndpoint, MasterInquiryRequest, ReportResponseBase<MasterInquiryResponseDto>>(request);
        response.Should().NotBeNull();
    }

     [Fact(DisplayName = "PS-433: Profit Master Inquiry - All")]
    public async Task GetMasterInquiryWithSocialSecurityWithAll()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new MasterInquiryRequest() { StartProfitMonth = 1, EndProfitMonth = 12, StartProfitYear = 2022, EndProfitYear = 2024, ContributionAmount = 0, EarningsAmount = 0, ForfeitureAmount = 0, PaymentAmount = 0, ProfitCode = ProfitCode.Constants.IncomingContributions.Id,  SocialSecurity = 000000000, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryEndpoint, MasterInquiryRequest, ReportResponseBase<MasterInquiryResponseDto>>(request);
        response.Should().NotBeNull();
    }


}
