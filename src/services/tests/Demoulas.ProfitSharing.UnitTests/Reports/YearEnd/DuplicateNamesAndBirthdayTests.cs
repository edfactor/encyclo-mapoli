using System.Text.Json;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit.v3;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class DuplicateNamesAndBirthdayTests : ApiTestBase<Program>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly DuplicateNamesAndBirthdaysRequest _defaultRequest = new() { ProfitYear = 2023, Skip = 0, Take = byte.MaxValue };
    private const byte DuplicateRowCount = 5;

    public DuplicateNamesAndBirthdayTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);
    }


    [Fact(DisplayName = "PS-152 : Detects exact name and birthday matches")]
    public async Task GetDuplicateNamesAndBirthdays_ExactMatches_ReturnsResults()
    {
        var request = new DuplicateNamesAndBirthdaysRequest { ProfitYear = _defaultRequest.ProfitYear, Take = 1000, Skip = 0 };
        await CreateExactDuplicateRecords();

        var response = await ApiClient.GETAsync<DuplicateNamesAndBirthdaysEndpoint, DuplicateNamesAndBirthdaysRequest, ReportResponseBase<DuplicateNamesAndBirthdaysResponse>>(request);

        if (response.Result is null)
        {
            _testOutputHelper.WriteLine($"HTTP Response Status: {response.Response.StatusCode}");
            _testOutputHelper.WriteLine($"HTTP Response Content: {await response.Response.Content.ReadAsStringAsync()}");
        }

        response.Result.ShouldNotBeNull();
        response.Result.Response.Results.Count().ShouldBeGreaterThanOrEqualTo(DuplicateRowCount);
        LogResponse(response.Result);
    }

    [Fact(DisplayName = "PS-152 : Detects similar names with same birthdays")]
    public async Task GetDuplicateNamesAndBirthdays_SimilarNames_ReturnsResults()
    {
        var request = new DuplicateNamesAndBirthdaysRequest { ProfitYear = _defaultRequest.ProfitYear, Take = 1000, Skip = 0 };
        await CreateSimilarNameRecords();

        var response = await ApiClient.GETAsync<DuplicateNamesAndBirthdaysEndpoint, DuplicateNamesAndBirthdaysRequest, ReportResponseBase<DuplicateNamesAndBirthdaysResponse>>(request);

        response.Result.ShouldNotBeNull();
        response.Result.Response.Results.Count().ShouldBeGreaterThanOrEqualTo(2);
        LogResponse(response.Result);
    }

    [Fact(DisplayName = "PS-152 : Detects matches with close birth dates")]
    public async Task GetDuplicateNamesAndBirthdays_CloseBirthDates_ReturnsResults()
    {
        var request = new DuplicateNamesAndBirthdaysRequest { ProfitYear = _defaultRequest.ProfitYear, Take = 1000, Skip = 0 };
        await CreateCloseBirthDateRecords();

        var response = await ApiClient.GETAsync<DuplicateNamesAndBirthdaysEndpoint, DuplicateNamesAndBirthdaysRequest, ReportResponseBase<DuplicateNamesAndBirthdaysResponse>>(request);

        response.Result.ShouldNotBeNull();
        response.Result.Response.Results.Count().ShouldBeGreaterThanOrEqualTo(2);
        LogResponse(response.Result);
    }

    [Fact(DisplayName = "PS-152 : Pagination returns single result")]
    public async Task GetDuplicateNamesAndBirthdays_WithPagination_ReturnsSingleResult()
    {
        await CreateExactDuplicateRecords();
        var request = new DuplicateNamesAndBirthdaysRequest { ProfitYear = _defaultRequest.ProfitYear, Skip = 0, Take = 1 };

        var response = await ApiClient.GETAsync<DuplicateNamesAndBirthdaysEndpoint, DuplicateNamesAndBirthdaysRequest, ReportResponseBase<DuplicateNamesAndBirthdaysResponse>>(request);

        response.Result.ShouldNotBeNull();
        response.Result.Response.Results.Count().ShouldBe(1);
        LogResponse(response.Result);
    }

    [Fact(DisplayName = "PS-152 : CSV export contains expected data")]
    public Task GetDuplicateNamesAndBirthdays_CsvExport_ContainsExpectedData()
    {
        DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        return MockDbContextFactory.UseWritableContext(async context =>
        {
            var modelDemographic = await context.Demographics
                .Include(demographic => demographic.ContactInfo)
                .FirstAsync(CancellationToken.None);

            foreach (var demographic in context.Demographics.Take(DuplicateRowCount))
            {
                demographic.DateOfBirth = modelDemographic.DateOfBirth;
                demographic.ContactInfo.FirstName = modelDemographic.ContactInfo.FirstName;
                demographic.ContactInfo.LastName = modelDemographic.ContactInfo.LastName;
                demographic.ContactInfo.FullName = modelDemographic.ContactInfo.FullName;

                if (demographic.PayProfits.All(x => x.ProfitYear != _defaultRequest.ProfitYear))
                {
                    demographic.PayProfits[0].ProfitYear = _defaultRequest.ProfitYear;
                }
            }

            await context.SaveChangesAsync(CancellationToken.None);

            var response = await DownloadClient.GETAsync<DuplicateNamesAndBirthdaysEndpoint, DuplicateNamesAndBirthdaysRequest, DuplicateNamesAndBirthdaysResponse>(_defaultRequest);

            string content = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
            content.ShouldNotBeNullOrEmpty();

            var lines = content.Split(Environment.NewLine);
            lines.Count().ShouldBeGreaterThanOrEqualTo(DuplicateRowCount + 4);
            _testOutputHelper.WriteLine(content);
        });
    }

    private Task CreateExactDuplicateRecords()
    {
        return MockDbContextFactory.UseWritableContext(async context =>
        {
            var modelDemographic = await context.Demographics
                .Include(demographic => demographic.ContactInfo)
                .FirstAsync(CancellationToken.None);

            foreach (var demographic in context.Demographics.Take(DuplicateRowCount))
            {
                demographic.DateOfBirth = modelDemographic.DateOfBirth;
                demographic.ContactInfo.FirstName = modelDemographic.ContactInfo.FirstName;
                demographic.ContactInfo.LastName = modelDemographic.ContactInfo.LastName;
                demographic.ContactInfo.FullName = modelDemographic.ContactInfo.FullName;

                if (demographic.PayProfits.All(x => x.ProfitYear != _defaultRequest.ProfitYear))
                {
                    demographic.PayProfits[0].ProfitYear = _defaultRequest.ProfitYear;
                }
            }

            await context.SaveChangesAsync(CancellationToken.None);
        });
    }

    private Task CreateSimilarNameRecords()
    {
        return MockDbContextFactory.UseWritableContext(async context =>
        {
            var modelDemographic = await context.Demographics
                .Include(demographic => demographic.ContactInfo)
                .FirstAsync(CancellationToken.None);

            var demographic = await context.Demographics
                .Include(d => d.ContactInfo)
                .Skip(1)
                .FirstAsync(CancellationToken.None);

            demographic.DateOfBirth = modelDemographic.DateOfBirth;
            // Introduce a small typo in the name
            demographic.ContactInfo.FirstName = modelDemographic.ContactInfo.FirstName + "e";
            demographic.ContactInfo.LastName = modelDemographic.ContactInfo.LastName;
            demographic.ContactInfo.FullName = demographic.ContactInfo.FirstName + " " + demographic.ContactInfo.LastName;

            await context.SaveChangesAsync(CancellationToken.None);
        });
    }

    private Task CreateCloseBirthDateRecords()
    {
        return MockDbContextFactory.UseWritableContext(async context =>
        {
            var modelDemographic = await context.Demographics
                .Include(demographic => demographic.ContactInfo)
                .FirstAsync(CancellationToken.None);

            var demographic = await context.Demographics
                .Include(d => d.ContactInfo)
                .Skip(1)
                .FirstAsync(CancellationToken.None);

            // Set birth date 2 days apart
            demographic.DateOfBirth = modelDemographic.DateOfBirth.AddDays(2);
            demographic.ContactInfo.FirstName = modelDemographic.ContactInfo.FirstName;
            demographic.ContactInfo.LastName = modelDemographic.ContactInfo.LastName;
            demographic.ContactInfo.FullName = modelDemographic.ContactInfo.FullName;

            await context.SaveChangesAsync(CancellationToken.None);
        });
    }

    private void LogResponse<T>(ReportResponseBase<T> response) where T : class
    {
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }
}
