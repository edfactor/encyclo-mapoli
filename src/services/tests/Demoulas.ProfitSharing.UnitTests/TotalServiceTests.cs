using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.UnitTests.Base;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Http.Features;
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

    [Fact(DisplayName = "Totals - Should return records")]
    public async Task TotalBalanceShouldReturnCorrectly()
    {
        long demoSsn = 0;
        await _dataContextFactory.UseWritableContext(async ctx =>
        {
            var demoTest = await ctx.Demographics.FirstAsync();
            demoSsn = demoTest.Ssn;
            var pdArray = await ctx.ProfitDetails.Where(x => x.Ssn == demoTest.Ssn).ToArrayAsync();

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitYear = (short)(DateTime.Now.Year - i);
                prof.ProfitCode = ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal;
                prof.ProfitCodeId = ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id;
                prof.Contribution = Convert.ToDecimal(Math.Pow(2, i * 3));
                prof.Earnings = Convert.ToDecimal(Math.Pow(2, i * 3 + 1));
                prof.Forfeiture = Convert.ToDecimal(Math.Pow(2, i * 3 + 2));
                prof.MonthToDate = 0;
                prof.YearToDate = (short)(DateTime.Now.Year - i);
                prof.FederalTaxes = 0.5m;
                prof.StateTaxes = 0.25m;
                prof.IsTransferIn = false;
                prof.IsTransferOut = false;
            }

            await ctx.SaveChangesAsync();

            var testRslt = await _totalService.GetTotalBalanceSet(ctx, new DateOnly(DateTime.Now.Year, 12, 31))
                                        .Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); //Testing where Forfeiture, Contribution and Earnigns are all added
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(32767);

            foreach (var prof in pdArray)
            {
                prof.ProfitCode = ProfitCode.Constants.Outgoing100PercentVestedPayment;
                prof.ProfitCodeId = ProfitCode.Constants.Outgoing100PercentVestedPayment.Id;
            }

            testRslt = await _totalService.GetTotalBalanceSet(ctx, new DateOnly(DateTime.Now.Year, 12, 31))
                                        .Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); //Testing where only forfeitures are added (negatively)
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(-18724);

            foreach (var prof in pdArray)
            {
                prof.ProfitCode = ProfitCode.Constants.IncomingContributions;
                prof.ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id;
            }

            testRslt = await _totalService.GetTotalBalanceSet(ctx, new DateOnly(DateTime.Now.Year, 12, 31))
                                        .Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // Testing where Earnings and forfeitures are added
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(28086);

            testRslt = await _totalService.GetTotalBalanceSet(ctx, new DateOnly(DateTime.Now.Year - 1, 12, 31))
                                        .Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // Testing As of filter
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(28080);
        });

    }

    [Fact(DisplayName = "Total ETVA Tests")]
    public async Task TotalEtvaShouldReturnCorrectly()
    {
        long demoSsn = 0;
        await _dataContextFactory.UseWritableContext(async ctx =>
        {
            var demoTest = await ctx.Demographics.FirstAsync();
            demoSsn = demoTest.Ssn;
            var pdArray = await ctx.ProfitDetails.Where(x => x.Ssn == demoTest.Ssn).ToArrayAsync();

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitYear = (short)(DateTime.Now.Year - i);
                prof.ProfitCode = ProfitCode.Constants.IncomingQdroBeneficiary;
                prof.ProfitCodeId = ProfitCode.Constants.IncomingQdroBeneficiary.Id;
                prof.Contribution = Convert.ToDecimal(Math.Pow(2, i * 3));
                prof.Earnings = Convert.ToDecimal(Math.Pow(2, i * 3 + 1));
                prof.Forfeiture = Convert.ToDecimal(Math.Pow(2, i * 3 + 2));
                prof.MonthToDate = 0;
                prof.YearToDate = (short)(DateTime.Now.Year - i);
                prof.FederalTaxes = 0.5m;
                prof.StateTaxes = 0.25m;
                prof.IsTransferIn = false;
                prof.IsTransferOut = false;
            }

            await ctx.SaveChangesAsync();

            var testRslt = await _totalService.GetTotalEtva(ctx, new DateOnly(DateTime.Now.Year, 12, 31)).Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // Incoming QDRO Beneficiary
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(4681);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.Incoming100PercentVestedEarnings;
                prof.ProfitCodeId = ProfitCode.Constants.Incoming100PercentVestedEarnings.Id;
            }

            await ctx.SaveChangesAsync();

            testRslt = await _totalService.GetTotalEtva(ctx, new DateOnly(DateTime.Now.Year, 12, 31)).Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // Incoming 100% Vested Earnings
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(9362);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.Outgoing100PercentVestedPayment;
                prof.ProfitCodeId = ProfitCode.Constants.Outgoing100PercentVestedPayment.Id;
            }

            await ctx.SaveChangesAsync();

            testRslt = await _totalService.GetTotalEtva(ctx, new DateOnly(DateTime.Now.Year, 12, 31)).Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // Outgoing 100% Vested Earnings
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(18724);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.Outgoing100PercentVestedPayment;
                prof.ProfitCodeId = ProfitCode.Constants.Outgoing100PercentVestedPayment.Id;
            }

            await ctx.SaveChangesAsync();

            testRslt = await _totalService.GetTotalEtva(ctx, new DateOnly(DateTime.Now.Year - 1, 12, 31)).Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // Test as of filter
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(18720);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.IncomingContributions;
                prof.ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id;
            }

            await ctx.SaveChangesAsync();

            testRslt = await _totalService.GetTotalEtva(ctx, new DateOnly(DateTime.Now.Year, 12, 31)).Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // All non-etva records
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(0);
        });
    }

    [Fact(DisplayName = "Total Distribution Tests")]
    public async Task TotalDistributionShouldReturnCorrectly()
    {
        long demoSsn = 0;
        await _dataContextFactory.UseWritableContext(async ctx =>
        {
            var demoTest = await ctx.Demographics.FirstAsync();
            demoSsn = demoTest.Ssn;
            var pdArray = await ctx.ProfitDetails.Where(x => x.Ssn == demoTest.Ssn).ToArrayAsync();

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
                prof.IsTransferIn = false;
                prof.IsTransferOut = false;
            }

            await ctx.SaveChangesAsync();

            var testRslt = await _totalService.GetTotalDistributions(ctx, new DateOnly(DateTime.Now.Year, 12, 31)).Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // Outgoing Partial Withdrawal
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(18724);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.OutgoingForfeitures;
                prof.ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id;
            }

            await ctx.SaveChangesAsync();

            testRslt = await _totalService.GetTotalDistributions(ctx, new DateOnly(DateTime.Now.Year, 12, 31)).Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // Outgoing Forfeitures
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(18724);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.Outgoing100PercentVestedPayment;
                prof.ProfitCodeId = ProfitCode.Constants.Outgoing100PercentVestedPayment.Id;
            }

            await ctx.SaveChangesAsync();

            testRslt = await _totalService.GetTotalDistributions(ctx, new DateOnly(DateTime.Now.Year, 12, 31)).Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // Outgoing 100% Vested Earnings
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(18724);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.Outgoing100PercentVestedPayment;
                prof.ProfitCodeId = ProfitCode.Constants.Outgoing100PercentVestedPayment.Id;
            }

            await ctx.SaveChangesAsync();

            testRslt = await _totalService.GetTotalDistributions(ctx, new DateOnly(DateTime.Now.Year - 1, 12, 31)).Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // Test as of filter
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(18720);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitCode = ProfitCode.Constants.IncomingQdroBeneficiary;
                prof.ProfitCodeId = ProfitCode.Constants.IncomingQdroBeneficiary.Id;
            }

            await ctx.SaveChangesAsync();

            testRslt = await _totalService.GetTotalDistributions(ctx, new DateOnly(DateTime.Now.Year, 12, 31)).Where(x => x.Ssn == demoSsn).ToListAsync();
            testRslt.Should().NotBeNull(); // All non-distributon records
            testRslt.Count.Should().Be(1);
            testRslt[0].Total.Should().Be(0);
        });
    }
}
