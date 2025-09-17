using System.Text;
using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Report;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.ProfitSharing.UnitTests.Common.Helpers;

using FastEndpoints;
using IdGen;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class CleanupReportServiceTests : ApiTestBase<Program>
{
    private readonly CleanupReportClient _cleanupReportClient;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ProfitYearRequest _paginationRequest = new ProfitYearRequest { ProfitYear = 2023, Skip = 0, Take = byte.MaxValue };
    private readonly IdGenerator _generator;


    public CleanupReportServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _cleanupReportClient = new CleanupReportClient(ApiClient, DownloadClient);
        _generator = new IdGenerator(0);
    }


    [Fact(DisplayName = "PS-147: Check Duplicate SSNs (JSON)")]
    public async Task GetDuplicateSsNsTestJson()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await _cleanupReportClient.GetDuplicateSsnAsync(_paginationRequest, CancellationToken.None);
        response.ShouldNotBeNull();
        response.Response.Results.Count().ShouldBe(0);
    }

    [Fact(DisplayName = "PS-147: Check Duplicate SSNs (CSV)")]
    public async Task GetDuplicateSsNsTestCsv()
    {
        DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await DownloadClient
            .GETAsync<GetDuplicateSsNsEndpoint, ProfitYearRequest, PayrollDuplicateSsnResponseDto>(_paginationRequest);


        string content = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);

        content.ShouldNotBeNullOrEmpty();

        _testOutputHelper.WriteLine(content);
    }

    [Fact(DisplayName = "PS-151: Demographic badges without payprofit (JSON)")]
    public Task GetDemographicBadgesWithoutPayProfitTests()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            var response = await _cleanupReportClient.GetDemographicBadgesNotInPayProfitAsync(_paginationRequest, CancellationToken.None);
            response.ShouldNotBeNull();
            response.Response.Results.Count().ShouldBe(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            byte mismatchedValues = 5;

            foreach (var dem in c.Demographics.Take(mismatchedValues))
            {
                long lastSevenDigits = _generator.CreateId() % 10_000_000;
                dem.Id += (int)lastSevenDigits;
            }

            await c.SaveChangesAsync(CancellationToken.None);

            response = await _cleanupReportClient.GetDemographicBadgesNotInPayProfitAsync(_paginationRequest, CancellationToken.None);
            response.ShouldNotBeNull();
            response.Response.Results.Count().ShouldBe(mismatchedValues);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            var oneRecord = new ProfitYearRequest { Skip = 0, Take = 1, ProfitYear = _paginationRequest.ProfitYear };
            response = await _cleanupReportClient.GetDemographicBadgesNotInPayProfitAsync(oneRecord, CancellationToken.None);
            response.ShouldNotBeNull();
            response.Response.Results.Count().ShouldBe(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        });
    }

    [Fact(DisplayName = "PS-151: Demographic badges without payprofit (CSV)")]
    public Task GetDemographicBadgesWithoutPayProfitTestsCsv()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            byte mismatchedValues = 5;

            foreach (var dem in c.Demographics.Take(mismatchedValues))
            {
                long lastSevenDigits = _generator.CreateId() % 10_000_000;
                dem.Id += (int)lastSevenDigits;
            }

            await c.SaveChangesAsync(CancellationToken.None);

            await c.SaveChangesAsync(CancellationToken.None);

            var stream = await _cleanupReportClient.DownloadDemographicBadgesNotInPayProfit(_paginationRequest.ProfitYear, CancellationToken.None);
            stream.ShouldNotBeNull();

            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
            string result = await reader.ReadToEndAsync(CancellationToken.None);
            result.ShouldNotBeNullOrEmpty();

            var lines = result.Split(Environment.NewLine);
            lines.Count().ShouldBe(mismatchedValues + 4);

            _testOutputHelper.WriteLine(result);
        });
    }



    [Fact(DisplayName = "PS-145 : Negative ETVA for SSNs on PayProfit (JSON)")]
    public Task GetNegativeEtvaReportJson()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);
        byte negativeValues = 5;
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            await c.PayProfits.Take(negativeValues).ForEachAsync(pp =>
            {
                pp.Etva *= -1;
                pp.ProfitYear = _paginationRequest.ProfitYear;
            });


            await c.SaveChangesAsync(CancellationToken.None);


            var response = await _cleanupReportClient.GetNegativeETVAForSSNsOnPayProfitResponseAsync(_paginationRequest, CancellationToken.None);

            response.ShouldNotBeNull();
            response.ReportName.ShouldBeEquivalentTo("Negative ETVA for SSNs on PayProfit");
            response.Response.Results.Count().ShouldBe(negativeValues);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            var oneRecord = new ProfitYearRequest { ProfitYear = _paginationRequest.ProfitYear, Skip = 0, Take = 1 };
            response = await _cleanupReportClient.GetNegativeETVAForSSNsOnPayProfitResponseAsync(oneRecord, CancellationToken.None);
            response.ShouldNotBeNull();
            response.Response.Results.Count().ShouldBe(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        });
    }

    [Fact(DisplayName = "PS-145 : Negative ETVA for SSNs on PayProfit (CSV)")]
    public async Task GetNegativeEtvaReportCsv()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var stream = await _cleanupReportClient.DownloadNegativeETVAForSSNsOnPayProfitResponse(_paginationRequest.ProfitYear, CancellationToken.None);
        stream.ShouldNotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync(CancellationToken.None);
        result.ShouldNotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }



    [Fact(DisplayName = "PS-61 : Year-end Profit Sharing Report (JSON)")]
    public async Task GetYearEndProfitSharingReport()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var profitYear = (short)(2024);
        var req = new YearEndProfitSharingReportRequest()
        {
            Skip = 0,
            Take = byte.MaxValue,
            ProfitYear = profitYear,
            ReportId = YearEndProfitSharingReportId.Age21OrOlderWith1000Hours
            // Default to report 2 for active/inactive
        };
        var testHours = 1001;
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            //Terminate all employees so that none of the random ones are returned
            foreach (var dem in ctx.Demographics)
            {
                dem.EmploymentStatusId = 't';
            }

            foreach (var demH in ctx.DemographicHistories)
            {
                demH.EmploymentStatusId = 't';
            }

            //Prevent any payprofit records from being returned
            foreach (var pp in ctx.PayProfits)
            {
                pp.ProfitYear = 2100;
            }

            //Setup employee to be returned
            var payProfit = await ctx.PayProfits.Include(payProfit => payProfit.Demographic).FirstAsync(CancellationToken.None);
            var emp = payProfit.Demographic;
            var empH = await ctx.DemographicHistories.FirstAsync(x => x.DemographicId == emp!.Id);

            emp!.EmploymentStatusId = 'a';
            emp.DateOfBirth = new DateOnly(DateTime.Now.Year - 28, 9, 21);
            empH.EmploymentStatusId = emp.EmploymentStatusId;
            empH.DateOfBirth = emp.DateOfBirth;

            payProfit.ProfitYear = profitYear;
            payProfit.CurrentHoursYear = testHours;
            payProfit.HoursExecutive = 0;

            var profitDetails = await ctx.ProfitDetails.Where(x => x.Ssn == emp.Ssn).ToListAsync(CancellationToken.None);
            foreach (var pd in profitDetails.Skip(2))
            {
                pd.Ssn = 0; //Reset the profit detail records
            }

            profitDetails[0].ProfitYear = profitYear;
            profitDetails[0].ProfitYearIteration = 0;
            profitDetails[0].ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id;
            profitDetails[0].ProfitCode = ProfitCode.Constants.IncomingContributions;
            profitDetails[1].ProfitYear = (short)(profitYear - 1);
            profitDetails[1].ProfitYearIteration = 0;
            profitDetails[1].ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id;
            profitDetails[1].ProfitCode = ProfitCode.Constants.IncomingContributions;

            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        var response = await ApiClient.POSTAsync<YearEndProfitSharingReportEndpoint, YearEndProfitSharingReportRequest, YearEndProfitSharingReportResponse>(req);

        response.Result.ShouldNotBeNull();
        response.Result.ReportName.ShouldBeEquivalentTo($"PROFIT SHARE REPORT (PAY426) - {req.ProfitYear}");
        response.Result.Response.Total.ShouldBeGreaterThanOrEqualTo(1);
        response.Result.Response.Results.Count().ShouldBeGreaterThanOrEqualTo(1);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        //Test omission if under 18
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            //Setup employee to be returned
            var payProfit = await ctx.PayProfits.Include(payProfit => payProfit.Demographic).FirstAsync(CancellationToken.None);
            var emp = payProfit.Demographic;
            var empH = await ctx.DemographicHistories.FirstAsync(x => x.DemographicId == emp!.Id);

            emp!.DateOfBirth = new DateOnly(DateTime.Now.Year - 15, 9, 21);
            empH.DateOfBirth = emp.DateOfBirth;
            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        response = await ApiClient.POSTAsync<YearEndProfitSharingReportEndpoint, YearEndProfitSharingReportRequest, YearEndProfitSharingReportResponse>(req);

        response.Result.ShouldNotBeNull();
        response.Result.Response.Total.ShouldBe(0);
        response.Result.Response.Results.Count().ShouldBe(0);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        //Test under 1000 hours
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            //Setup employee to be returned
            var payProfit = await ctx.PayProfits.Include(payProfit => payProfit.Demographic).FirstAsync(CancellationToken.None);
            var emp = payProfit.Demographic;

            emp!.DateOfBirth = new DateOnly(DateTime.Now.Year - 28, 9, 21);
            payProfit.CurrentHoursYear = 50;
            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        response = await ApiClient.POSTAsync<YearEndProfitSharingReportEndpoint, YearEndProfitSharingReportRequest, YearEndProfitSharingReportResponse>(req);

        response.Result.ShouldNotBeNull();
        response.Result.Response.Total.ShouldBe(0);
        response.Result.Response.Results.Count().ShouldBe(0);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-399 : Year-end Profit Sharing Report with filters (JSON")]
    public async Task GetYearEndProfitSharingReportWithFilters()
    {
        _cleanupReportClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var profitYear = (short)Math.Min(DateTime.Now.Year - 1, 2024);
        var req = new YearEndProfitSharingReportRequest() { Skip = 0, Take = byte.MaxValue, ProfitYear = profitYear, ReportId = YearEndProfitSharingReportId.Age21OrOlderWith1000Hours };
        var testHours = 1001;
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            foreach (var dem in ctx.Demographics)
            {
                dem.EmploymentStatusId = EmploymentStatus.Constants.Delete;
            }

            foreach (var demh in ctx.DemographicHistories)
            {
                demh.EmploymentStatusId = EmploymentStatus.Constants.Delete;
            }

            foreach (var pp in ctx.PayProfits)
            {
                pp.ProfitYear = 2100;
            }

            var payProfit = await ctx.PayProfits.Include(payProfit => payProfit.Demographic).FirstAsync(CancellationToken.None);
            var emp = payProfit.Demographic;
            var empH = await ctx.DemographicHistories.FirstAsync(x => x.DemographicId == emp!.Id);

            emp!.EmploymentStatusId = EmploymentStatus.Constants.Active;
            emp.DateOfBirth = new DateOnly(DateTime.Now.Year - 28, 9, 21);
            empH.EmploymentStatusId = emp.EmploymentStatusId;
            empH.DateOfBirth = emp.DateOfBirth;

            payProfit.ProfitYear = profitYear;
            payProfit.CurrentHoursYear = testHours;
            payProfit.HoursExecutive = 0;

            var profitDetails = await ctx.ProfitDetails.Where(x => x.Ssn == emp.Ssn).ToListAsync(CancellationToken.None);
            foreach (var pd in profitDetails.Skip(2))
            {
                pd.Ssn = 0;
            }

            profitDetails[0].ProfitYear = profitYear;
            profitDetails[0].ProfitYearIteration = 0;
            profitDetails[0].ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id;
            profitDetails[0].ProfitCode = ProfitCode.Constants.IncomingContributions;
            profitDetails[1].ProfitYear = (short)(profitYear - 1);
            profitDetails[1].ProfitYearIteration = 0;
            profitDetails[1].ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id;
            profitDetails[1].ProfitCode = ProfitCode.Constants.IncomingContributions;

            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        var response = await ApiClient.POSTAsync<YearEndProfitSharingReportEndpoint, YearEndProfitSharingReportRequest, YearEndProfitSharingReportResponse>(req);
        response.ShouldNotBeNull();
        response.Result.ReportName.ShouldBeEquivalentTo($"PROFIT SHARE REPORT (PAY426) - {req.ProfitYear}");
        response.Result.Response.Total.ShouldBeGreaterThanOrEqualTo(1);
        response.Result.Response.Results.Count().ShouldBeGreaterThanOrEqualTo(1);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        // Test with different ReportIds for different filters
        req.ReportId = YearEndProfitSharingReportId.Under18; // Example: < AGE 18
        response = await ApiClient.POSTAsync<YearEndProfitSharingReportEndpoint, YearEndProfitSharingReportRequest, YearEndProfitSharingReportResponse>(req);
        response.ShouldNotBeNull();
        // Add assertions as needed for this filter

        req.ReportId = YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount; // Example: >= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT
        response = await ApiClient.POSTAsync<YearEndProfitSharingReportEndpoint, YearEndProfitSharingReportRequest, YearEndProfitSharingReportResponse>(req);
        response.ShouldNotBeNull();
        // Add assertions as needed for this filter
    }

    [Fact(DisplayName = "PS-294 : Distributions and Forfeitures (JSON)")]
    public async Task GetDistributionsAndForfeitures()
    {
        decimal sampleforfeiture = 5150m;

        _cleanupReportClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var req = new DistributionsAndForfeituresRequest() { Skip = 0, Take = byte.MaxValue };
        TestResult<DistributionsAndForfeitureTotalsResponse> response;


        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            //Clear out existing data, so that random numbers don't cause the numbers to inflate
            foreach (var dem in ctx.Demographics)
            {
                dem.Ssn = -1;
            }

            foreach (var ben in ctx.Beneficiaries.Include(b => b.Contact))
            {
                ben.Contact!.Ssn = -1;
                ben.PsnSuffix = -1;
            }

            await ctx.SaveChangesAsync(CancellationToken.None);
        });
        var distributionProfitCodes = new[] { 1, 3, 9 }; //Test to see that the forfeiture ends up in the right column for these codes.
        foreach (var profitCode in distributionProfitCodes)
        {
            await MockDbContextFactory.UseWritableContext(async ctx =>
            {
                var demographic = await ctx.Demographics.FirstAsync(CancellationToken.None);
                demographic.Ssn = 1001;

                var profitDetail = await ctx.ProfitDetails.FirstAsync(CancellationToken.None);

                profitDetail.ProfitYear = (short)(DateTime.Now.Year - 1);
                profitDetail.ProfitYearIteration = 0;
                profitDetail.ProfitCodeId = (byte)profitCode;
                profitDetail.Forfeiture = sampleforfeiture;
                profitDetail.MonthToDate = 3;
                profitDetail.YearToDate = (short)(DateTime.Now.Year - 1);
                profitDetail.FederalTaxes = 100;
                profitDetail.StateTaxes = 50;
                profitDetail.TaxCodeId = '7';
                profitDetail.Ssn = demographic.Ssn;
                profitDetail.DistributionSequence = 6011;

                await ctx.SaveChangesAsync(CancellationToken.None);
            });

            response = await ApiClient
                    .GETAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);


            response.Result.ShouldNotBeNull();
            response.Result.ReportName.ShouldBeEquivalentTo("DISTRIBUTIONS AND FORFEITURES");
            response.Result.Response.Results.Count().ShouldBe(1);
            response.Result.Response.Results.First().DistributionAmount.ShouldBe(sampleforfeiture);
            response.Result.Response.Results.First().ForfeitAmount.ShouldBe(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        }

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var profitDetail = await ctx.ProfitDetails.FirstAsync(CancellationToken.None);

            profitDetail.ProfitCodeId = 2; //This profit code should end up in the forfeit column
            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        response = await ApiClient
            .GETAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest,
                DistributionsAndForfeitureTotalsResponse>(req);

        response.Result.ShouldNotBeNull();
        response.Result.Response.Results.Count().ShouldBe(1);
        response.Result.Response.Results.First().DistributionAmount.ShouldBe(0);
        response.Result.Response.Results.First().ForfeitAmount.ShouldBe(sampleforfeiture);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var profitDetail = await ctx.ProfitDetails.FirstAsync(CancellationToken.None);

            profitDetail.ProfitCodeId = 6; //This profit code shouldn't be in the report
            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        response = await ApiClient
            .GETAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest,
                DistributionsAndForfeitureTotalsResponse>(req);

        response.ShouldNotBeNull();
        response.Result.Response.Results.Count().ShouldBe(0);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var profitDetail = await ctx.ProfitDetails.FirstAsync(CancellationToken.None);

            profitDetail.ProfitCodeId = 9; //This profit code shouldn't be in the report if is a transfer
            profitDetail.CommentTypeId = CommentType.Constants.TransferOut;
            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        response = await ApiClient
            .GETAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest,
                DistributionsAndForfeitureTotalsResponse>(req);

        response.ShouldNotBeNull();
        response.Result.Response.Results.Count().ShouldBe(0);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    //[Fact(DisplayName = "CleanupReportService auth check")]
    //public Task YearEndServiceAuthCheck()
#pragma warning disable S125
    //{
#pragma warning restore S125
    //    _cleanupReportClient.CreateAndAssignTokenForClient(Role.HARDSHIPADMINISTRATOR);
    //    return Assert.ThrowsAsync<HttpRequestException>(async () =>
    //    {
    //        _ = await _cleanupReportClient.GetDemographicBadgesNotInPayProfitAsync(_paginationRequest, CancellationToken.None);
    //    });
    //}

    [Fact(DisplayName = "GetYearEndProfitSharingSummaryReportAsync Tests")]
    public async Task GetYearEndProfitSharingSummaryReportAsyncCheck()
    {
        var req = new FrozenProfitYearRequest() { ProfitYear = 2024, UseFrozenData = false };
        var response =
            await ApiClient
                .POSTAsync<YearEndProfitSharingSummaryReportEndpoint,
                    FrozenProfitYearRequest, YearEndProfitSharingReportSummaryResponse>(req);

        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        response =
            await ApiClient
                .POSTAsync<YearEndProfitSharingSummaryReportEndpoint,
                    FrozenProfitYearRequest, YearEndProfitSharingReportSummaryResponse>(req);

        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

    }
}
