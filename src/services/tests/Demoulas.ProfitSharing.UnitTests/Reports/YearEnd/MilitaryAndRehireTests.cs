using System.Net;
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
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Military;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(MilitaryAndRehireEndpoint))]
public class MilitaryAndRehireTests : ApiTestBase<Api.Program>
{
    private readonly MilitaryAndRehireEndpoint _endpoint;

    public MilitaryAndRehireTests()
    {
        MilitaryAndRehireService mockService = new MilitaryAndRehireService(MockDbContextFactory, new CalendarService(MockDbContextFactory));
        _endpoint = new MilitaryAndRehireEndpoint(mockService);
    }

    
    [Fact(DisplayName = "PS-156: Check for Military (JSON)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var expectedResponse = new ReportResponseBase<MilitaryAndRehireReportResponse>
            {
                ReportName = "EMPLOYEES ON MILITARY LEAVE",
                ReportDate = DateTimeOffset.Now,
                Response = new PaginatedResponseDto<MilitaryAndRehireReportResponse>
                {
                    Results = new List<MilitaryAndRehireReportResponse> { setup.ExpectedResponse }
                }
            };

            // Act
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response =
                await ApiClient.GETAsync<MilitaryAndRehireEndpoint, PaginationRequestDto, ReportResponseBase<MilitaryAndRehireReportResponse>>(setup.Request);

            // Assert
            response.Result.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
            response.Result.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
        });
    }

    [Fact(DisplayName = "PS-156: Check for Military (CSV)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest_CSV()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            // Act
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response = await DownloadClient.GETAsync<MilitaryAndRehireEndpoint, PaginationRequestDto, StreamContent>(setup.Request);
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
                await ApiClient.GETAsync<MilitaryAndRehireEndpoint, PaginationRequestDto, ReportResponseBase<MilitaryAndRehireReportResponse>>(setup.Request);

            response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        });
    }

    [Fact(DisplayName = "PS-156: Empty Results")]
    public async Task GetResponse_Should_HandleEmptyResults()
    {
        // Arrange
        var request = new PaginationRequestDto { Skip = 0, Take = 10 };
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<MilitaryAndRehireReportResponse>
        {
            ReportName = "EMPLOYEES ON MILITARY LEAVE",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<MilitaryAndRehireReportResponse> { Results = new List<MilitaryAndRehireReportResponse>() }
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
        var expectedResponse = new ReportResponseBase<MilitaryAndRehireReportResponse>
        {
            ReportName = "EMPLOYEES ON MILITARY LEAVE",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<MilitaryAndRehireReportResponse> { Results = [] }
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
        reportFileName.Should().Be("EMPLOYEES ON MILITARY LEAVE");
    }

    private static async Task<(PaginationRequestDto Request, MilitaryAndRehireReportResponse ExpectedResponse)> SetupTestEmployee(ProfitSharingDbContext c)
    {
        // Setup
        MilitaryAndRehireReportResponse example = MilitaryAndRehireReportResponse.ResponseExample();

        var demo = await c.Demographics.FirstAsync();
        demo.TerminationCodeId = TerminationCode.Constants.Military;
        demo.EmploymentStatusId = EmploymentStatus.Constants.Inactive;

        demo.DepartmentId = example.DepartmentId;
        demo.BadgeNumber = example.BadgeNumber;
        demo.DateOfBirth = example.DateOfBirth;
        demo.TerminationDate = example.TerminationDate;
        await c.SaveChangesAsync();

        example.Ssn = demo.Ssn.MaskSsn();
        example.FullName = demo.FullName;


        return (new PaginationRequestDto { Skip = 0, Take = 10 }, example);
    }
}
