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
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

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
                dem.ContactInfo.FullName = dem.ContactInfo.FullName?.Replace(", ", " ");
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
                dem.ContactInfo.FullName = dem.ContactInfo.FullName?.Replace(", ", " ");
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
                pp.ProfitYear = _paginationRequest.ProfitYear;
            });


            await c.SaveChangesAsync();



            var response = await _cleanupReportClient.GetNegativeETVAForSSNsOnPayProfitResponse(_paginationRequest, CancellationToken.None);

            response.Should().NotBeNull();
            response.ReportName.Should().BeEquivalentTo("Negative ETVA for SSNs on PayProfit");
            response.Response.Results.Should().HaveCount(negativeValues);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            var oneRecord = new ProfitYearRequest { ProfitYear = _paginationRequest.ProfitYear, Skip = 0, Take = 1 };
            response = await _cleanupReportClient.GetNegativeETVAForSSNsOnPayProfitResponse(oneRecord, CancellationToken.None);
            response.Should().NotBeNull();
            response.Response.Results.Should().HaveCount(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        });
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
            var modelDemographic = await c.Demographics.Include(demographic => demographic.ContactInfo).FirstAsync();

            foreach (var dem in c.Demographics.Take(duplicateRows))
            {
                dem.DateOfBirth = modelDemographic.DateOfBirth;
                dem.ContactInfo.FirstName = modelDemographic.ContactInfo.FirstName;
                dem.ContactInfo.LastName = modelDemographic.ContactInfo.LastName;
                dem.ContactInfo.FullName = modelDemographic.ContactInfo.FullName;
                dem.PayProfits[0]!.ProfitYear = _paginationRequest.ProfitYear;
            }
            
            await c.SaveChangesAsync();
        });

        response = await _cleanupReportClient.GetDuplicateNamesAndBirthdays(request, CancellationToken.None);
        response.Should().NotBeNull();
        response.Response.Results.Count().Should().BeGreaterThanOrEqualTo(duplicateRows);

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
            var modelDemographic = await c.Demographics.Include(demographic => demographic.ContactInfo).FirstAsync();

            foreach (var dem in c.Demographics.Take(duplicateRows))
            {
                dem.DateOfBirth = modelDemographic.DateOfBirth;
                dem.ContactInfo.FirstName = modelDemographic.ContactInfo.FirstName;
                dem.ContactInfo.LastName = modelDemographic.ContactInfo.LastName;
                dem.ContactInfo.FullName = modelDemographic.ContactInfo.FullName;
                dem.PayProfits[0]!.ProfitYear = _paginationRequest.ProfitYear;
            }

            await c.SaveChangesAsync();
        });

        var stream = await _cleanupReportClient.DownloadDuplicateNamesAndBirthdays(_paginationRequest.ProfitYear, CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        var lines = result.Split(Environment.NewLine);
        lines.Count().Should().BeGreaterThanOrEqualTo(duplicateRows + 4); //Includes initial row that was used as the template to create duplicates

        _testOutputHelper.WriteLine(result);

    }

    [Fact(DisplayName = "PS-61 : Year-end Profit Sharing Report (JSON)")]
    public async Task GetYearEndProfitSharingReport()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var profitYear = (short)(DateTime.Now.Year - 1);
        var req = new YearEndProfitSharingReportRequest() { Skip = 0, Take = byte.MaxValue, ProfitYear = profitYear, IsYearEnd = true };
        var testHours = 1001;
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            //Terminate all employees so that none of the random ones are returned
            foreach (var dem in ctx.Demographics)
            {
                dem.EmploymentStatusId = 't';
            }

            //Prevent any payprofit records from being returned
            foreach (var pp in ctx.PayProfits)
            {
                pp.ProfitYear = 1999;
            }

            //Setup employee to be returned
            var payProfit = await ctx.PayProfits.FirstAsync();
            var emp = payProfit.Demographic;

            emp!.EmploymentStatusId = 'a';
            emp!.DateOfBirth = new DateOnly(DateTime.Now.Year - 28, 9, 21);

            payProfit.ProfitYear = profitYear;
            payProfit.CurrentHoursYear = testHours;
            payProfit.HoursExecutive = 0;

            await ctx.SaveChangesAsync();
        });

        ReportResponseBase<YearEndProfitSharingReportResponse> response;

        response = await _cleanupReportClient.GetYearEndProfitSharingReport(req, CancellationToken.None);

        response.Should().NotBeNull();
        response.ReportName.Should().BeEquivalentTo($"PROFIT SHARE YEAR END REPORT FOR {req.ProfitYear}");
        response.Response.Total.Should().Be( 1 );
        response.Response.Results.Count().Should().Be(1);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        //Test omission if under 18
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            //Setup employee to be returned
            var payProfit = await ctx.PayProfits.FirstAsync();
            var emp = payProfit.Demographic;

            emp!.DateOfBirth = new DateOnly(DateTime.Now.Year - 15, 9, 21);
            await ctx.SaveChangesAsync();
        });

        response = await _cleanupReportClient.GetYearEndProfitSharingReport(req, CancellationToken.None);

        response.Should().NotBeNull();
        response.Response.Total.Should().Be(0);
        response.Response.Results.Count().Should().Be(0);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        //Test under 1000 hours
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            //Setup employee to be returned
            var payProfit = await ctx.PayProfits.FirstAsync();
            var emp = payProfit.Demographic;

            emp!.DateOfBirth = new DateOnly(DateTime.Now.Year - 28, 9, 21);
            payProfit.CurrentHoursYear = 50;
            await ctx.SaveChangesAsync();
        });

        response = await _cleanupReportClient.GetYearEndProfitSharingReport(req, CancellationToken.None);

        response.Should().NotBeNull();
        response.Response.Total.Should().Be(0);
        response.Response.Results.Count().Should().Be(0);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName ="PS-294 : Distributions and Forfeitures (JSON)")]
    public async Task GetDistributionsAndForfeitures()
    {
        decimal sampleforfeiture = 5150m;

        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var req = new DistributionsAndForfeituresRequest() { Skip=0, Take=byte.MaxValue, ProfitYear = (short)(DateTime.Now.Year-1), IncludeOutgoingForfeitures=true};
        ReportResponseBase<DistributionsAndForfeitureResponse> response;

        
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            //Clear out existing data, so that random numbers don't cause the numbers to inflate
            foreach (var dem in ctx.Demographics)
            {
                dem.Ssn = -1;
            }
            foreach (var ben in ctx.Beneficiaries.Include(b=> b.Contact))
            {
                ben.Contact!.Ssn = -1;
                ben.PsnSuffix = -1;
            }
            await ctx.SaveChangesAsync();
        });
        var distributionProfitCodes = new[] { 1, 3, 9 }; //Test to see that the forfeiture ends up in the right column for these codes.
        foreach (var profitCode in distributionProfitCodes)
        {
            await MockDbContextFactory.UseWritableContext(async ctx =>
            {
                var demographic = await ctx.Demographics.FirstAsync();
                demographic.Ssn = 1001;

                var profitDetail = await ctx.ProfitDetails.FirstAsync();

                profitDetail.ProfitYear = (short)(DateTime.Now.Year - 1);
                profitDetail.ProfitYearIteration = 0;
                profitDetail.ProfitCodeId = (byte)profitCode;
                profitDetail.ProfitCode = new Data.Entities.ProfitCode() { Id = 1, Name = "Incoming contributions, forfeitures, earnings", Frequency = "Yearly" };
                profitDetail.Forfeiture = sampleforfeiture;
                profitDetail.MonthToDate = 3;
                profitDetail.YearToDate = (short)(DateTime.Now.Year - 1);
                profitDetail.FederalTaxes = 100;
                profitDetail.StateTaxes = 50;
                profitDetail.TaxCodeId = '7';
                profitDetail.Ssn = demographic.Ssn;
                profitDetail.DistributionSequence = 6011;

                await ctx.SaveChangesAsync();
            });
            response = await _cleanupReportClient.GetDistributionsAndForfeiture(req, CancellationToken.None);

            response.Should().NotBeNull();
            response.ReportName.Should().BeEquivalentTo("DISTRIBUTIONS AND FORFEITURES");
            response.Response.Results.Count().Should().Be(1);
            response.Response.Results.First().DistributionAmount.Should().Be(sampleforfeiture);
            response.Response.Results.First().ForfeitAmount.Should().Be(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        }

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var profitDetail = await ctx.ProfitDetails.FirstAsync();

            profitDetail.ProfitCodeId = 2; //This profit code should end up in the forfeit column
            await ctx.SaveChangesAsync();
        });

        response = await _cleanupReportClient.GetDistributionsAndForfeiture(req, CancellationToken.None);

        response.Should().NotBeNull();
        response.Response.Results.Count().Should().Be(1);
        response.Response.Results.First().DistributionAmount.Should().Be(0);
        response.Response.Results.First().ForfeitAmount.Should().Be(sampleforfeiture);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var profitDetail = await ctx.ProfitDetails.FirstAsync();

            profitDetail.ProfitCodeId = 6; //This profit code shouldn't be in the report
            await ctx.SaveChangesAsync();
        });

        response = await _cleanupReportClient.GetDistributionsAndForfeiture(req, CancellationToken.None);

        response.Should().NotBeNull();
        response.Response.Results.Count().Should().Be(0);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var profitDetail = await ctx.ProfitDetails.FirstAsync();

            profitDetail.ProfitCodeId = 9; //This profit code shouldn't be in the report if is a transfer
            profitDetail.IsTransferOut = true;
            await ctx.SaveChangesAsync();
        });

        response = await _cleanupReportClient.GetDistributionsAndForfeiture(req, CancellationToken.None);

        response.Should().NotBeNull();
        response.Response.Results.Count().Should().Be(0);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "CleanupReportService auth check")]
    public async Task YearEndServiceAuthCheck()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.HARDSHIPADMINISTRATOR);
        await Assert.ThrowsAsync<HttpRequestException>(async () => { _ = await _cleanupReportClient.GetDemographicBadgesNotInPayProfit(_paginationRequest, CancellationToken.None); });
    }
}
