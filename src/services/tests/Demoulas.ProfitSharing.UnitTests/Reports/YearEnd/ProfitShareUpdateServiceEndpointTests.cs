using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class ProfitShareUpdateServiceEndpointTests : ApiTestBase<Program>
{
    private readonly ProfitShareUpdateEndpoint _endpoint;
    private const short ProfitYear = 2024;

    public ProfitShareUpdateServiceEndpointTests()
    {
        IProfitShareUpdateService svc = ServiceProvider?.GetRequiredService<IProfitShareUpdateService>()!;
        ICrossReferenceValidationService crossRefSvc = ServiceProvider?.GetRequiredService<ICrossReferenceValidationService>()!;
        ILogger<ProfitShareUpdateEndpoint> logger = ServiceProvider?.GetRequiredService<ILogger<ProfitShareUpdateEndpoint>>()!;
        _endpoint = new ProfitShareUpdateEndpoint(svc, crossRefSvc, logger);
    }

    [Fact(DisplayName = "Unauthorized")]
    public async Task Unauthorized()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = ProfitYear };

        // Act
        TestResult<ProfitShareUpdateResponse> response =
            await ApiClient
                .GETAsync<ProfitShareUpdateEndpoint,
                    ProfitShareUpdateRequest, ProfitShareUpdateResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BasicQuery()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = ProfitYear };
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        TestResult<ProfitShareUpdateResponse> response =
            await ApiClient
                .GETAsync<ProfitShareUpdateEndpoint,
                    ProfitShareUpdateRequest, ProfitShareUpdateResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }


  
#pragma warning disable AsyncFixer01

    [Fact]
    public async Task Verify_bene_gets_earnings()
    {
        // Arrange
        const decimal currentBalance = 10_000m;
        await SetupBeneficiary(currentBalance);

        ProfitShareUpdateRequest req = new() { ProfitYear = ProfitYear, ContributionPercent = 0, IncomingForfeitPercent = 0, EarningsPercent = 6.7m };

        // ACT
        ProfitShareUpdateResponse response = await _endpoint.GetResponse(req, CancellationToken.None);

        // Assert
        response.ShouldNotBeNull();
    }

    private async Task SetupBeneficiary(decimal currentBalance)
    {
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            List<PayProfit> ppr = await ctx.PayProfits
                .Include(payProfit => payProfit.Demographic!)
                .ThenInclude(demographic => demographic.ContactInfo)
                .Include(p => p.Demographic != null)
                .ToListAsync(CancellationToken.None);

            // This knocks all the employees out of ProfitSharing
            foreach (PayProfit ppi in ppr)
            {
                ppi.ProfitYear = 3000;
                ppi.Demographic!.Ssn = 4;
                ppi.EnrollmentId = Enrollment.Constants.NotEnrolled; /*0*/
            }

            // This knocks all the profit details out of the way
            foreach (var pdx in ctx.ProfitDetails)
            {
                pdx.ProfitYear = 3000;
                pdx.Ssn = 4;
                pdx.YearsOfServiceCredit = 0;
            }

            // Now we move 1 bene back in.
            Beneficiary b = await ctx.Beneficiaries.Include(b => b.Contact).FirstAsync(CancellationToken.None);
            b.Contact!.Ssn = 7;

            var pd = await ctx.ProfitDetails.FirstAsync(CancellationToken.None);
            pd.Ssn = 7;
            pd.ProfitYear = 2000;
            pd.ProfitCode = ProfitCode.Constants.IncomingQdroBeneficiary;
            pd.ProfitCodeId = 6;
            pd.Contribution = currentBalance;
            pd.Earnings = 0;
            pd.Forfeiture = 0;
            pd.CommentRelatedOracleHcmId = 0;

            await ctx.SaveChangesAsync(CancellationToken.None);
        });
    }
}
