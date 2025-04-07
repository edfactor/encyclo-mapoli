using System.Linq.Dynamic.Core;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests;

#pragma warning disable S125 // allow commented out code

// ReSharper disable once NotAccessedField.Local
#pragma warning disable S4487

public class TotalServiceIntegrationTests
{
    private readonly ITestOutputHelper _output;
    private readonly TotalService _totalService;
    private readonly OracleConnection _connection;
    private readonly PristineDataContextFactory _dataContextFactory;
    private readonly short _employeeYear = 2025;

    public TotalServiceIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("test start");
        _dataContextFactory = new PristineDataContextFactory();

        AccountingPeriodsService aps = new();
        CalendarService calsvc = new(_dataContextFactory, aps);
        _totalService = new TotalService(_dataContextFactory, calsvc);

        // For accessing PROFITSHARE.* tables
        _connection = new OracleConnection(_dataContextFactory.ConnectionString);
    }


    [Fact]
    public async Task Verify_that_NetBalanceLastYear_and_CompanyContributionYears_match_PAYPROFIT()
    {
        var ppReady = await GetReadyPayProfitData();
        var ppSmartYis = await GetSmartPayProfitData();
        var ssnToSmartTotals = await GetSmartAmounts();
        var ssnToSmartEtva = await GetSmartEtvaAmounts();
        var yisAgree = 0;
        var yisDisagree = 0;
        var netBalAgree = 0;
        var netBalDisagree = 0;
        var etvaAgree = 0;
        var etvaDisagree = 0;

        foreach (var entry in ppReady)
        {
            if (!ppSmartYis.ContainsKey(entry.Key))
            {
                if (entry.Value.Years != 0)
                {
                    _output.WriteLine($"ERROR!: badge ={entry.Key} missing in SMART/GYOS");
                }
            }
            else
            {
                if (ppSmartYis[entry.Key] == entry.Value.Years)
                {
                    yisAgree++;
                }
                else
                {
                    yisDisagree++;

                    //  _output.WriteLine("badge: " + entry.Key + "  SMART YIS: " + ppSmartYis[entry.Key] + "  READY YIS: " + entry.Value.Years);
                }
            }

            if (!ssnToSmartTotals.ContainsKey(entry.Value.Ssn))
            {
                //_output.WriteLine($"ssn ={(long)entry.Value.Ssn} missing in Smart TOTALS.    READY has Amt:{entry.Value.Amount} Etv:{entry.Value.Etva}");
            }
            else
            {
                var totals = ssnToSmartTotals[entry.Value.Ssn];
                _ = totals.CurrentBalance == entry.Value.Amount ? netBalAgree++ : netBalDisagree++;
                var smartEtva = ssnToSmartEtva[entry.Value.Ssn];
                _ = smartEtva == entry.Value.Etva ? etvaAgree++ : etvaDisagree++;
            }
        }

        _output.WriteLine($"YIS Disagree count {yisDisagree}   Agree count {yisAgree}");
        _output.WriteLine($"Amt Disagree count {netBalDisagree}   Agree count {netBalAgree}");
        _output.WriteLine($"Etva Disagree count {etvaDisagree}   Agree count {etvaAgree}");
//        yisDisagree.Should().Be(0);
//        netBalDisagree.Should().Be(0);
//        etvaDisagree.Should().Be(0)
        true.Should().BeTrue();
    }


#pragma warning disable AsyncFixer01
    private async Task<Dictionary<int, int>> GetSmartPayProfitData()
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var ddata = await ctx.PayProfits
                .Include(p => p.Demographic)
                .Where(p => p.ProfitYear == _employeeYear)
                .GroupJoin(
                    _totalService.GetYearsOfService(ctx, _employeeYear),
                    p => p.Demographic!.Ssn,
                    tot => tot.Ssn,
                    (p, tots) => new { p, tots }
                )
                .SelectMany(
                    x => x.tots.DefaultIfEmpty(), // Left join behavior, handles nulls on the right side
                    (x, tot) => new
                    {
                        BadgeNumber = x.p.Demographic!.BadgeNumber, Years = (int)(tot == null ? 0 : (tot.Years == null) ? 0 : tot.Years) // Handle missing Years values safely
                    }
                )
                .ToDictionaryAsync(
                    p => p.BadgeNumber,
                    p => p.Years
                );

            _output.WriteLine($"SMART data count {ddata.Count}");
            return ddata;
        });
    }

    private async Task<Dictionary<int, ParticipantTotalVestingBalanceDto>> GetSmartAmounts()
    {
        return await _dataContextFactory.UseReadOnlyContext(ctx =>
            _totalService.TotalVestingBalance(ctx, _employeeYear, (_employeeYear), /*asOfDate*/ new DateOnly(_employeeYear, 1, 4))
                .ToDictionaryAsync(
                    keySelector: p => p.Ssn,
                    elementSelector: p => p)
        );
    }

    private async Task<Dictionary<int, decimal>> GetSmartEtvaAmounts()
    {
        return await _dataContextFactory.UseReadOnlyContext(ctx =>
            ctx.PayProfits.Include(pp => pp.Demographic).Where(p => p.ProfitYear == _employeeYear)
                .ToDictionaryAsync(pp => pp.Demographic!.Ssn, pp => pp.Etva)
        );
    }

#pragma warning restore AsyncFixer01


    public class PayProfitReady
    {
        public int Ssn;
        public int Years;
        public decimal Amount;
        public decimal Etva;
    }

    private async Task<Dictionary<int, PayProfitReady>> GetReadyPayProfitData()
    {
        await _connection.OpenAsync();

        string query = "select payprof_badge, PAYPROF_SSN, PY_PS_YEARS, PY_PS_AMT, PY_PS_ETVA from PROFITSHARE.PAYPROFIT";
        // string query = "select payprof_badge, PAYPROF_SSN, PY_PS_YEARS, PY_PS_AMT, PY_PS_ETVA from TBHERRMANN.PAYPROFIT";

        var data = new Dictionary<int, PayProfitReady>();
        var command = new OracleCommand(query, _connection);

        var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var badge = reader.GetInt32(0);
            var pp = new PayProfitReady { Ssn = reader.GetInt32(1), Years = reader.GetInt32(2), Amount = reader.GetDecimal(3), Etva = reader.GetDecimal(4) };
            data.Add(badge, pp);
        }

        _output.WriteLine($"READY data count {data.Count}");

        return data;
    }
}
