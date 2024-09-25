using System.Text;
using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client.Reports.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Base;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using IdGen;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;
public class CleanupReportServiceTests:ApiTestBase<Program>
{
    private readonly CleanupReportClient _cleanupReportClient;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ProfitYearRequest _paginationRequest = new ProfitYearRequest {ProfitYear = 2023, Skip = 0, Take = byte.MaxValue };
    private readonly IdGenerator _generator;


    public CleanupReportServiceTests( ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _cleanupReportClient = new CleanupReportClient(ApiClient, DownloadClient);
        _generator = new IdGenerator(0);
    }

    

    [Fact(DisplayName ="PS-147: Check Duplicate SSNs (JSON)")]
    public async Task GetDuplicateSsNsTestJson()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await _cleanupReportClient.GetDuplicateSsNs(_paginationRequest, CancellationToken.None);
        response.Should().NotBeNull();
        response.Response.Results.Count().Should().Be(0); //Duplicate SSNs aren't allowed in our data model, prohibited by primary key on SSN in the demographics table.
    }

    [Fact(DisplayName = "PS-147: Check Duplicate SSNs (CSV)")]
    public async Task GetDuplicateSsNsTestCsv()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var stream = await _cleanupReportClient.DownloadDuplicateSsNs(_paginationRequest.ProfitYear, CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }

    [Fact(DisplayName ="PS-150: Payprofit badges w/o Demographics (JSON)")]
    public async Task GetPayProfitBadgesNotInDemographics()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var response = await _cleanupReportClient.GetPayProfitBadgesNotInDemographics(_paginationRequest, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            byte mismatchedValues = 5;

            await Parallel.ForEachAsync(c.PayProfits.Take(mismatchedValues), async (pp, token) =>
            {
                var demographic = await c.Demographics.FirstAsync(x => x.OracleHcmId == pp.OracleHcmId, cancellationToken: token);

                demographic.BadgeNumber += await c.Demographics.CountAsync(token) + 1;
            });

            await c.SaveChangesAsync();

            response = await _cleanupReportClient.GetPayProfitBadgesNotInDemographics(_paginationRequest, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(mismatchedValues);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            var oneRecord = new PaginationRequestDto { Skip=0, Take=1 };
            response = await _cleanupReportClient.GetPayProfitBadgesNotInDemographics(oneRecord, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        });
    }

    [Fact(DisplayName = "PS-150: Payprofit badges w/o Demographics (CSV)")]
    public async Task GetPayProfitBadgesNotInDemographicsCsv()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            byte mismatchedValues = 5;

            await c.PayProfits.Take(mismatchedValues).ForEachAsync(async pp =>
            {
                var demographic = await c.Demographics.FirstAsync(x => x.OracleHcmId == pp.OracleHcmId);

                demographic.BadgeNumber += await c.Demographics.CountAsync() + 1;
            });

            await c.SaveChangesAsync();

            var stream = await _cleanupReportClient.DownloadPayProfitBadgesNotInDemographics(CancellationToken.None);
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
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var response = await _cleanupReportClient.GetDemographicBadgesNotInPayProfit(_paginationRequest, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            byte mismatchedValues = 5;
            
            foreach (var dem in c.Demographics.Take(mismatchedValues))
            {
                long lastSevenDigits = _generator.CreateId() % 10_000_000;
                dem.OracleHcmId += (int)lastSevenDigits;
            }

            await c.SaveChangesAsync();

            response = await _cleanupReportClient.GetDemographicBadgesNotInPayProfit(_paginationRequest, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(mismatchedValues);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            var oneRecord = new PaginationRequestDto { Skip = 0, Take = 1 };
            response = await _cleanupReportClient.GetDemographicBadgesNotInPayProfit(oneRecord, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        });
    }

    [Fact(DisplayName = "PS-151: Demographic badges without payprofit (CSV)")]
    public async Task GetDemographicBadgesWithoutPayProfitTestsCsv()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async c =>
        {

            byte mismatchedValues = 5;

            foreach (var dem in c.Demographics.Take(mismatchedValues))
            {
                long lastSevenDigits = _generator.CreateId() % 10_000_000;
                dem.OracleHcmId += (int)lastSevenDigits;
            }

            await c.SaveChangesAsync();

            await c.SaveChangesAsync();

            var stream = await _cleanupReportClient.DownloadDemographicBadgesNotInPayProfit(CancellationToken.None);
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
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var request = new PaginationRequestDto() { Skip = 0, Take = 1000 };
            var response = await _cleanupReportClient.GetNamesMissingComma(request, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Count().Should().Be(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            byte disruptedNameCount = 10;
            foreach (var dem in ctx.Demographics.Take(disruptedNameCount))
            {
                dem.FullName = dem.FullName?.Replace(", ", " ");
            }

            await ctx.SaveChangesAsync();

            response = await _cleanupReportClient.GetNamesMissingComma(request, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Count().Should().Be(disruptedNameCount);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            var oneRecord = new PaginationRequestDto { Skip = 0, Take = 1 };
            response = await _cleanupReportClient.GetNamesMissingComma(oneRecord, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        });
    }

    [Fact(DisplayName = "PS-153: Names without commas (CSV)")]
    public async Task GetNamesWithoutCommasCsv()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            byte disruptedNameCount = 10;
            await ctx.Demographics.Take(disruptedNameCount).ForEachAsync(dem =>
            {
                dem.FullName = dem.FullName?.Replace(", ", " ");
            });

            await ctx.SaveChangesAsync();

            var stream = await _cleanupReportClient.DownloadNamesMissingComma(CancellationToken.None);
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
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        byte negativeValues = 5;
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            await c.PayProfits.Take(negativeValues).ForEachAsync(pp =>
            {
                pp.EarningsEtvaValue *= -1;
            });


            await c.SaveChangesAsync();
        });


        var response = await _cleanupReportClient.GetNegativeETVAForSSNsOnPayProfitResponse(_paginationRequest, CancellationToken.None);

        response.Should().NotBeNull();
        response.ReportName.Should().BeEquivalentTo("Negative ETVA for SSNs on PayProfit");
        response.Response.Results.Should().HaveCount(negativeValues);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        var oneRecord = new ProfitYearRequest {ProfitYear = _paginationRequest.ProfitYear, Skip = 0, Take = 1 };
        response = await _cleanupReportClient.GetNegativeETVAForSSNsOnPayProfitResponse(oneRecord, CancellationToken.None);
        response.Should().NotBeNull();
        response.Response.Results.Should().HaveCount(1);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-145 : Negative ETVA for SSNs on PayProfit (CSV)")]
    public async Task GetNegativeEtvaReportCsv()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var stream = await _cleanupReportClient.DownloadNegativeETVAForSSNsOnPayProfitResponse(_paginationRequest.ProfitYear, CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }

    [Fact(DisplayName = "PS-149 : Mismatched Ssns Payprofit and Demographics On Same Badge (JSON)")]
    public async Task GetMismatchedSsnsPayprofitAndDemographicsOnSameBadgeJson()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await _cleanupReportClient.GetMismatchedSsnsPayprofitAndDemographicsOnSameBadge(_paginationRequest, CancellationToken.None);

        response.Should().NotBeNull();
        response.ReportName.Should().BeEquivalentTo("MISMATCHED SSNs PAYPROFIT AND DEMO ON SAME BADGE");

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-149 : Mismatched Ssns Payprofit and Demographics On Same Badge (CSV)")]
    public async Task GetMismatchedSsnsPayprofitAndDemographicsOnSameBadgeCsv()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var stream = await _cleanupReportClient.DownloadMismatchedSsnsPayprofitAndDemographicsOnSameBadge(_paginationRequest.ProfitYear, CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }

    [Fact(DisplayName = "PS-152 : Duplicate names and Birthdays (JSON)")]
    public async Task GetDuplicateNamesAndBirthdays()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new ProfitYearRequest {ProfitYear = _paginationRequest.ProfitYear, Take = 1000, Skip = 0 };
        var response = await _cleanupReportClient.GetDuplicateNamesAndBirthdays(request, CancellationToken.None);
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
            }

            await c.SaveChangesAsync();
        });

        response = await _cleanupReportClient.GetDuplicateNamesAndBirthdays(request, CancellationToken.None);
        response.Should().NotBeNull();
        response.Response.Results.Count().Should().Be(duplicateRows + 1);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        var oneRecord = new ProfitYearRequest { ProfitYear = _paginationRequest.ProfitYear, Skip = 0, Take = 1 };
        response = await _cleanupReportClient.GetDuplicateNamesAndBirthdays(oneRecord, CancellationToken.None);
        response.Should().NotBeNull();
        response.Response.Results.Should().HaveCount(1);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-152 : Duplicate names and Birthdays (CSV)")]
    public async Task GetDuplicateNamesAndBirthdaysCsv()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
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

        var stream = await _cleanupReportClient.DownloadDuplicateNamesAndBirthdays(_paginationRequest.ProfitYear, CancellationToken.None);
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
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await _cleanupReportClient.GetPayrollDuplicateSsnsOnPayprofit(_paginationRequest, CancellationToken.None);

        response.Should().NotBeNull();
        response.ReportName.Should().BeEquivalentTo("PAYROLL DUPLICATE SSNs ON PAYPROFIT");

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-148 : Payroll Duplicate Ssns On Payprofit (CSV)")]
    public async Task GetPayrollDuplicateSsnsOnPayprofitCsv()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var stream = await _cleanupReportClient.DownloadPayrollDuplicateSsnsOnPayprofit(CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }

    [Fact(DisplayName = "CleanupReportService auth check")]
    public async Task YearEndServiceAuthCheck()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.HARDSHIPADMINISTRATOR);
        await Assert.ThrowsAsync<HttpRequestException>(async () => { _ = await _cleanupReportClient.GetPayrollDuplicateSsnsOnPayprofit(_paginationRequest, CancellationToken.None); });
    }
}
