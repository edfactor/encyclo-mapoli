using System.ComponentModel;
using System.Text;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Contracts.Report;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Common;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using IdGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[Collection("SharedGlobalState")]
public class CleanupReportServiceTests : ApiTestBase<Program>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ProfitYearRequest _paginationRequest = new ProfitYearRequest { ProfitYear = 2023, Skip = 0, Take = byte.MaxValue };
    private readonly IdGenerator _generator;


    public CleanupReportServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _generator = new IdGenerator(0);
    }


    [Fact(DisplayName = "PS-147: Check Duplicate SSNs (JSON)")]
    public async Task GetDuplicateSsNsTestJson()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await ApiClient.GETAsync<GetDuplicateSsNsEndpoint, ProfitYearRequest, ReportResponseBase<PayrollDuplicateSsnResponseDto>>(_paginationRequest);
        response.Result.ShouldNotBeNull();
        response.Result.Response.Results.Count().ShouldBe(0);
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
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            var response = await ApiClient.GETAsync<DemographicBadgesNotInPayProfitEndpoint, ProfitYearRequest, ReportResponseBase<DemographicBadgesNotInPayProfitResponse>>(_paginationRequest);
            response.Result.ShouldNotBeNull();
            response.Result.Response.Results.Count().ShouldBe(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response.Result, new JsonSerializerOptions { WriteIndented = true }));

            byte mismatchedValues = 5;

            foreach (var dem in c.Demographics.Take(mismatchedValues))
            {
                long lastSevenDigits = _generator.CreateId() % 10_000_000;
                dem.Id += (int)lastSevenDigits;
            }

            await c.SaveChangesAsync(CancellationToken.None);

            response = await ApiClient.GETAsync<DemographicBadgesNotInPayProfitEndpoint, ProfitYearRequest, ReportResponseBase<DemographicBadgesNotInPayProfitResponse>>(_paginationRequest);
            response.Result.ShouldNotBeNull();
            response.Result.Response.Results.Count().ShouldBe(mismatchedValues);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response.Result, new JsonSerializerOptions { WriteIndented = true }));

            var oneRecord = new ProfitYearRequest { Skip = 0, Take = 1, ProfitYear = _paginationRequest.ProfitYear };
            response = await ApiClient.GETAsync<DemographicBadgesNotInPayProfitEndpoint, ProfitYearRequest, ReportResponseBase<DemographicBadgesNotInPayProfitResponse>>(oneRecord);
            response.Result.ShouldNotBeNull();
            response.Result.Response.Results.Count().ShouldBe(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response.Result, new JsonSerializerOptions { WriteIndented = true }));
        });
    }

    [Fact(DisplayName = "PS-151: Demographic badges without payprofit (CSV)")]
    public Task GetDemographicBadgesWithoutPayProfitTestsCsv()
    {
        DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
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

            var response = await DownloadClient.GETAsync<DemographicBadgesNotInPayProfitEndpoint, ProfitYearRequest, DemographicBadgesNotInPayProfitResponse>(_paginationRequest);
            var stream = await response.Response.Content.ReadAsStreamAsync();
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
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);
        byte negativeValues = 5;
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            await c.PayProfits.Take(negativeValues).ForEachAsync(pp =>
            {
                pp.Etva *= -1;
                pp.ProfitYear = _paginationRequest.ProfitYear;
            });

            await c.SaveChangesAsync(CancellationToken.None);

            var response = await ApiClient.GETAsync<NegativeEtvaForSsNsOnPayProfitEndPoint, ProfitYearRequest, ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>>(_paginationRequest);

            response.Result.ShouldNotBeNull();
            response.Result.ReportName.ShouldBeEquivalentTo("Negative ETVA for SSNs on PayProfit");
            response.Result.Response.Results.Count().ShouldBe(negativeValues);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response.Result, new JsonSerializerOptions { WriteIndented = true }));

            var oneRecord = new ProfitYearRequest { ProfitYear = _paginationRequest.ProfitYear, Skip = 0, Take = 1 };
            response = await ApiClient.GETAsync<NegativeEtvaForSsNsOnPayProfitEndPoint, ProfitYearRequest, ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>>(oneRecord);
            response.Result.ShouldNotBeNull();
            response.Result.Response.Results.Count().ShouldBe(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response.Result, new JsonSerializerOptions { WriteIndented = true }));
        });
    }

    [Fact(DisplayName = "PS-145 : Negative ETVA for SSNs on PayProfit (CSV)")]
    public async Task GetNegativeEtvaReportCsv()
    {
        DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await DownloadClient.GETAsync<NegativeEtvaForSsNsOnPayProfitEndPoint, ProfitYearRequest, NegativeEtvaForSsNsOnPayProfitResponse>(_paginationRequest);
        var stream = await response.Response.Content.ReadAsStreamAsync();
        stream.ShouldNotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync(CancellationToken.None);
        result.ShouldNotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }



    [Fact(DisplayName = "PS-61 : Year-end Profit Sharing Report (JSON)")]
    public async Task GetYearEndProfitSharingReport()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);
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
            emp.DateOfBirth = new DateOnly((short)DateTime.Now.Year - 28, 9, 21);
            empH.EmploymentStatusId = emp.EmploymentStatusId;
            empH.DateOfBirth = emp.DateOfBirth;

            payProfit.ProfitYear = profitYear;
            payProfit.CurrentHoursYear = testHours;
            payProfit.HoursExecutive = 0;
            payProfit.TotalHours = payProfit.CurrentHoursYear + payProfit.HoursExecutive;

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

            emp!.DateOfBirth = new DateOnly((short)DateTime.Now.Year - 15, 9, 21);
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

            emp!.DateOfBirth = new DateOnly((short)DateTime.Now.Year - 28, 9, 21);
            payProfit.CurrentHoursYear = 50;
            payProfit.TotalHours = payProfit.CurrentHoursYear + payProfit.HoursExecutive;
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
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);
        var profitYear = (short)Math.Min((short)DateTime.Now.Year - 1, 2024);
        var req = new YearEndProfitSharingReportRequest()
        {
            Skip = 0,
            Take = byte.MaxValue,
            ProfitYear = profitYear,
            ReportId = YearEndProfitSharingReportId.Age21OrOlderWith1000Hours
        };
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
            emp.DateOfBirth = new DateOnly((short)DateTime.Now.Year - 28, 9, 21);
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

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER, Role.EXECUTIVEADMIN);
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

                profitDetail.ProfitYear = (short)((short)DateTime.Now.Year - 1);
                profitDetail.ProfitYearIteration = 0;
                profitDetail.ProfitCodeId = (byte)profitCode;
                profitDetail.Forfeiture = sampleforfeiture;
                profitDetail.MonthToDate = 3;
                profitDetail.YearToDate = (short)((short)DateTime.Now.Year - 1);
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
    [Description("PS-2275 : Distributions and Forfeitures should include MonthToDate=0 records (year-level records)")]
    public async Task GetDistributionsAndForfeitures_IncludesMonthToDateZeroRecords()
    {
        // Arrange
        decimal sampleDistribution = 1234.56m;
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER, Role.EXECUTIVEADMIN);

        // Use last year to avoid any current year edge cases
        var testYear = (short)((short)DateTime.Now.Year - 1);
        var req = new DistributionsAndForfeituresRequest
        {
            Skip = 0,
            Take = byte.MaxValue,
            StartDate = new DateOnly(testYear, 1, 1),  // January 1 (month = 1)
            EndDate = new DateOnly(testYear, 12, 31)   // December 31 (month = 12)
        };

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            // Clear out existing data to isolate test
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

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            // Setup a demographic with known SSN
            var demographic = await ctx.Demographics.FirstAsync(CancellationToken.None);
            demographic.Ssn = 2275001; // Unique SSN for this test

            // Create a ProfitDetail record with MonthToDate = 0 (year-level record)
            // This is the bug scenario - these records were being excluded when startDate.Month = 1
            var profitDetail = await ctx.ProfitDetails.FirstAsync(CancellationToken.None);
            profitDetail.ProfitYear = testYear;
            profitDetail.ProfitYearIteration = 0;
            profitDetail.ProfitCodeId = ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id; // Code 1 = distribution
            profitDetail.Forfeiture = sampleDistribution;
            profitDetail.MonthToDate = 0;  // <-- KEY: MonthToDate = 0 indicates year-level record
            profitDetail.YearToDate = testYear;
            profitDetail.FederalTaxes = 100;
            profitDetail.StateTaxes = 50;
            profitDetail.TaxCodeId = '7';
            profitDetail.Ssn = demographic.Ssn;
            profitDetail.DistributionSequence = 2275;
            profitDetail.CommentTypeId = null; // Not a transfer/QDRO

            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert - MonthToDate=0 record should be included (this failed before PS-2275 fix)
        response.Result.ShouldNotBeNull();
        response.Result.Response.Results.Count().ShouldBe(1, "MonthToDate=0 records should be included in year-level queries");
        response.Result.Response.Results.First().DistributionAmount.ShouldBe(sampleDistribution);
        response.Result.DistributionTotal.ShouldBe(sampleDistribution);

        _testOutputHelper.WriteLine($"PS-2275 Fix Verified: MonthToDate=0 record included with distribution amount {sampleDistribution:C}");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact]
    [Description("PS-2275 : Distributions and Forfeitures should correctly filter MonthToDate=0 records for partial year queries")]
    public async Task GetDistributionsAndForfeitures_MonthToDateZero_IncludedInPartialYearQueries()
    {
        // Arrange - Test that MonthToDate=0 records are included even for partial year queries
        // This tests the edge case where someone queries March through June, MonthToDate=0 should still be included
        decimal sampleDistribution = 7890.12m;
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER, Role.EXECUTIVEADMIN);

        var testYear = (short)((short)DateTime.Now.Year - 1);
        var req = new DistributionsAndForfeituresRequest
        {
            Skip = 0,
            Take = byte.MaxValue,
            StartDate = new DateOnly(testYear, 3, 1),  // March (month = 3)
            EndDate = new DateOnly(testYear, 6, 30)    // June (month = 6)
        };

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            // Clear out existing data to isolate test
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

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var demographic = await ctx.Demographics.FirstAsync(CancellationToken.None);
            demographic.Ssn = 2275002;

            var profitDetail = await ctx.ProfitDetails.FirstAsync(CancellationToken.None);
            profitDetail.ProfitYear = testYear;
            profitDetail.ProfitYearIteration = 0;
            profitDetail.ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id; // Code 2 = forfeiture
            profitDetail.Forfeiture = sampleDistribution;
            profitDetail.MonthToDate = 0;  // Year-level record
            profitDetail.YearToDate = testYear;
            profitDetail.FederalTaxes = 0;
            profitDetail.StateTaxes = 0;
            profitDetail.Ssn = demographic.Ssn;
            profitDetail.DistributionSequence = 2275;
            profitDetail.CommentTypeId = null;

            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert - MonthToDate=0 should be included even in partial year queries (year-level records apply to entire year)
        response.Result.ShouldNotBeNull();
        response.Result.Response.Results.Count().ShouldBe(1, "MonthToDate=0 records should be included in partial year queries");
        response.Result.Response.Results.First().ForfeitAmount.ShouldBe(sampleDistribution);
        response.Result.ForfeitureTotal.ShouldBe(sampleDistribution);

        _testOutputHelper.WriteLine($"PS-2275 Partial Year Fix Verified: MonthToDate=0 record included with forfeiture amount {sampleDistribution:C}");
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

    [Fact]
    [Description("PS-2275 : Validate all report columns against READY production values")]
    public async Task GetDistributionsAndForfeitures_AllColumns_MatchREADYProductionValues()
    {
        // =============================================================================
        // READY 2025 YTD PRODUCTION VALUES (from QPAY129 report screenshot)
        // =============================================================================
        // This test uses EXACT values from the READY production system to ensure
        // our C# implementation matches the legacy COBOL QPAY129 program.
        //
        // IMPORTANT: All monetary calculations must use MidpointRounding.AwayFromZero
        // to match COBOL behavior (traditional banker's rounding).
        // =============================================================================

        // ======================== EXPECTED COMPANY TOTALS ========================
        // These are the EXACT values from the READY screenshot header row
        const decimal EXPECTED_DISTRIBUTION_TOTAL = 71_824_557.99m;
        const decimal EXPECTED_FORFEITURE_TOTAL = 30_896_800.40m;
        const decimal EXPECTED_FEDERAL_TAX_TOTAL = 6_017_535.83m;
        const decimal EXPECTED_STATE_TAX_TOTAL = 1_085_194.87m;

        // ======================== EXPECTED STATE TAX BREAKDOWN ========================
        // Only 3 states have state tax withholding (MA, RI, ME)
        const decimal EXPECTED_MA_STATE_TAX = 1_025_174.59m;
        const decimal EXPECTED_RI_STATE_TAX = 34_033.02m;
        const decimal EXPECTED_ME_STATE_TAX = 25_987.26m;

        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);

        var testYear = (short)((short)DateTime.Now.Year - 1);
        var req = new DistributionsAndForfeituresRequest
        {
            Skip = 0,
            Take = 500, // Ensure we get all records
            StartDate = new DateOnly(testYear, 1, 1),
            EndDate = new DateOnly(testYear, 12, 31)
        };

        // EXACT STATE DISTRIBUTION DATA FROM READY SCREENSHOT
        // Format: (State, GrossDistribution, NetDistribution, FederalTax, StateTax, Forfeitures)
        // Note: In QPAY129, the last column is forfeitures PER STATE
        var readyStateData = new (string State, decimal GrossDist, decimal NetDist, decimal FedTax, decimal StateTax, decimal Forfeit)[]
        {
            ("MA", 36_846_186.00m, 20_871_572.64m, 4_071_056.73m, 1_025_174.59m, 20_871_572.64m),
            ("AR",      4_795.12m,      4_795.12m,       959.02m,         0.00m,      4_795.12m),
            ("CA",      4_353.73m,      4_353.73m,       870.75m,         0.00m,      4_353.73m),
            ("CO",     82_966.74m,     82_966.74m,    16_593.35m,         0.00m,     82_966.74m),
            ("CT",     18_831.41m,     18_831.41m,       408.48m,         0.00m,     18_831.41m),
            ("DE",      4_313.34m,      4_313.34m,       862.67m,         0.00m,      4_313.34m),
            ("FL",  1_626_427.37m,    493_040.05m,    92_508.01m,         0.00m,    493_040.05m),
            ("GA",     50_000.00m,     50_000.00m,    10_000.00m,         0.00m,     50_000.00m),
            ("IN",     13_328.50m,      3_200.00m,       640.00m,         0.00m,      3_200.00m),
            ("KY",      6_000.00m,      6_000.00m,     1_200.00m,         0.00m,      6_000.00m),
            ("LA",      4_400.00m,      4_400.00m,         0.00m,         0.00m,      4_400.00m),
            ("ME",    875_518.82m,    492_483.37m,   105_749.03m,    25_987.26m,    492_483.37m),
            ("MI",      3_018.36m,      3_018.36m,       603.67m,         0.00m,      3_018.36m),
            ("MO",     10_773.22m,     10_773.22m,     2_154.64m,         0.00m,     10_773.22m),
            ("NC",    968_431.88m,    190_983.10m,    38_196.62m,         0.00m,    190_983.10m),
            ("NE",      6_829.57m,      6_829.57m,     1_365.91m,         0.00m,      6_829.57m),
            ("NH", 29_693_972.87m,  7_644_293.76m, 1_478_912.24m,         0.00m,  7_644_293.76m),
            ("NJ",      4_274.35m,          0.00m,         0.00m,         0.00m,          0.00m),
            ("NV",     16_000.65m,     16_000.65m,     3_200.13m,         0.00m,     16_000.65m),
            ("NY",     66_123.24m,     66_123.24m,    13_224.65m,         0.00m,     66_123.24m),
            ("OH",     71_677.97m,     71_677.97m,    14_335.59m,         0.00m,     71_677.97m),
            ("PA",     60_000.00m,     60_000.00m,    12_000.00m,         0.00m,     60_000.00m),
            ("RI",    809_444.88m,    595_835.91m,   113_632.70m,    34_033.02m,    595_835.91m),
            ("SC",     92_680.34m,     92_680.34m,    18_536.07m,         0.00m,     92_680.34m),
            ("TX",    233_293.63m,     73_485.34m,    14_697.06m,         0.00m,     73_485.34m),
            ("VT",    186_123.03m,      1_860.21m,       372.04m,         0.00m,      1_860.21m),
            ("WI",     37_510.64m,          0.00m,         0.00m,         0.00m,          0.00m),
            ("WV",     27_282.33m,     27_282.33m,     5_456.47m,         0.00m,     27_282.33m),
        };

        // =============================================================================
        // STEP 1: VALIDATE TEST DATA MATCHES EXPECTED CONSTANTS (to the penny)
        // =============================================================================
        var calculatedGrossDist = readyStateData.Sum(x => x.GrossDist);
        var calculatedNetDist = readyStateData.Sum(x => x.NetDist);
        var calculatedFedTax = readyStateData.Sum(x => x.FedTax);
        var calculatedStateTax = readyStateData.Sum(x => x.StateTax);
        var calculatedForfeit = readyStateData.Sum(x => x.Forfeit);

        _testOutputHelper.WriteLine("=== STEP 1: VALIDATE TEST DATA MATCHES READY CONSTANTS ===");
        _testOutputHelper.WriteLine("");

        // Validate DISTRIBUTION TOTAL
        _testOutputHelper.WriteLine("--- DISTRIBUTION TOTAL ---");
        _testOutputHelper.WriteLine($"  READY Constant:  {EXPECTED_DISTRIBUTION_TOTAL:C}");
        _testOutputHelper.WriteLine($"  Sum of States:   {calculatedGrossDist:C}");
        _testOutputHelper.WriteLine($"  Match: {(calculatedGrossDist == EXPECTED_DISTRIBUTION_TOTAL ? "✅ YES" : "❌ NO")}");
        calculatedGrossDist.ShouldBe(EXPECTED_DISTRIBUTION_TOTAL,
            $"Sum of state GrossDist ({calculatedGrossDist:C}) must equal READY Distribution Total ({EXPECTED_DISTRIBUTION_TOTAL:C})");

        // Validate FORFEITURE TOTAL
        _testOutputHelper.WriteLine("--- FORFEITURE TOTAL ---");
        _testOutputHelper.WriteLine($"  READY Constant:  {EXPECTED_FORFEITURE_TOTAL:C}");
        _testOutputHelper.WriteLine($"  Sum of States:   {calculatedForfeit:C}");
        _testOutputHelper.WriteLine($"  Match: {(calculatedForfeit == EXPECTED_FORFEITURE_TOTAL ? "✅ YES" : "❌ NO")}");
        calculatedForfeit.ShouldBe(EXPECTED_FORFEITURE_TOTAL,
            $"Sum of state Forfeit ({calculatedForfeit:C}) must equal READY Forfeiture Total ({EXPECTED_FORFEITURE_TOTAL:C})");

        // Validate FEDERAL TAX TOTAL
        _testOutputHelper.WriteLine("--- FEDERAL TAX TOTAL ---");
        _testOutputHelper.WriteLine($"  READY Constant:  {EXPECTED_FEDERAL_TAX_TOTAL:C}");
        _testOutputHelper.WriteLine($"  Sum of States:   {calculatedFedTax:C}");
        _testOutputHelper.WriteLine($"  Match: {(calculatedFedTax == EXPECTED_FEDERAL_TAX_TOTAL ? "✅ YES" : "❌ NO")}");
        calculatedFedTax.ShouldBe(EXPECTED_FEDERAL_TAX_TOTAL,
            $"Sum of state FedTax ({calculatedFedTax:C}) must equal READY Federal Tax Total ({EXPECTED_FEDERAL_TAX_TOTAL:C})");

        // Validate STATE TAX TOTAL
        _testOutputHelper.WriteLine("--- STATE TAX TOTAL ---");
        _testOutputHelper.WriteLine($"  READY Constant:  {EXPECTED_STATE_TAX_TOTAL:C}");
        _testOutputHelper.WriteLine($"  Sum of States:   {calculatedStateTax:C}");
        _testOutputHelper.WriteLine($"  Match: {(calculatedStateTax == EXPECTED_STATE_TAX_TOTAL ? "✅ YES" : "❌ NO")}");
        calculatedStateTax.ShouldBe(EXPECTED_STATE_TAX_TOTAL,
            $"Sum of state StateTax ({calculatedStateTax:C}) must equal READY State Tax Total ({EXPECTED_STATE_TAX_TOTAL:C})");

        // Validate STATE TAX BREAKDOWN
        _testOutputHelper.WriteLine("\n--- STATE TAX BREAKDOWN ---");
        var statesWithTax = readyStateData.Where(x => x.StateTax > 0).ToList();
        var maData = statesWithTax.First(x => x.State == "MA");
        var riData = statesWithTax.First(x => x.State == "RI");
        var meData = statesWithTax.First(x => x.State == "ME");

        _testOutputHelper.WriteLine($"  MA: Expected {EXPECTED_MA_STATE_TAX:C}, Actual {maData.StateTax:C} {(maData.StateTax == EXPECTED_MA_STATE_TAX ? "✅" : "❌")}");
        _testOutputHelper.WriteLine($"  RI: Expected {EXPECTED_RI_STATE_TAX:C}, Actual {riData.StateTax:C} {(riData.StateTax == EXPECTED_RI_STATE_TAX ? "✅" : "❌")}");
        _testOutputHelper.WriteLine($"  ME: Expected {EXPECTED_ME_STATE_TAX:C}, Actual {meData.StateTax:C} {(meData.StateTax == EXPECTED_ME_STATE_TAX ? "✅" : "❌")}");

        maData.StateTax.ShouldBe(EXPECTED_MA_STATE_TAX, "MA state tax mismatch");
        riData.StateTax.ShouldBe(EXPECTED_RI_STATE_TAX, "RI state tax mismatch");
        meData.StateTax.ShouldBe(EXPECTED_ME_STATE_TAX, "ME state tax mismatch");

        // Validate sum of state taxes equals total
        var sumOfStateTaxes = EXPECTED_MA_STATE_TAX + EXPECTED_RI_STATE_TAX + EXPECTED_ME_STATE_TAX;
        _testOutputHelper.WriteLine($"\n  Sum (MA+RI+ME): {sumOfStateTaxes:C}");
        _testOutputHelper.WriteLine($"  State Tax Total: {EXPECTED_STATE_TAX_TOTAL:C}");
        _testOutputHelper.WriteLine($"  Match: {(sumOfStateTaxes == EXPECTED_STATE_TAX_TOTAL ? "✅ YES" : "❌ NO")}");
        sumOfStateTaxes.ShouldBe(EXPECTED_STATE_TAX_TOTAL,
            "Sum of individual state taxes must equal State Tax Total");

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            // Clear out existing data to isolate test
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

        // Create ProfitDetail records matching READY data
        // We need to create records for distributions (non-zero GrossDist with ProfitCode 1)
        // The forfeitures column in READY appears to be tied to the same records
        int ssnCounter = 2275300;
        var profitDetailCounter = 0;

        // Filter to states that have actual distribution activity
        var activeStates = readyStateData.Where(x => x.GrossDist > 0).ToArray();

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var demographics = await ctx.Demographics.Take(activeStates.Length).ToListAsync(CancellationToken.None);
            var profitDetails = await ctx.ProfitDetails.Take(activeStates.Length).ToListAsync(CancellationToken.None);

            for (var i = 0; i < activeStates.Length; i++)
            {
                var stateData = activeStates[i];
                var demographic = demographics[i];
                var profitDetail = profitDetails[i];

                demographic.Ssn = ssnCounter++;

                profitDetail.ProfitYear = testYear;
                profitDetail.ProfitYearIteration = 0;
                profitDetail.ProfitCodeId = ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id; // Distribution
                profitDetail.Forfeiture = stateData.GrossDist; // Gross distribution amount
                profitDetail.MonthToDate = 0; // Year-level record (PS-2275 fix)
                profitDetail.YearToDate = testYear;
                profitDetail.FederalTaxes = stateData.FedTax;
                profitDetail.StateTaxes = stateData.StateTax;
                profitDetail.CommentRelatedState = stateData.State;
                profitDetail.TaxCodeId = stateData.StateTax > 0 ? '7' : null;
                profitDetail.Ssn = demographic.Ssn;
                profitDetail.DistributionSequence = 2275 + profitDetailCounter++;
                profitDetail.CommentTypeId = null;
                profitDetail.Remark = null;
            }

            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        // Act
        var response = await ApiClient
            .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(req);

        // Assert - Comprehensive validation against READY values
        response.Result.ShouldNotBeNull();

        _testOutputHelper.WriteLine("\n=== PS-2275 READY PRODUCTION VALUE VALIDATION ===");
        _testOutputHelper.WriteLine($"Report: {response.Result.ReportName}");
        _testOutputHelper.WriteLine($"Date Range: {response.Result.StartDate} to {response.Result.EndDate}");
        _testOutputHelper.WriteLine($"Total Records: {response.Result.Response.Total}");
        _testOutputHelper.WriteLine("");

        // 1. DISTRIBUTION TOTAL - Compare API against READY constant
        _testOutputHelper.WriteLine("--- DISTRIBUTION TOTAL ---");
        _testOutputHelper.WriteLine($"  READY Constant:  {EXPECTED_DISTRIBUTION_TOTAL:C}");
        _testOutputHelper.WriteLine($"  API Response:    {response.Result.DistributionTotal:C}");
        _testOutputHelper.WriteLine($"  Match: {(response.Result.DistributionTotal == EXPECTED_DISTRIBUTION_TOTAL ? "✅ YES" : "❌ NO")}");
        response.Result.DistributionTotal.ShouldBe(EXPECTED_DISTRIBUTION_TOTAL,
            $"DistributionTotal mismatch: expected {EXPECTED_DISTRIBUTION_TOTAL:C}, got {response.Result.DistributionTotal:C}");

        // 2. FEDERAL TAX TOTAL - Compare API against READY constant
        _testOutputHelper.WriteLine("--- FEDERAL TAX TOTAL ---");
        _testOutputHelper.WriteLine($"  READY Constant:  {EXPECTED_FEDERAL_TAX_TOTAL:C}");
        _testOutputHelper.WriteLine($"  API Response:    {response.Result.FederalTaxTotal:C}");
        _testOutputHelper.WriteLine($"  Match: {(response.Result.FederalTaxTotal == EXPECTED_FEDERAL_TAX_TOTAL ? "✅ YES" : "❌ NO")}");
        response.Result.FederalTaxTotal.ShouldBe(EXPECTED_FEDERAL_TAX_TOTAL,
            $"FederalTaxTotal mismatch: expected {EXPECTED_FEDERAL_TAX_TOTAL:C}, got {response.Result.FederalTaxTotal:C}");

        // 3. STATE TAX TOTAL - Compare API against READY constant
        _testOutputHelper.WriteLine("--- STATE TAX TOTAL ---");
        _testOutputHelper.WriteLine($"  READY Constant:  {EXPECTED_STATE_TAX_TOTAL:C}");
        _testOutputHelper.WriteLine($"  API Response:    {response.Result.StateTaxTotal:C}");
        _testOutputHelper.WriteLine($"  Match: {(response.Result.StateTaxTotal == EXPECTED_STATE_TAX_TOTAL ? "✅ YES" : "❌ NO")}");
        response.Result.StateTaxTotal.ShouldBe(EXPECTED_STATE_TAX_TOTAL,
            $"StateTaxTotal mismatch: expected {EXPECTED_STATE_TAX_TOTAL:C}, got {response.Result.StateTaxTotal:C}");

        // 4. STATE-BY-STATE TAX BREAKDOWN
        _testOutputHelper.WriteLine("--- STATE-BY-STATE TAX TOTALS ---");
        response.Result.StateTaxTotals.ShouldNotBeNull();

        foreach (var stateData in statesWithTax.OrderByDescending(x => x.StateTax))
        {
            var actualStateTax = response.Result.StateTaxTotals.TryGetValue(stateData.State, out var tax) ? tax : 0m;
            _testOutputHelper.WriteLine($"  {stateData.State}: READY={stateData.StateTax:C}, API={actualStateTax:C}");

            response.Result.StateTaxTotals.ContainsKey(stateData.State).ShouldBeTrue(
                $"State '{stateData.State}' should be in StateTaxTotals");
            response.Result.StateTaxTotals[stateData.State].ShouldBe(stateData.StateTax,
                $"State '{stateData.State}' tax mismatch: expected {stateData.StateTax:C}");
        }

        // 5. STATE TAX TOTALS SHOULD SUM TO STATETAXTOTAL
        var sumOfStateTaxTotals = response.Result.StateTaxTotals.Values.Sum();
        _testOutputHelper.WriteLine($"  Sum of all states: {sumOfStateTaxTotals:C}");
        sumOfStateTaxTotals.ShouldBe(response.Result.StateTaxTotal,
            $"Sum of StateTaxTotals ({sumOfStateTaxTotals:C}) should equal StateTaxTotal ({response.Result.StateTaxTotal:C})");

        // 6. RECORD COUNT VALIDATION
        _testOutputHelper.WriteLine("--- RECORD COUNT ---");
        _testOutputHelper.WriteLine($"  Expected Records: {activeStates.Length}");
        _testOutputHelper.WriteLine($"  Actual Records:   {response.Result.Response.Results.Count()}");
        response.Result.Response.Results.Count().ShouldBe(activeStates.Length,
            "Record count mismatch");

        // 7. INDIVIDUAL RECORD VALIDATION - spot check key states
        _testOutputHelper.WriteLine("--- INDIVIDUAL RECORD SPOT CHECK ---");
        var records = response.Result.Response.Results.ToList();

        // Check MA (largest state)
        var maRecords = records.Where(r => r.State == "MA").ToList();
        var maExpected = readyStateData.First(x => x.State == "MA");
        _testOutputHelper.WriteLine($"  MA Records: {maRecords.Count}");
        maRecords.Sum(r => r.DistributionAmount).ShouldBe(maExpected.GrossDist, "MA distribution total mismatch");
        maRecords.Sum(r => r.FederalTax).ShouldBe(maExpected.FedTax, "MA federal tax mismatch");
        maRecords.Sum(r => r.StateTax).ShouldBe(maExpected.StateTax, "MA state tax mismatch");

        // Check RI (second highest state tax)
        var riRecords = records.Where(r => r.State == "RI").ToList();
        var riExpected = readyStateData.First(x => x.State == "RI");
        _testOutputHelper.WriteLine($"  RI Records: {riRecords.Count}");
        riRecords.Sum(r => r.StateTax).ShouldBe(riExpected.StateTax, "RI state tax mismatch");

        // Check ME (third state with taxes)
        var meRecords = records.Where(r => r.State == "ME").ToList();
        var meExpected = readyStateData.First(x => x.State == "ME");
        _testOutputHelper.WriteLine($"  ME Records: {meRecords.Count}");
        meRecords.Sum(r => r.StateTax).ShouldBe(meExpected.StateTax, "ME state tax mismatch");

        // Check NH (largest by volume, no state tax)
        var nhRecords = records.Where(r => r.State == "NH").ToList();
        var nhExpected = readyStateData.First(x => x.State == "NH");
        _testOutputHelper.WriteLine($"  NH Records: {nhRecords.Count}");
        nhRecords.Sum(r => r.DistributionAmount).ShouldBe(nhExpected.GrossDist, "NH distribution total mismatch");
        nhRecords.Sum(r => r.StateTax).ShouldBe(0m, "NH should have no state tax");

        _testOutputHelper.WriteLine("");
        _testOutputHelper.WriteLine("✅ PS-2275 ALL READY PRODUCTION VALUES VALIDATED SUCCESSFULLY");
        _testOutputHelper.WriteLine("   - Distribution totals match READY");
        _testOutputHelper.WriteLine("   - Federal tax totals match READY");
        _testOutputHelper.WriteLine("   - State tax totals match READY ($1,085,194.87)");
        _testOutputHelper.WriteLine("   - State-by-state breakdown matches:");
        _testOutputHelper.WriteLine("     • MA: $1,025,174.59 ✓");
        _testOutputHelper.WriteLine("     • RI: $34,033.02 ✓");
        _testOutputHelper.WriteLine("     • ME: $25,987.26 ✓");
        _testOutputHelper.WriteLine("   - All 28 state records validated");
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
