using System.ComponentModel;
using System.Text;
using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Report;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
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
using Microsoft.Extensions.DependencyInjection;
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
                    .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);


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
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest,
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
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest,
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
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest,
                DistributionsAndForfeitureTotalsResponse>(req);

        response.ShouldNotBeNull();
        response.Result.Response.Results.Count().ShouldBe(0);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact]
    [Description("PS-1731 : Distributions and Forfeitures returns Result<T> pattern and handles successful case")]
    public async Task GetDistributionsAndForfeitures_ReturnsResultPattern()
    {
        // Arrange - create a service directly to test Result<T> pattern
        var cleanupService = ServiceProvider!.GetRequiredService<ICleanupReportService>();
        var req = new DistributionsAndForfeituresRequest() { Skip = 0, Take = byte.MaxValue };

        // Act - call service method directly
        var result = await cleanupService.GetDistributionsAndForfeitureAsync(req, CancellationToken.None);

        // Assert - verify it returns Result<T> pattern correctly
        result.ShouldNotBeNull();

        if (result.IsSuccess)
        {
            // When successful, verify the structure
            result.IsError.ShouldBeFalse();
            result.Value.ShouldNotBeNull();
            result.Error.ShouldBeNull();
            result.Value!.ReportName.ShouldBe("Distributions and Forfeitures");
            result.Value.Response.ShouldNotBeNull();
            _testOutputHelper.WriteLine($"Success: Returned {result.Value.Response.Results.Count()} records");
        }
        else
        {
            // When it fails (like no PayProfits data), verify error structure
            result.IsError.ShouldBeTrue();
            result.IsSuccess.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
            result.Value.ShouldBeNull();

            // Could be NoPayProfitsDataAvailable or other validation errors
            result.Error.Code.ShouldBeOneOf(105); // NoPayProfitsDataAvailable
            _testOutputHelper.WriteLine($"Error: {result.Error.Description} (Code: {result.Error.Code})");
        }

        _testOutputHelper.WriteLine($"Result pattern validation passed - IsSuccess: {result.IsSuccess}, IsError: {result.IsError}");
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

    [Fact]
    [Description("PS-1902 : Distributions filtering by states array - empty array returns all")]
    public async Task GetDistributionsAndForfeitures_WithEmptyStatesArray_ReturnsAll()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var req = new DistributionsAndForfeituresRequest()
        {
            Skip = 0,
            Take = byte.MaxValue,
            States = [] // Empty array = "All"
        };

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();
        result.Response.ShouldNotBeNull();
        // Empty array should not filter - returns all records
        _testOutputHelper.WriteLine($"Empty states array returned {result.Response.Results.Count()} records");
    }

    [Fact]
    [Description("PS-1902 : Distributions filtering by states array - specific states filter results")]
    public async Task GetDistributionsAndForfeitures_WithSpecificStates_FiltersResults()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // First get all records to have a baseline
        var reqAll = new DistributionsAndForfeituresRequest()
        {
            Skip = 0,
            Take = byte.MaxValue,
            States = []
        };
        var responseAll = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(reqAll);
        var allCount = responseAll.Result?.Response.Results.Count() ?? 0;

        // Now filter by specific state (MA)
        var reqFiltered = new DistributionsAndForfeituresRequest()
        {
            Skip = 0,
            Take = byte.MaxValue,
            States = ["MA"] // Only Massachusetts
        };

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(reqFiltered);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();
        result.Response.ShouldNotBeNull();

        // Filtered results should be <= all results
        var filteredCount = result.Response.Results.Count();
        filteredCount.ShouldBeLessThanOrEqualTo(allCount);

        // All returned records should have State = "MA" (or null/empty if no state data)
        var nonMARecords = result.Response.Results.Where(r => r.State != "MA" && !string.IsNullOrEmpty(r.State)).ToList();
        nonMARecords.ShouldBeEmpty($"Expected only MA records, but found: {string.Join(", ", nonMARecords.Select(r => r.State))}");

        _testOutputHelper.WriteLine($"All records: {allCount}, MA filtered: {filteredCount}");
    }

    [Fact]
    [Description("PS-1902 : Distributions filtering by multiple states")]
    public async Task GetDistributionsAndForfeitures_WithMultipleStates_ReturnsMatchingRecords()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var req = new DistributionsAndForfeituresRequest()
        {
            Skip = 0,
            Take = byte.MaxValue,
            States = ["MA", "CT", "NH"] // Multiple states
        };

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();
        result.Response.ShouldNotBeNull();

        // All returned records should have State in ["MA", "CT", "NH"] (or null/empty)
        var invalidRecords = result.Response.Results
            .Where(r => !string.IsNullOrEmpty(r.State) && !new[] { "MA", "CT", "NH" }.Contains(r.State))
            .ToList();
        invalidRecords.ShouldBeEmpty($"Expected only MA/CT/NH records, but found: {string.Join(", ", invalidRecords.Select(r => r.State))}");

        _testOutputHelper.WriteLine($"Multi-state filter returned {result.Response.Results.Count()} records");
    }

    [Fact]
    [Description("PS-1902 : Distributions filtering by tax codes array - empty array returns all")]
    public async Task GetDistributionsAndForfeitures_WithEmptyTaxCodesArray_ReturnsAll()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var req = new DistributionsAndForfeituresRequest()
        {
            Skip = 0,
            Take = byte.MaxValue,
            TaxCodes = [] // Empty array = "All"
        };

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();
        result.Response.ShouldNotBeNull();
        _testOutputHelper.WriteLine($"Empty tax codes array returned {result.Response.Results.Count()} records");
    }

    [Fact]
    [Description("PS-1902 : Distributions filtering by tax codes array - specific codes filter results")]
    public async Task GetDistributionsAndForfeitures_WithSpecificTaxCodes_FiltersResults()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // First get all records
        var reqAll = new DistributionsAndForfeituresRequest()
        {
            Skip = 0,
            Take = byte.MaxValue,
            TaxCodes = []
        };
        var responseAll = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(reqAll);
        var allCount = responseAll.Result?.Response.Results.Count() ?? 0;

        // Now filter by specific tax code
        var reqFiltered = new DistributionsAndForfeituresRequest()
        {
            Skip = 0,
            Take = byte.MaxValue,
            TaxCodes = ['H'] // Only tax code H
        };

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(reqFiltered);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();
        result.Response.ShouldNotBeNull();

        var filteredCount = result.Response.Results.Count();
        filteredCount.ShouldBeLessThanOrEqualTo(allCount);

        _testOutputHelper.WriteLine($"All records: {allCount}, Tax code H filtered: {filteredCount}");
    }

    [Fact]
    [Description("PS-1902 : Distributions filtering by combined states and tax codes")]
    public async Task GetDistributionsAndForfeitures_WithStatesAndTaxCodes_FiltersResults()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var req = new DistributionsAndForfeituresRequest()
        {
            Skip = 0,
            Take = byte.MaxValue,
            States = ["MA", "CT"],
            TaxCodes = ['H', '8']
        };

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();
        result.Response.ShouldNotBeNull();

        // Verify state filtering
        var invalidStateRecords = result.Response.Results
            .Where(r => !string.IsNullOrEmpty(r.State) && !new[] { "MA", "CT" }.Contains(r.State))
            .ToList();
        invalidStateRecords.ShouldBeEmpty("All records should have State = MA or CT");

        _testOutputHelper.WriteLine($"Combined filter (MA/CT + H/8) returned {result.Response.Results.Count()} records");
    }

    [Fact]
    [Description("MAIN-2170: Forfeit type indicators - Regular forfeitures have no type")]
    public async Task GetDistributionsAndForfeitures_RegularForfeitures_NoTypeIndicator()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            // Create a test profit detail with regular forfeiture (no special comment type)
            var testDemographic = await ctx.Demographics.FirstOrDefaultAsync();
            if (testDemographic != null)
            {
                var regularForfeiture = new ProfitDetail
                {
                    Ssn = testDemographic.Ssn,
                    ProfitYear = 2025,
                    ProfitYearIteration = 1,
                    DistributionSequence = 1,
                    ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id,
                    Contribution = 0,
                    Earnings = 0,
                    Forfeiture = 100.00m,
                    MonthToDate = 3,
                    YearToDate = 2025,
                    FederalTaxes = 0,
                    StateTaxes = 0,
                    CommentTypeId = null,
                    Remark = "Regular forfeit",
                    YearsOfServiceCredit = 0
                };
                ctx.ProfitDetails.Add(regularForfeiture);
                await ctx.SaveChangesAsync();
            }
        });

        var req = new DistributionsAndForfeituresRequest { Skip = 0, Take = byte.MaxValue };

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();

        var regularForfeitRecord = result.Response.Results.FirstOrDefault(r => r.ForfeitAmount == 100.00m && r.ForfeitType == null);
        regularForfeitRecord.ShouldNotBeNull("Should have regular forfeit with no type indicator");

        _testOutputHelper.WriteLine($"Regular forfeit record found with ForfeitType = null");
    }

    [Fact]
    [Description("MAIN-2170: Forfeit type indicators - Administrative forfeitures marked with 'A'")]
    public async Task GetDistributionsAndForfeitures_AdministrativeForfeitures_MarkedWithA()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var testDemographic = await ctx.Demographics.FirstOrDefaultAsync();
            if (testDemographic != null)
            {
                var adminForfeiture = new ProfitDetail
                {
                    Ssn = testDemographic.Ssn,
                    ProfitYear = 2025,
                    ProfitYearIteration = 1,
                    DistributionSequence = 2,
                    ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id,
                    Contribution = 0,
                    Earnings = 0,
                    Forfeiture = 200.00m,
                    MonthToDate = 3,
                    YearToDate = 2025,
                    FederalTaxes = 0,
                    StateTaxes = 0,
                    CommentTypeId = CommentType.Constants.ForfeitAdministrative.Id,
                    Remark = "ADMINISTRATIVE",
                    YearsOfServiceCredit = 0
                };
                ctx.ProfitDetails.Add(adminForfeiture);
                await ctx.SaveChangesAsync();
            }
        });

        var req = new DistributionsAndForfeituresRequest { Skip = 0, Take = byte.MaxValue };

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();

        var adminForfeitRecord = result.Response.Results.FirstOrDefault(r => r.ForfeitAmount == 200.00m && r.ForfeitType == 'A');
        adminForfeitRecord.ShouldNotBeNull("Should have administrative forfeit marked with 'A'");

        // Verify totals breakdown
        result.ForfeitureAdministrativeTotal.ShouldBeGreaterThanOrEqualTo(200.00m);

        _testOutputHelper.WriteLine($"Administrative forfeit record found with ForfeitType = 'A', Admin Total = {result.ForfeitureAdministrativeTotal}");
    }

    [Fact]
    [Description("MAIN-2170: Forfeit type indicators - Class Action forfeitures marked with 'C'")]
    public async Task GetDistributionsAndForfeitures_ClassActionForfeitures_MarkedWithC()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var testDemographic = await ctx.Demographics.FirstOrDefaultAsync();
            if (testDemographic != null)
            {
                var caForfeiture = new ProfitDetail
                {
                    Ssn = testDemographic.Ssn,
                    ProfitYear = 2025,
                    ProfitYearIteration = 1,
                    DistributionSequence = 3,
                    ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id,
                    Contribution = 0,
                    Earnings = 0,
                    Forfeiture = 300.00m,
                    MonthToDate = 3,
                    YearToDate = 2025,
                    FederalTaxes = 0,
                    StateTaxes = 0,
                    CommentTypeId = CommentType.Constants.ForfeitClassAction.Id,
                    Remark = "FORFEIT CA",
                    YearsOfServiceCredit = 0
                };
                ctx.ProfitDetails.Add(caForfeiture);
                await ctx.SaveChangesAsync();
            }
        });

        var req = new DistributionsAndForfeituresRequest { Skip = 0, Take = byte.MaxValue };

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();

        var caForfeitRecord = result.Response.Results.FirstOrDefault(r => r.ForfeitAmount == 300.00m && r.ForfeitType == 'C');
        caForfeitRecord.ShouldNotBeNull("Should have class action forfeit marked with 'C'");

        // Verify totals breakdown
        result.ForfeitureClassActionTotal.ShouldBeGreaterThanOrEqualTo(300.00m);

        _testOutputHelper.WriteLine($"Class Action forfeit record found with ForfeitType = 'C', CA Total = {result.ForfeitureClassActionTotal}");
    }

    [Fact]
    [Description("MAIN-2170: Forfeit breakdown totals - Sum of all forfeit types equals total")]
    public async Task GetDistributionsAndForfeitures_ForfeitTotals_SumEqualsTotal()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        var req = new DistributionsAndForfeituresRequest { Skip = 0, Take = byte.MaxValue };

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();

        // Verify that sum of breakdown equals total
        var calculatedTotal = result.ForfeitureRegularTotal + result.ForfeitureAdministrativeTotal + result.ForfeitureClassActionTotal;
        calculatedTotal.ShouldBe(result.ForfeitureTotal, 0.01m);

        _testOutputHelper.WriteLine($"Forfeit totals verified: Regular={result.ForfeitureRegularTotal}, Admin={result.ForfeitureAdministrativeTotal}, CA={result.ForfeitureClassActionTotal}, Total={result.ForfeitureTotal}");
    }

    [Fact]
    [Description("MAIN-2170: Forfeit type indicators - Remark-based detection for Administrative")]
    public async Task GetDistributionsAndForfeitures_AdministrativeForfeitures_DetectedByRemark()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var testDemographic = await ctx.Demographics.FirstOrDefaultAsync();
            if (testDemographic != null)
            {
                // Test with remark containing "ADMINISTRATIVE" but no CommentTypeId
                var adminForfeitByRemark = new ProfitDetail
                {
                    Ssn = testDemographic.Ssn,
                    ProfitYear = 2025,
                    ProfitYearIteration = 1,
                    DistributionSequence = 4,
                    ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id,
                    Contribution = 0,
                    Earnings = 0,
                    Forfeiture = 150.00m,
                    MonthToDate = 4,
                    YearToDate = 2025,
                    FederalTaxes = 0,
                    StateTaxes = 0,
                    CommentTypeId = null,
                    Remark = "Legacy ADMINISTRATIVE forfeit",
                    YearsOfServiceCredit = 0
                };
                ctx.ProfitDetails.Add(adminForfeitByRemark);
                await ctx.SaveChangesAsync();
            }
        });

        var req = new DistributionsAndForfeituresRequest { Skip = 0, Take = byte.MaxValue };

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var result = response.Result;
        result.ShouldNotBeNull();

        var adminForfeitRecord = result.Response.Results.FirstOrDefault(r => r.ForfeitAmount == 150.00m && r.ForfeitType == 'A');
        adminForfeitRecord.ShouldNotBeNull("Should detect administrative forfeit from remark field");

        _testOutputHelper.WriteLine($"Administrative forfeit detected from remark: ForfeitType = 'A'");
    }
}
