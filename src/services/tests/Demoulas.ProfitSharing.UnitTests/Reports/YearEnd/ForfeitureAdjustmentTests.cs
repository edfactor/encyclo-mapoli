using System.Text.Json;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Helpers;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Api;
using FastEndpoints;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class ForfeitureAdjustmentTests : ApiTestBase<Program>
{
    private readonly ForfeitureAdjustmentClient _client;

    public ForfeitureAdjustmentTests()
    {
        _client = new ForfeitureAdjustmentClient(ApiClient);
    }

    [Fact(DisplayName = "Create Forfeiture Adjustment - Success")]
    public Task CreateForfeitureAdjustmentSuccessTest()
    {
        _client.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        return MockDbContextFactory.UseWritableContext(async ctx =>
        {
            // Setup test data
            int badgeNumber = 700310;
            short profitYear = (short)DateTime.Now.Year;
            decimal forfeitureAmount = 1000m;

            // Find or create a demographic record
            var demographic = await ctx.Demographics.FirstOrDefaultAsync();
            if (demographic == null)
            {
                throw new InvalidOperationException("No demographic data available in test database");
            }

            demographic.BadgeNumber = badgeNumber;
            demographic.StoreNumber = 100;

            // Find or create a payprofit record for this demographic
            var payProfit = await ctx.PayProfits.FirstOrDefaultAsync();
            if (payProfit == null)
            {
                throw new InvalidOperationException("No payprofit data available in test database");
            }

            payProfit.DemographicId = demographic.Id;
            payProfit.ProfitYear = profitYear;

            await ctx.SaveChangesAsync();

            // Create request
            var request = new CreateForfeitureAdjustmentRequest
            {
                BadgeNumber = badgeNumber,
                ClientNumber = demographic.StoreNumber,
                ProfitYear = profitYear,
                ForfeitureAmount = forfeitureAmount,
                StartingBalance = 10,
                NetBalance = 5,
                NetVested = 0
            };

            // Call the endpoint
            var response = await _client.CreateForfeitureAdjustmentAsync(request);

            // Assertions
            response.Should().NotBeNull();
            response.Result.Should().NotBeNull();
            response.Result.BadgeNumber.Should().Be(badgeNumber);
            response.Result.ForfeitureAmount.Should().Be(forfeitureAmount);
            response.Result.StartingBalance.Should().Be(request.StartingBalance);
            response.Result.NetBalance.Should().Be(request.NetBalance);
            response.Result.NetVested.Should().Be(request.NetVested);
        });
    }

    [Fact(DisplayName = "Create Forfeiture Adjustment - Un-Forfeit")]
    public Task CreateForfeitureAdjustmentUnForfeitTest()
    {
        _client.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        return MockDbContextFactory.UseWritableContext(async ctx =>
        {
            // Setup test data
            int badgeNumber = 700310;
            short profitYear = (short)DateTime.Now.Year;
            decimal forfeitureAmount = -500m; // Negative amount for un-forfeit

            // Find or create a demographic record
            var demographic = await ctx.Demographics.FirstOrDefaultAsync();
            if (demographic == null)
            {
                throw new InvalidOperationException("No demographic data available in test database");
            }

            demographic.BadgeNumber = badgeNumber;
            demographic.StoreNumber = 101;

            // Find or create a payprofit record for this demographic
            var payProfit = await ctx.PayProfits.FirstOrDefaultAsync();
            if (payProfit == null)
            {
                throw new InvalidOperationException("No payprofit data available in test database");
            }

            payProfit.DemographicId = demographic.Id;
            payProfit.ProfitYear = profitYear;

            await ctx.SaveChangesAsync();

            // Create request
            var request = new CreateForfeitureAdjustmentRequest
            {
                BadgeNumber = badgeNumber,
                ClientNumber = demographic.StoreNumber,
                ProfitYear = profitYear,
                ForfeitureAmount = forfeitureAmount,
                StartingBalance = 10,
                NetBalance = 5,
                NetVested = 0
            };

            // Call the endpoint
            var response = await _client.CreateForfeitureAdjustmentAsync(request);

            // Assertions
            response.Should().NotBeNull();
            response.Result.Should().NotBeNull();
            response.Result.BadgeNumber.Should().Be(badgeNumber);
            response.Result.ForfeitureAmount.Should().Be(forfeitureAmount);
        });
    }

    [Fact(DisplayName = "Create Forfeiture Adjustment - Employee Not Found")]
    public Task CreateForfeitureAdjustmentEmployeeNotFoundTest()
    {
        _client.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        var nonExistentBadgeNumber = 99999;

        // Create request with non-existent badge number
        var request = new CreateForfeitureAdjustmentRequest
        {
            BadgeNumber = nonExistentBadgeNumber,
            ClientNumber = 100,
            ProfitYear = (short)DateTime.Now.Year,
            ForfeitureAmount = 5,
            StartingBalance = 10,
            NetBalance = 5,
            NetVested = 0
        };

        // Call the endpoint and expect it to fail with Bad Request (400) since service throws ArgumentException
        var response = _client.CreateForfeitureAdjustmentAsync(request);

        return Task.Run(() => response.Result.Response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest));
    }

    [Fact(DisplayName = "Create Forfeiture Adjustment - Zero Amount")]
    public Task CreateForfeitureAdjustmentZeroAmountTest()
    {
        _client.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        return MockDbContextFactory.UseWritableContext(async ctx =>
        {
            // Setup test data
            int badgeNumber = 700310;

            // Find or create a demographic record
            var demographic = await ctx.Demographics.FirstOrDefaultAsync();
            if (demographic == null)
            {
                throw new InvalidOperationException("No demographic data available in test database");
            }

            demographic.BadgeNumber = badgeNumber;
            demographic.StoreNumber = 102;

            await ctx.SaveChangesAsync();

            // Create request with zero amount
            var request = new CreateForfeitureAdjustmentRequest
            {
                BadgeNumber = badgeNumber,
                ClientNumber = demographic.StoreNumber,
                ProfitYear = (short)DateTime.Now.Year,
                ForfeitureAmount = 0, // Zero amount should be rejected
                StartingBalance = 10,
                NetBalance = 5,
                NetVested = 0
            };

            // Call the endpoint and expect it to fail with Bad Request (400)
            var response = await _client.CreateForfeitureAdjustmentAsync(request);

            response.Response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        });
    }

    [Fact(DisplayName = "Create Forfeiture Adjustment - No Profit Sharing Data")]
    public Task CreateForfeitureAdjustmentNoProfitSharingDataTest()
    {
        _client.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        return MockDbContextFactory.UseWritableContext(async ctx =>
        {
            // Setup test data
            int badgeNumber = 700310;
            short profitYear = (short)DateTime.Now.Year;
            short differentYear = (short)(profitYear + 1);

            // Find or create a demographic record
            var demographic = await ctx.Demographics.FirstOrDefaultAsync();
            if (demographic == null)
            {
                throw new InvalidOperationException("No demographic data available in test database");
            }

            demographic.BadgeNumber = badgeNumber;
            demographic.StoreNumber = 103;

            // Find or create a payprofit record for this demographic but for a different year
            var payProfit = await ctx.PayProfits.FirstOrDefaultAsync();
            if (payProfit == null)
            {
                throw new InvalidOperationException("No payprofit data available in test database");
            }

            payProfit.DemographicId = demographic.Id;
            payProfit.ProfitYear = differentYear; // Different year than request

            await ctx.SaveChangesAsync();

            // Create request for a year that doesn't have profit sharing data
            var request = new CreateForfeitureAdjustmentRequest
            {
                BadgeNumber = badgeNumber,
                ClientNumber = demographic.StoreNumber,
                ProfitYear = profitYear, // Different from the payprofit record's year
                ForfeitureAmount = 10,
                StartingBalance = 10,
                NetBalance = 5,
                NetVested = 0
            };

            // Call the endpoint and expect it to fail with Bad Request (400)
            var response = await _client.CreateForfeitureAdjustmentAsync(request);

            response.Response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        });
    }

    [Fact(DisplayName = "Create Forfeiture Adjustment - Missing Required Role")]
    public Task CreateForfeitureAdjustmentWithoutPermission()
    {
        // Use a role that doesn't have permission for this endpoint
        _client.CreateAndAssignTokenForClient(Role.HARDSHIPADMINISTRATOR);

        var request = new CreateForfeitureAdjustmentRequest
        {
            BadgeNumber = 700310,
            ClientNumber = 104,
            ProfitYear = (short)DateTime.Now.Year,
            ForfeitureAmount = 10,
            StartingBalance = 10,
            NetBalance = 5,
            NetVested = 0
        };

        // Call should fail with Forbidden (403)
        var response = _client.CreateForfeitureAdjustmentAsync(request);

        return Task.Run(() => response.Result.Response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden));
    }
}

// Create a client class for the forfeiture adjustment endpoint
public class ForfeitureAdjustmentClient
{
    private readonly HttpClient _httpClient;

    public ForfeitureAdjustmentClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void CreateAndAssignTokenForClient(string role)
    {
        _httpClient.CreateAndAssignTokenForClient(role);
    }

    public Task<TestResult<ForfeitureAdjustmentReportDetail>> CreateForfeitureAdjustmentAsync(
        CreateForfeitureAdjustmentRequest request)
    {
        return _httpClient.POSTAsync<CreateForfeitureAdjustmentEndpoint,
            CreateForfeitureAdjustmentRequest, ForfeitureAdjustmentReportDetail>(request);
    }
}