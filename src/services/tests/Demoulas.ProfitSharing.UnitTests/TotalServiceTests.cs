using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests;

public class TotalServiceTests : ApiTestBase<Program>
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;

    public TotalServiceTests()
    {
        _dataContextFactory = MockDbContextFactory;
        this._totalService = ServiceProvider?.GetRequiredService<TotalService>()!;
    }

    [Fact(DisplayName = "Total ETVA Tests")]
    public Task TotalEtvaShouldReturnCorrectly()
    {
        return _dataContextFactory.UseWritableContext(async ctx =>
        {
            var ppTest = await ctx.PayProfits.FirstAsync(CancellationToken.None);
            ppTest.ProfitYear = 2100;
            ppTest.Etva = 4321;
            var etvaResult = await _totalService.GetTotalComputedEtva(ctx, 2100).FirstAsync();
            etvaResult.Total.Should().Be(-20750.98m);
        });
    }

    [Fact(DisplayName = "Total Distribution Tests")]
    public Task TotalDistributionShouldReturnCorrectly()
    {
        long demoSsn = 0;
        return _dataContextFactory.UseWritableContext(async ctx =>
        {
            var demoTest = await ctx.Demographics.FirstAsync(CancellationToken.None);
            demoSsn = demoTest.Ssn;
            var pdArray = await ctx.ProfitDetails.Where(x => x.Ssn == demoTest.Ssn).ToArrayAsync(CancellationToken.None);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitYear = (short)(DateTime.Now.Year - i);
                prof.ProfitCode = ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal;
                prof.ProfitCodeId = ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id;
                prof.Contribution = Convert.ToDecimal(Math.Pow(2, i * 3));
                prof.Earnings = Convert.ToDecimal(Math.Pow(2, (i * 3) + 1));
                prof.Forfeiture = Convert.ToDecimal(Math.Pow(2, (i * 3) + 2));
                prof.MonthToDate = 0;
                prof.YearToDate = (short)(DateTime.Now.Year - i);
                prof.FederalTaxes = 0.5m;
                prof.StateTaxes = 0.25m;
            }

            await ctx.SaveChangesAsync(CancellationToken.None);

            var testRslt = await _totalService.GetTotalDistributions(ctx, (short)DateTime.Now.Year).Where(x => x.Ssn == demoSsn).ToListAsync(CancellationToken.None);
            testRslt.Should().NotBeNull(); // Outgoing Partial Withdrawal
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(18724);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.OutgoingForfeitures;
                prof.ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id;
            }

            await ctx.SaveChangesAsync(CancellationToken.None);

            testRslt = await _totalService.GetTotalDistributions(ctx, (short)DateTime.Now.Year).Where(x => x.Ssn == demoSsn).ToListAsync(CancellationToken.None);
            testRslt.Should().NotBeNull(); // Outgoing Forfeitures
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(18724);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.Outgoing100PercentVestedPayment;
                prof.ProfitCodeId = ProfitCode.Constants.Outgoing100PercentVestedPayment.Id;
            }

            await ctx.SaveChangesAsync(CancellationToken.None);

            testRslt = await _totalService.GetTotalDistributions(ctx, (short)DateTime.Now.Year).Where(x => x.Ssn == demoSsn).ToListAsync(CancellationToken.None);
            testRslt.Should().NotBeNull(); // Outgoing 100% Vested Earnings
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(18724);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.Outgoing100PercentVestedPayment;
                prof.ProfitCodeId = ProfitCode.Constants.Outgoing100PercentVestedPayment.Id;
            }

            await ctx.SaveChangesAsync(CancellationToken.None);

            testRslt = await _totalService.GetTotalDistributions(ctx, (short)(DateTime.Now.Year - 1)).Where(x => x.Ssn == demoSsn)
                .ToListAsync(CancellationToken.None);
            testRslt.Should().NotBeNull(); // Test as of filter
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(18720);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.IncomingQdroBeneficiary;
                prof.ProfitCodeId = ProfitCode.Constants.IncomingQdroBeneficiary.Id;
            }

            await ctx.SaveChangesAsync(CancellationToken.None);

            testRslt = await _totalService.GetTotalDistributions(ctx, (short)DateTime.Now.Year).Where(x => x.Ssn == demoSsn).ToListAsync(CancellationToken.None);
            testRslt.Should().NotBeNull(); // All non-distributon records
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(0);
        });
    }
}
