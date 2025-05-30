using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ExecutiveHoursAndDollars;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class SetExecutiveHoursAndDollarsTests : ApiTestBase<Program>
{
    [Fact]
    public async Task duplicate_badge_is_bad()
    {
        // Arrange
        SetExecutiveHoursAndDollarsRequest request = new SetExecutiveHoursAndDollarsRequest
        {
            ProfitYear = 0,
            ExecutiveHoursAndDollars =
            [
                new() { BadgeNumber = 99, ExecutiveDollars = 0, ExecutiveHours = 0 },
                new() { BadgeNumber = 99, ExecutiveDollars = 0, ExecutiveHours = 0 }
            ]
        };
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response =
            await ApiClient
                .PUTAsync<SetExecutiveHoursAndDollarsEndpoint, SetExecutiveHoursAndDollarsRequest, HttpResponseMessage>(request);
        response.Response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.BadRequest);

        
        response.Response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        var pd = await response.Response.Content.ReadFromJsonAsync<ProblemDetails>();
        pd.Should().NotBeNull();
        pd!.Errors.SelectMany(e=> e.Reason).Should().Contain("Badge Numbers must be unique.");
    }

    [Fact]
    public async Task A_bad_year_should_cause_validation_error()
    {
        // Arrange
        SetExecutiveHoursAndDollarsRequest request = new SetExecutiveHoursAndDollarsRequest
        {
            ProfitYear = 0,
            ExecutiveHoursAndDollars =
            [
                new() { BadgeNumber = 484848, ExecutiveDollars = 444m, ExecutiveHours = 555m }
            ]
        };
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        var response = await ApiClient.PUTAsync<SetExecutiveHoursAndDollarsEndpoint, SetExecutiveHoursAndDollarsRequest, HttpResponseMessage>(request);

        // Assert
        Assert.False(response.Response.IsSuccessStatusCode);

        var pd = await response.Response.Content.ReadFromJsonAsync<ProblemDetails>();
        pd.Should().NotBeNull();
        pd!.Detail?.Should().Contain("Year 0 is not valid.");
    }

    [Fact]
    public async Task update_with_bad_badge()
    {
        // Arrange
        short profitYear = await GetMaxProfitYearAsync(CancellationToken.None);
        SetExecutiveHoursAndDollarsRequest request = new SetExecutiveHoursAndDollarsRequest
        {
            ProfitYear = profitYear,
            ExecutiveHoursAndDollars =
            [
                new() { BadgeNumber = int.MaxValue, ExecutiveDollars = 22, ExecutiveHours = 33 }
            ]
        };
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient
            .PUTAsync<SetExecutiveHoursAndDollarsEndpoint, SetExecutiveHoursAndDollarsRequest, HttpResponseMessage>(request);


        // Assert
        Assert.False(response.Response.IsSuccessStatusCode);

        var pd = await response.Response.Content.ReadFromJsonAsync<ProblemDetails>();
        pd.Should().NotBeNull();
        pd!.Detail?.Should().Contain("One or more badge numbers were not found.");
    }

    [Fact]
    public async Task at_least_one_employee_should_be_provided()
    {
        // Arrange
        short profitYear = await GetMaxProfitYearAsync(CancellationToken.None);
        SetExecutiveHoursAndDollarsRequest request = new SetExecutiveHoursAndDollarsRequest { ProfitYear = profitYear, ExecutiveHoursAndDollars = [] };
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response =
            await ApiClient
                .PUTAsync<SetExecutiveHoursAndDollarsEndpoint, SetExecutiveHoursAndDollarsRequest, HttpResponseMessage>(request);
        response.Response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.BadRequest);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        var pd = await response.Response.Content.ReadFromJsonAsync<ProblemDetails>();
        pd.Should().NotBeNull();
        pd!.Errors.SelectMany(e => e.Reason).Should().Contain("At least one employee must be provided");
    }


    [Fact]
    public Task update_one_employee()
    {
        return MockDbContextFactory.UseWritableContext(async ctx =>
        {
            // Arrange
            short profitYear = await GetMaxProfitYearAsync(CancellationToken.None);

            // Grab an employee
            var payProfit = await ctx.PayProfits
                .Include(p => p.Demographic)
                .Where(p => p.ProfitYear == profitYear)
                .FirstAsync(CancellationToken.None);

            // pull out badge number, create altered hours and dollars 
            var badgeNumber = payProfit.Demographic!.BadgeNumber;
            var newHoursExecutive = payProfit.HoursExecutive + 41;
            var newIncomeExecutive = payProfit.IncomeExecutive + 43;

            // Make the request to change the employee
            SetExecutiveHoursAndDollarsRequest request = new SetExecutiveHoursAndDollarsRequest
            {
                ProfitYear = profitYear,
                ExecutiveHoursAndDollars =
                [
                    new() { BadgeNumber = badgeNumber, ExecutiveDollars = newIncomeExecutive, ExecutiveHours = newHoursExecutive }
                ]
            };
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            var response =
                await ApiClient
                    .PUTAsync<SetExecutiveHoursAndDollarsEndpoint, SetExecutiveHoursAndDollarsRequest, HttpResponseMessage>(request);

            // Assert
            response.Response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NoContent);

            // Verify that the underlying employee was altered properly.
            payProfit = await ctx.PayProfits
                .Include(p => p.Demographic != null)
                .Where(p => p.ProfitYear == profitYear && p.DemographicId == payProfit.DemographicId)
                .FirstAsync(CancellationToken.None);

            // verify updated hours and income
            payProfit.HoursExecutive.Should().Be(newHoursExecutive);
            payProfit.IncomeExecutive.Should().Be(newIncomeExecutive);
        });
    }

    [Fact]
    public Task ensure_bad_badge_stops_updating_of_others()
    {
        return MockDbContextFactory.UseWritableContext(async ctx =>
        {
            // Arrange
            short profitYear = await GetMaxProfitYearAsync(CancellationToken.None);

            // Gather employee
            var demographicsWithPayProfits = await ctx.Demographics
                .Join(ctx.PayProfits,
                    d => d.Id,
                    pp => pp.DemographicId,
                    (d, pp) => new { Demographic = d, PayProfit = pp })
                .Where(joined => joined.PayProfit.ProfitYear == profitYear)
                .FirstAsync(CancellationToken.None);

            // note badge number, keep track of current hours/dollars and attempt to change them
            var badgeNumber = demographicsWithPayProfits.Demographic.BadgeNumber;
            var origExecutiveHoursExecutive = demographicsWithPayProfits.PayProfit.HoursExecutive;
            var origIncomeExecutive = demographicsWithPayProfits.PayProfit.IncomeExecutive;
            var newHoursExecutive = demographicsWithPayProfits.PayProfit.HoursExecutive + 41;
            var newIncomeExecutive = demographicsWithPayProfits.PayProfit.IncomeExecutive + 43;

            // Request with Bad Employee also included
            var request = new SetExecutiveHoursAndDollarsRequest
            {
                ProfitYear = profitYear,
                ExecutiveHoursAndDollars =
                [
                    new() { BadgeNumber = badgeNumber, ExecutiveDollars = newIncomeExecutive, ExecutiveHours = newHoursExecutive },
                    new() { BadgeNumber = int.MaxValue, ExecutiveDollars = 44, ExecutiveHours = 55 }
                ]
            };
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            var response = await ApiClient.PUTAsync<SetExecutiveHoursAndDollarsEndpoint, SetExecutiveHoursAndDollarsRequest, HttpResponseMessage>(request);

            // Assert
            Assert.False(response.Response.IsSuccessStatusCode);

            var pd = await response.Response.Content.ReadFromJsonAsync<ProblemDetails>();
            pd.Should().NotBeNull();
            pd!.Detail?.Should().Contain("One or more badge numbers were not found.");

            // verify no change to existing employee.
            var demographicsWithPayProfitsReloaded = await ctx.Demographics
                .Where(d => d.BadgeNumber == badgeNumber)
                .Join(ctx.PayProfits,
                    d => d.Id,
                    pp => pp.DemographicId,
                    (d, pp) => new { Demographic = d, PayProfit = pp })
                .Where(joined => joined.PayProfit.ProfitYear == profitYear)
                .FirstAsync(CancellationToken.None);

            demographicsWithPayProfitsReloaded.PayProfit.HoursExecutive.Should().Be(origExecutiveHoursExecutive);
            demographicsWithPayProfitsReloaded.PayProfit.IncomeExecutive.Should().Be(origIncomeExecutive);
        });
    }
}
