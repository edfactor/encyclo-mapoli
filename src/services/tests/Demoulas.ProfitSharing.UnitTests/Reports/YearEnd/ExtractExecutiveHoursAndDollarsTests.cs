using System.Net;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ExecutiveHoursAndDollars;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Reports;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class ExecutiveHoursAndDollarsTests : ApiTestBase<Program>
{
    private const int ProfitShareTestYear = 2075; // used to avoid pre-canned data.

    private static readonly ExecutiveHoursAndDollarsRequest _request = new() { ProfitYear = ProfitShareTestYear, Skip = 0, Take = 10 };

    private static readonly ExecutiveHoursAndDollarsResponse _example =
        ExecutiveHoursAndDollarsResponse.ResponseExample();

    private readonly ExecutiveHoursAndDollarsEndpoint _endpoint;
    private readonly string _expectedReportName = $"Executive Hours and Dollars for Year {ProfitShareTestYear}";


    public ExecutiveHoursAndDollarsTests()
    {
        ExecutiveHoursAndDollarsService mockService = new(MockDbContextFactory);
        _endpoint = new ExecutiveHoursAndDollarsEndpoint(mockService);
    }


    [Fact(DisplayName = "PS-360: Check to ensure unauthorized")]
    public Task Unauthorized()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            await SetupTestEmployee_With_HoursAndDollars(c);

            TestResult<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> response =
                await ApiClient
                    .GETAsync<ExecutiveHoursAndDollarsEndpoint, ExecutiveHoursAndDollarsRequest,
                        ReportResponseBase<ExecutiveHoursAndDollarsResponse>>(_request);

            response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        });
    }

    [Fact(DisplayName = "PS-360: return all employees (no exec dollars/hours constraint)")]
    public Task get_all()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            await SetupTestEmployee_Without_HoursAndDollars(c);
            ReportResponseBase<ExecutiveHoursAndDollarsResponse> expectedResponse = StockResponse();
            expectedResponse.Response.Results.First().HoursExecutive = 0;
            expectedResponse.Response.Results.First().IncomeExecutive = 0;

            // Act
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            TestResult<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> response =
                await ApiClient
                    .GETAsync<ExecutiveHoursAndDollarsEndpoint, ExecutiveHoursAndDollarsRequest,
                        ReportResponseBase<ExecutiveHoursAndDollarsResponse>>(_request);

            // Assert
            response.Result.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
            response.Result.Response.Results.Count().Should().Be(1);
            response.Result.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
        });
    }

    [Fact(DisplayName = "PS-360: return only employees with dollars/hours -> expect none")]
    public Task get_nothing()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            await SetupTestEmployee_Without_HoursAndDollars(c);
            ExecutiveHoursAndDollarsRequest request = new() { ProfitYear = ProfitShareTestYear, Skip = 0, Take = 10, HasExecutiveHoursAndDollars = true };

            // Act
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            TestResult<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> response =
                await ApiClient
                    .GETAsync<ExecutiveHoursAndDollarsEndpoint, ExecutiveHoursAndDollarsRequest,
                        ReportResponseBase<ExecutiveHoursAndDollarsResponse>>(request);

            // Assert

            response.Result.ReportName.Should().BeEquivalentTo(_expectedReportName);
            response.Result.Response.Results.Count().Should().Be(0);
        });
    }

    [Fact(DisplayName = "PS-360: return all employees (no dollars/hours)")]
    public Task get_one_row_json()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            await SetupTestEmployee_With_HoursAndDollars(c);
            ReportResponseBase<ExecutiveHoursAndDollarsResponse> expectedResponse = StockResponse();

            // Act
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            TestResult<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> response =
                await ApiClient
                    .GETAsync<ExecutiveHoursAndDollarsEndpoint, ExecutiveHoursAndDollarsRequest,
                        ReportResponseBase<ExecutiveHoursAndDollarsResponse>>(_request);

            // Assert
            response.Result.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
            response.Result.Response.Results.Count().Should().Be(1);
            response.Result.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
        });
    }

    [Fact(DisplayName = "PS-360: Executive Hours and Dollars (CSV)")]
    public Task get_one_row_csv()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            await SetupTestEmployee_With_HoursAndDollars(c);
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            ExecutiveHoursAndDollarsRequest request = new() { ProfitYear = ProfitShareTestYear, HasExecutiveHoursAndDollars = true };

            // Act
            TestResult<StreamContent> response = await DownloadClient
                .GETAsync<ExecutiveHoursAndDollarsEndpoint, ExecutiveHoursAndDollarsRequest, StreamContent>(request);

            // Assert
            response.Response.Content.Should().NotBeNull();

            // Verify CSV file
            string csvData = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
            string[] lines = csvData.Split(["\r\n", "\n"], StringSplitOptions.None);
            const string testName = "John, Null E";
            const string testSSN = "XXX-XX-8825";
            lines[1].Should().Be(_expectedReportName);
            lines[2].Should().Be("BADGE,NAME,STR,EXEC HRS,EXEC DOLS,ORA HRS CUR,ORA DOLS CUR,FREQ,STATUS,SSN");
            lines[3].Should().Be("1," + testName + ",2,3,4,5,6,2,a," + testSSN);
            lines[4].Should().Be("");
        });
    }


    [Fact(DisplayName = "PS-360: Empty Results")]
    public async Task GetResponse_Should_HandleEmptyResults()
    {
        // Arrange
        ReportResponseBase<ExecutiveHoursAndDollarsResponse> expectedResponse = StockResponse();
        expectedResponse.Response = new PaginatedResponseDto<ExecutiveHoursAndDollarsResponse>();

        // Act
        ReportResponseBase<ExecutiveHoursAndDollarsResponse> response =
            await _endpoint.GetResponse(_request, CancellationToken.None);

        // Assert
        response.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
    }

    [Fact(DisplayName = "PS-360: Report name is correct")]
    public void ReportFileName_Should_ReturnCorrectValue()
    {
        string reportFileName = _endpoint.ReportFileName;
        reportFileName.Should().Be("Executive Hours and Dollars");
    }

    [Fact]
    public Task GetResponse_Should_search_by_fullname()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            PayProfit payProfit = await SetupTestEmployee_With_HoursAndDollars(c);
            ExecutiveHoursAndDollarsRequest request = new()
            {
                FullNameContains = payProfit.Demographic!.ContactInfo.FullName, ProfitYear = ProfitShareTestYear, Skip = 0, Take = 10
            };
            ReportResponseBase<ExecutiveHoursAndDollarsResponse> expectedResponse = StockResponse();
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            TestResult<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> response =
                await ApiClient
                    .GETAsync<ExecutiveHoursAndDollarsEndpoint, ExecutiveHoursAndDollarsRequest,
                        ReportResponseBase<ExecutiveHoursAndDollarsResponse>>(request);

            // Assert
            response.Result.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
            response.Result.Response.Results.Count().Should().Be(1);
            response.Result.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
        });
    }

    [Fact]
    public Task GetResponse_Should_search_by_fullname_not_found()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            PayProfit payProfit = await SetupTestEmployee_With_HoursAndDollars(c);
            ExecutiveHoursAndDollarsRequest request = new()
            {
                FullNameContains = $"ZZ{payProfit.Demographic!.ContactInfo.FullName}", ProfitYear = ProfitShareTestYear, Skip = 0, Take = 10
            };
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            TestResult<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> response =
                await ApiClient
                    .GETAsync<ExecutiveHoursAndDollarsEndpoint, ExecutiveHoursAndDollarsRequest,
                        ReportResponseBase<ExecutiveHoursAndDollarsResponse>>(request);

            // Assert
            response.Result.Response.Results.Count().Should().Be(0);
        });
    }

    private ReportResponseBase<ExecutiveHoursAndDollarsResponse> StockResponse()
    {
        return new ReportResponseBase<ExecutiveHoursAndDollarsResponse>
        {
            ReportName = _expectedReportName,
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<ExecutiveHoursAndDollarsResponse> { Results = [ExecutiveHoursAndDollarsResponse.ResponseExample()] }
        };
    }

    private static async Task<PayProfit> SetupTestEmployee_With_HoursAndDollars(ProfitSharingDbContext c)
    {
        PayProfit pp = await c.PayProfits
            .Include(payProfit => payProfit.Demographic!)
            .ThenInclude(demographic => demographic.ContactInfo)
            .Include(p => p.Demographic != null)
            .FirstAsync(CancellationToken.None);
        Demographic demo = pp.Demographic!;

        demo.BadgeNumber = _example.BadgeNumber;
        demo.ContactInfo.FullName = _example.FullName;
        demo.StoreNumber = _example.StoreNumber;
        pp.IncomeExecutive = _example.IncomeExecutive;
        pp.HoursExecutive = _example.HoursExecutive;
        pp.CurrentIncomeYear = _example.CurrentIncomeYear;
        pp.CurrentHoursYear = _example.CurrentHoursYear;
        demo.PayFrequencyId = _example.PayFrequencyId;
        demo.EmploymentStatusId = _example.EmploymentStatusId;
        pp.ProfitYear = ProfitShareTestYear;
        pp.Demographic = demo;

        await c.SaveChangesAsync(CancellationToken.None);
        return pp;
    }

    private static async Task SetupTestEmployee_Without_HoursAndDollars(ProfitSharingDbContext c)
    {
        PayProfit pp = await c.PayProfits
            .Include(payProfit => payProfit.Demographic!)
            .ThenInclude(demographic => demographic.ContactInfo)
            .Include(p => p.Demographic != null)
            .FirstAsync(CancellationToken.None);
        Demographic demo = pp.Demographic!;

        demo.BadgeNumber = _example.BadgeNumber;
        demo.ContactInfo.FullName = _example.FullName;
        demo.StoreNumber = _example.StoreNumber;
        pp.IncomeExecutive = 0m;
        pp.HoursExecutive = 0;
        pp.CurrentIncomeYear = _example.CurrentIncomeYear;
        pp.CurrentHoursYear = _example.CurrentHoursYear;
        demo.PayFrequencyId = _example.PayFrequencyId;
        demo.EmploymentStatusId = _example.EmploymentStatusId;
        pp.ProfitYear = ProfitShareTestYear;
        pp.Demographic = demo;

        await c.SaveChangesAsync(CancellationToken.None);
    }
}
