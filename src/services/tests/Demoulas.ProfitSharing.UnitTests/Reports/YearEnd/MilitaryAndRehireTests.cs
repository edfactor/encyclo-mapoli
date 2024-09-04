using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Moq;
using FluentAssertions;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Services.Reports;
using JetBrains.Annotations;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(MilitaryAndRehireEndpoint))]
public class MilitaryAndRehireTests : ApiTestBase<Api.Program>
{
    private readonly MilitaryAndRehireEndpoint _endpoint;

    public MilitaryAndRehireTests()
    {
        MilitaryAndRehireService mockService = new MilitaryAndRehireService(MockDbContextFactory);
        _endpoint = new MilitaryAndRehireEndpoint(mockService);
    }

    [Fact(DisplayName = "PS-156: Check for Military (JSON)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            // Setup
            var example = MilitaryAndRehireReportResponse.ResponseExample();

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
            

            // Arrange
            var request = new PaginationRequestDto { Skip = 0, Take = 10 };
            var cancellationToken = CancellationToken.None;
            var expectedResponse = new ReportResponseBase<MilitaryAndRehireReportResponse>
            {
                ReportName = "EMPLOYEES ON MILITARY LEAVE",
                ReportDate = DateTimeOffset.Now,
                Response = new PaginatedResponseDto<MilitaryAndRehireReportResponse>
                {
                    Results = new List<MilitaryAndRehireReportResponse> { example }
                }
            };

            // Act
            var response = await _endpoint.GetResponse(request, cancellationToken);

            // Assert
            response.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
            response.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
        });
    }

    [Fact]
    public async Task GetResponse_Should_HandleEmptyResults()
    {
        // Arrange
        var request = new PaginationRequestDto { Skip = 0, Take = 10 };
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<MilitaryAndRehireReportResponse>
        {
            ReportName = "EMPLOYEES ON MILITARY LEAVE",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<MilitaryAndRehireReportResponse>
            {
                Results = new List<MilitaryAndRehireReportResponse>()
            }
        };

        // Act
        var response = await _endpoint.GetResponse(request, cancellationToken);

        // Assert
        response.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
    }

    [Fact]
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

    [Fact]
    public void ReportFileName_Should_ReturnCorrectValue()
    {
        // Act
        var reportFileName = _endpoint.ReportFileName;

        // Assert
        reportFileName.Should().Be("EMPLOYEES ON MILITARY LEAVE");
    }
}
