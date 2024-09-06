using System.Text;
using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client.Reports.YearEnd;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Base;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using System.Net.Http;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;
public class YearEndServiceTests:ApiTestBase<Program>
{
    private readonly YearEndClient _yearEndClient;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly PaginationRequestDto _paginationRequest = new PaginationRequestDto { Skip = 0, Take = byte.MaxValue };


    public YearEndServiceTests( ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _yearEndClient = new YearEndClient(ApiClient, DownloadClient);
    }

    

    [Fact(DisplayName ="PS-147: Check Duplicate SSNs (JSON)")]
    public async Task GetDuplicateSsNsTestJson()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await _yearEndClient.GetDuplicateSsNs(_paginationRequest, CancellationToken.None);
        response.Should().NotBeNull();
        response.Response.Results.Count().Should().Be(0); //Duplicate SSNs aren't allowed in our data model, prohibited by primary key on SSN in the demographics table.
    }

    [Fact(DisplayName = "PS-147: Check Duplicate SSNs (CSV)")]
    public async Task GetDuplicateSsNsTestCsv()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var stream = await _yearEndClient.DownloadDuplicateSsNs(CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }

    [Fact(DisplayName ="PS-150: Payprofit badges w/o Demographics (JSON)")]
    public async Task GetPayProfitBadgesNotInDemographics()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var response = await _yearEndClient.GetPayProfitBadgesNotInDemographics(_paginationRequest, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            byte mismatchedValues = 5;

            await Parallel.ForEachAsync(c.PayProfits.Take(mismatchedValues), async (pp, token) =>
            {
                var demographic = await c.Demographics.FirstAsync(x => x.BadgeNumber == pp.BadgeNumber && x.Ssn == pp.Ssn, cancellationToken: token);

                demographic.BadgeNumber = pp.BadgeNumber + await c.Demographics.CountAsync(token) + 1;
            });

            await c.SaveChangesAsync();

            response = await _yearEndClient.GetPayProfitBadgesNotInDemographics(_paginationRequest, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(mismatchedValues);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            var oneRecord = new PaginationRequestDto { Skip=0, Take=1 };
            response = await _yearEndClient.GetPayProfitBadgesNotInDemographics(oneRecord, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        });
    }

    [Fact(DisplayName = "PS-150: Payprofit badges w/o Demographics (CSV)")]
    public async Task GetPayProfitBadgesNotInDemographicsCsv()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            byte mismatchedValues = 5;

            await c.PayProfits.Take(mismatchedValues).ForEachAsync(async pp =>
            {
                var demographic = await c.Demographics.FirstAsync(x => x.BadgeNumber == pp.BadgeNumber && x.Ssn == pp.Ssn);

                demographic.BadgeNumber = pp.BadgeNumber + await c.Demographics.CountAsync() + 1;
            });

            await c.SaveChangesAsync();

            var stream = await _yearEndClient.DownloadPayProfitBadgesNotInDemographics(CancellationToken.None);
            stream.Should().NotBeNull();

            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
            string result = await reader.ReadToEndAsync();
            result.Should().NotBeNullOrEmpty();

            var lines = result.Split(Environment.NewLine);
            lines.Count().Should().Be(mismatchedValues + 4);


            _testOutputHelper.WriteLine(result);
        });
    }

    [Fact(DisplayName = "PS-151: Demographic badges without payprofit (JSON)")]
    public async Task GetDemographicBadgesWithoutPayProfitTests()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var response = await _yearEndClient.GetDemographicBadgesNotInPayProfit(_paginationRequest, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            byte mismatchedValues = 5;

            foreach (var dem in c.Demographics.Take(mismatchedValues))
            {
                dem.BadgeNumber = dem.BadgeNumber + c.PayProfits.Count() + 1;
            };

            await c.SaveChangesAsync();

            response = await _yearEndClient.GetDemographicBadgesNotInPayProfit(_paginationRequest, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(mismatchedValues);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            var oneRecord = new PaginationRequestDto { Skip = 0, Take = 1 };
            response = await _yearEndClient.GetDemographicBadgesNotInPayProfit(oneRecord, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        });
    }

    [Fact(DisplayName = "PS-151: Demographic badges without payprofit (CSV)")]
    public async Task GetDemographicBadgesWithoutPayProfitTestsCsv()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            
            byte mismatchedValues = 5;

            await c.Demographics.Take(mismatchedValues).ForEachAsync(dem =>
            {
                dem.BadgeNumber = dem.BadgeNumber + c.PayProfits.Count() + 1;
            });

            await c.SaveChangesAsync();

            var stream = await _yearEndClient.DownloadDemographicBadgesNotInPayProfit(CancellationToken.None);
            stream.Should().NotBeNull();

            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
            string result = await reader.ReadToEndAsync();
            result.Should().NotBeNullOrEmpty();

            var lines = result.Split(Environment.NewLine);
            lines.Count().Should().Be(mismatchedValues + 4);

            _testOutputHelper.WriteLine(result);
        });
    }

    [Fact(DisplayName ="PS-153: Names without commas (JSON)")]
    public async Task GetNamesWithoutCommas()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var request = new PaginationRequestDto() { Skip = 0, Take = 1000 };
            var response = await _yearEndClient.GetNamesMissingComma(request, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Count().Should().Be(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            byte disruptedNameCount = 10;
            foreach (var dem in ctx.Demographics.Take(disruptedNameCount))
            {
                dem.FullName = dem.FullName?.Replace(", ", " ");
            }

            await ctx.SaveChangesAsync();

            response = await _yearEndClient.GetNamesMissingComma(request, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Count().Should().Be(disruptedNameCount);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            var oneRecord = new PaginationRequestDto { Skip = 0, Take = 1 };
            response = await _yearEndClient.GetNamesMissingComma(oneRecord, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        });
    }

    [Fact(DisplayName = "PS-153: Names without commas (CSV)")]
    public async Task GetNamesWithoutCommasCsv()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            byte disruptedNameCount = 10;
            await ctx.Demographics.Take(disruptedNameCount).ForEachAsync(dem =>
            {
                dem.FullName = dem.FullName?.Replace(", ", " ");
            });

            await ctx.SaveChangesAsync();

            var stream = await _yearEndClient.DownloadNamesMissingComma(CancellationToken.None);
            stream.Should().NotBeNull();

            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
            string result = await reader.ReadToEndAsync();
            result.Should().NotBeNullOrEmpty();

            var lines = result.Split(Environment.NewLine);
            lines.Count().Should().Be(disruptedNameCount + 4);

            _testOutputHelper.WriteLine(result);
        });
    }

    [Fact(DisplayName = "PS-145 : Negative ETVA for SSNs on PayProfit (JSON)")]
    public async Task GetNegativeEtvaReportJson()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        byte negativeValues = 5;
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            await c.PayProfits.Take(negativeValues).ForEachAsync(pp =>
            {
                pp.EarningsEtvaValue *= -1;
            });


            await c.SaveChangesAsync();
        });


        var response = await _yearEndClient.GetNegativeETVAForSSNsOnPayProfitResponse(_paginationRequest, CancellationToken.None);

        response.Should().NotBeNull();
        response.ReportName.Should().BeEquivalentTo("Negative ETVA for SSNs on PayProfit");
        response.Response.Results.Should().HaveCount(negativeValues);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        var oneRecord = new PaginationRequestDto { Skip = 0, Take = 1 };
        response = await _yearEndClient.GetNegativeETVAForSSNsOnPayProfitResponse(oneRecord, CancellationToken.None);
        response.Should().NotBeNull();
        response.Response.Results.Should().HaveCount(1);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-145 : Negative ETVA for SSNs on PayProfit (CSV)")]
    public async Task GetNegativeEtvaReportCsv()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var stream = await _yearEndClient.DownloadNegativeETVAForSSNsOnPayProfitResponse(CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }

    [Fact(DisplayName = "PS-149 : Mismatched Ssns Payprofit and Demographics On Same Badge (JSON)")]
    public async Task GetMismatchedSsnsPayprofitAndDemographicsOnSameBadgeJson()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await _yearEndClient.GetMismatchedSsnsPayprofitAndDemographicsOnSameBadge(_paginationRequest, CancellationToken.None);

        response.Should().NotBeNull();
        response.ReportName.Should().BeEquivalentTo("MISMATCHED SSNs PAYPROFIT AND DEMO ON SAME BADGE");

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-149 : Mismatched Ssns Payprofit and Demographics On Same Badge (CSV)")]
    public async Task GetMismatchedSsnsPayprofitAndDemographicsOnSameBadgeCsv()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var stream = await _yearEndClient.DownloadMismatchedSsnsPayprofitAndDemographicsOnSameBadge(CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }

    [Fact(DisplayName = "PS-152 : Duplicate names and Birthdays (JSON)")]
    public async Task GetDuplicateNamesAndBirthdays()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new PaginationRequestDto { Take = 1000, Skip = 0 };
        var response = await _yearEndClient.GetDuplicateNamesAndBirthdays(request, CancellationToken.None);
        response.Should().NotBeNull();
        response.Response.Results.Count().Should().Be(0);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        byte duplicateRows = 5;
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var modelDemographic = await c.Demographics.FirstAsync();

            foreach (var dem in c.Demographics.Skip(1).Take(duplicateRows))
            {
                dem.DateOfBirth = modelDemographic.DateOfBirth;
                dem.FirstName = modelDemographic.FirstName;
                dem.LastName = modelDemographic.LastName;
                dem.FullName = modelDemographic.FullName;
            };

            await c.SaveChangesAsync();
        });

        response = await _yearEndClient.GetDuplicateNamesAndBirthdays(request, CancellationToken.None);
        response.Should().NotBeNull();
        response.Response.Results.Count().Should().Be(duplicateRows + 1);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        var oneRecord = new PaginationRequestDto { Skip = 0, Take = 1 };
        response = await _yearEndClient.GetDuplicateNamesAndBirthdays(oneRecord, CancellationToken.None);
        response.Should().NotBeNull();
        response.Response.Results.Should().HaveCount(1);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-152 : Duplicate names and Birthdays (CSV)")]
    public async Task GetDuplicateNamesAndBirthdaysCsv()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        byte duplicateRows = 5;
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var modelDemographic = await c.Demographics.FirstAsync();

            await c.Demographics.Skip(1).Take(duplicateRows).ForEachAsync(dem =>
            {
                dem.DateOfBirth = modelDemographic.DateOfBirth;
                dem.FirstName = modelDemographic.FirstName;
                dem.LastName = modelDemographic.LastName;
                dem.FullName = modelDemographic.FullName;
            });

            await c.SaveChangesAsync();
        });

        var stream = await _yearEndClient.DownloadDuplicateNamesAndBirthdays(CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        var lines = result.Split(Environment.NewLine);
        lines.Count().Should().Be(duplicateRows + 4 + 1); //Includes initial row that was used as the template to create duplicates

        _testOutputHelper.WriteLine(result);

    }

    [Fact(DisplayName = "PS-148 : Payroll Duplicate Ssns On Payprofit (JSON)")]
    public async Task GetPayrollDuplicateSsnsOnPayprofitJson()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await _yearEndClient.GetPayrollDuplicateSsnsOnPayprofit(_paginationRequest, CancellationToken.None);

        response.Should().NotBeNull();
        response.ReportName.Should().BeEquivalentTo("PAYROLL DUPLICATE SSNs ON PAYPROFIT");

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-148 : Payroll Duplicate Ssns On Payprofit (CSV)")]
    public async Task GetPayrollDuplicateSsnsOnPayprofitCsv()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var stream = await _yearEndClient.DownloadPayrollDuplicateSsnsOnPayprofit(CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }

    [Fact(DisplayName = "YearEndService auth check")]
    public async Task YearEndServiceAuthCheck()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.HARDSHIPADMINISTRATOR);
        await Assert.ThrowsAsync<HttpRequestException>(async () => { _ = await _yearEndClient.GetPayrollDuplicateSsnsOnPayprofit(_paginationRequest, CancellationToken.None); });
    }
}
