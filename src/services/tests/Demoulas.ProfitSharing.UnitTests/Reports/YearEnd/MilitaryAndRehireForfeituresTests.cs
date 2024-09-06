using System.Net;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using FluentAssertions;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Services.Reports;
using JetBrains.Annotations;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http.Features;
using YamlDotNet.Core;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(MilitaryAndRehireForfeituresEndpoint))]
public class MilitaryAndRehireForfeituresTests : ApiTestBase<Api.Program>
{
    private readonly MilitaryAndRehireForfeituresEndpoint _endpoint;

    public MilitaryAndRehireForfeituresTests()
    {
        MilitaryAndRehireService mockService = new MilitaryAndRehireService(MockDbContextFactory);
        _endpoint = new MilitaryAndRehireForfeituresEndpoint(mockService);
    }

    
    [Fact(DisplayName = "PS-345: Check for Military (JSON)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var expectedResponse = new ReportResponseBase<MilitaryAndRehireForfeituresResponse>
            {
                ReportName = "REHIRE'S PROFIT SHARING DATA",
                ReportDate = DateTimeOffset.Now,
                Response = new PaginatedResponseDto<MilitaryAndRehireForfeituresResponse>
                {
                    Results = new List<MilitaryAndRehireForfeituresResponse> { setup.ExpectedResponse }
                }
            };

            // Act
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response =
                await ApiClient.GETAsync<MilitaryAndRehireForfeituresEndpoint, PaginationRequestDto, ReportResponseBase<MilitaryAndRehireForfeituresResponse>>(setup.Request);

            // Assert
            response.Result.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
            response.Result.Response.Results.Should().HaveCountGreaterThan(0);
            response.Result.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
        });
    }

    [Fact(DisplayName = "PS-345: Check for Military (CSV)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest_CSV()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            // Act
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response = await DownloadClient.GETAsync<MilitaryAndRehireForfeituresEndpoint, PaginationRequestDto, StreamContent>(setup.Request);
            response.Response.Content.Should().NotBeNull();

            string result = await response.Response.Content.ReadAsStringAsync();
            result.Should().NotBeNullOrEmpty();
        });
    }

    [Fact(DisplayName = "PS-156: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var response =
                await ApiClient.GETAsync<MilitaryAndRehireForfeituresEndpoint, PaginationRequestDto, ReportResponseBase<MilitaryAndRehireForfeituresResponse>>(setup.Request);

            response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        });
    }

    [Fact(DisplayName = "PS-156: Empty Results")]
    public async Task GetResponse_Should_HandleEmptyResults()
    {
        // Arrange
        var request = new PaginationRequestDto { Skip = 0, Take = 10 };
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<MilitaryAndRehireForfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<MilitaryAndRehireForfeituresResponse> { Results = new List<MilitaryAndRehireForfeituresResponse>() }
        };

        // Act
        var response = await _endpoint.GetResponse(request, cancellationToken);

        // Assert
        response.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
    }

    [Fact(DisplayName = "PS-156: Null Results")]
    public async Task GetResponse_Should_HandleNullResults()
    {
        // Arrange
        var request = new PaginationRequestDto { Skip = 0, Take = 10 };
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<MilitaryAndRehireForfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<MilitaryAndRehireForfeituresResponse> { Results = [] }
        };

        // Act
        var response = await _endpoint.GetResponse(request, cancellationToken);

        // Assert
        response.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
    }

    [Fact(DisplayName = "PS-156: Report name is correct")]
    public void ReportFileName_Should_ReturnCorrectValue()
    {
        // Act
        var reportFileName = _endpoint.ReportFileName;

        // Assert
        reportFileName.Should().Be("REHIRE'S PROFIT SHARING DATA");
    }

    private static async Task<(PaginationRequestDto Request, MilitaryAndRehireForfeituresResponse ExpectedResponse)> SetupTestEmployee(ProfitSharingDbContext c)
    {
        // Setup
        MilitaryAndRehireForfeituresResponse example = MilitaryAndRehireForfeituresResponse.ResponseExample();

        var demo = await c.Demographics.FirstAsync();
        demo.EmploymentStatusId = EmploymentStatus.Constants.Active;
        demo.ReHireDate = DateTime.Today.ToDateOnly();
        demo.BadgeNumber = example.BadgeNumber;

        var payProfit = await c.PayProfits.FirstAsync(pp => pp.Ssn == demo.Ssn);
        payProfit.EnrollmentId = Enrollment.Constants.NewVestingPlanHasForfeitureRecords;
        payProfit.CompanyContributionYears = 3;
        payProfit.HoursCurrentYear = 2358;

        var details = await c.ProfitDetails.Where(pd => pd.Ssn == demo.Ssn).ToListAsync();
        foreach (var detail in details)
        {
            detail.Forfeiture = short.MaxValue;
            detail.ProfitYear = 2021;
            detail.Remark = "Test remarks";
            detail.ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id;
        }

        await c.SaveChangesAsync();

        example.Ssn = demo.Ssn.MaskSsn();
        example.FullName = demo.FullName;


        return (new PaginationRequestDto { Skip = 0, Take = 10 }, example);
    }
}
