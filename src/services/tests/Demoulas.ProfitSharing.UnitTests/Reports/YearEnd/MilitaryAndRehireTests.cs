using System.Net;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Military;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(MilitaryAndRehireEndpoint))]
public class MilitaryAndRehireTests : ApiTestBase<Program>
{
    private readonly MilitaryAndRehireEndpoint _endpoint;

    public MilitaryAndRehireTests()
    {
        IMilitaryAndRehireService mockService = ServiceProvider?.GetRequiredService<IMilitaryAndRehireService>()!;
        _endpoint = new MilitaryAndRehireEndpoint(mockService);
    }


    [Fact(DisplayName = "PS-156: Check for Military (JSON)")]
    public Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var expectedResponse = new ReportResponseBase<MilitaryAndRehireReportResponse>
            {
                ReportName = "EMPLOYEES ON MILITARY LEAVE",
                ReportDate = DateTimeOffset.Now,
                Response = new PaginatedResponseDto<MilitaryAndRehireReportResponse> { Results = new List<MilitaryAndRehireReportResponse> { setup.ExpectedResponse } }
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
    public Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest_CSV()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            // Act
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response = await DownloadClient.GETAsync<MilitaryAndRehireEndpoint, PaginationRequestDto, StreamContent>(setup.Request);
            response.Response.Content.Should().NotBeNull();

            string result = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
            result.Should().NotBeNullOrEmpty();
        });
    }

    [Fact(DisplayName = "PS-156: Check to ensure unauthorized")]
    public Task Unauthorized()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
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
            ReportName = "EMPLOYEES ON MILITARY LEAVE", ReportDate = DateTimeOffset.Now, Response = new PaginatedResponseDto<MilitaryAndRehireReportResponse> { Results = [] }
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

        var demo = await c.Demographics.Include(demographic => demographic.ContactInfo).FirstAsync(CancellationToken.None);
        demo.TerminationCodeId = TerminationCode.Constants.Military;
        demo.EmploymentStatusId = EmploymentStatus.Constants.Inactive;

        demo.DepartmentId = example.DepartmentId;
        demo.EmployeeId = example.BadgeNumber;
        demo.DateOfBirth = example.DateOfBirth;
        demo.TerminationDate = example.TerminationDate;
        await c.SaveChangesAsync(CancellationToken.None);

        example.Ssn = demo.Ssn.MaskSsn();
        example.FullName = demo.ContactInfo.FullName;


        return (new PaginationRequestDto { Skip = 0, Take = 10 }, example);
    }
}
