using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.ServiceDto;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.IntegrationTests;

public class TotalServiceIntegrationTests
{
    // ReSharper disable once NotAccessedField.Local
#pragma warning disable S4487
    private readonly ITestOutputHelper _output;

    private readonly CalendarService calsvc;
    private readonly TotalService totalService;
    private readonly OracleConnection connection;
    private readonly PristineDataContextFactory dataContextFactory;

    public TotalServiceIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("test start");
        var configuration = new ConfigurationBuilder().AddUserSecrets<TotalServiceIntegrationTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        dataContextFactory = new PristineDataContextFactory(connectionString);

        AccountingPeriodsService aps = new AccountingPeriodsService();
        calsvc = new CalendarService(dataContextFactory, aps);
        totalService = new TotalService(dataContextFactory, calsvc);

        // For accessing PROFITSHARE.* tables
        connection = new OracleConnection(connectionString);
    }


    [Fact]
    public async Task Verify_that_NetBalanceLastYear_and_CompanyContributionYears_match_PAYPROFIT()
    {
        var ppReady = await getReadyPayProfitData();
        var ppSmartYis = await GetSmartPayProfitData();
        var ssnToSmartTotals = await GetSmartAmounts();
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
                _ = ppSmartYis[entry.Key] == entry.Value.Years ? yisAgree++ : yisDisagree++;
            }

            if (!ssnToSmartTotals.ContainsKey(entry.Value.Ssn))
            {
                _output.WriteLine($"ssn ={(long)entry.Value.Ssn} missing in Smart TOTALS.    READY has Amt:{entry.Value.Amount} Etv:{entry.Value.Etva}");
            }
            else
            {
                var totals = ssnToSmartTotals[entry.Value.Ssn];
                _ = totals.CurrentBalance == entry.Value.Amount ? netBalAgree++ : netBalDisagree++;
                _ = totals.Etva == entry.Value.Etva ? etvaAgree++ : etvaDisagree++;
            }
        }

        _output.WriteLine($"YIS Disagree count {yisDisagree}   Agree count {yisAgree}");
        _output.WriteLine($"Amt Disagree count {netBalDisagree}   Agree count {netBalAgree}");
        _output.WriteLine($"Etva Disagree count {etvaDisagree}   Agree count {etvaAgree}");
        etvaDisagree.Should().Be(0);
    }

#pragma warning disable AsyncFixer01
    private async Task<Dictionary<int, int>> GetSmartPayProfitData()
    {
        return await dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var ddata = await ctx.PayProfits
                .Include(d => d.Demographic)
                .Where(p => p.ProfitYear == 2023)
                .Join(
                    totalService.GetYearsOfService(ctx, 2023),
                    x => x.Demographic!.Ssn,
                    x => x.Ssn,
                    (p, tot) => new { p.Demographic!.EmployeeId, Years = tot.Years }
                ).ToDictionaryAsync(
                    keySelector: p => p.EmployeeId, // Use EmployeeId as the key
                    elementSelector: p => (int)p.Years // Use Years as the value
                );
            _output.WriteLine($"SMART data count {ddata.Count}");
            return ddata;
        });
    }

    private async Task<Dictionary<int, ParticipantTotalVestingBalanceDto>> GetSmartAmounts()
    {
        return await dataContextFactory.UseReadOnlyContext(ctx =>
            totalService.TotalVestingBalance(ctx, (short)2023, new DateOnly(2024, 1, 1))
                .ToDictionaryAsync(
                    keySelector: p => p.Ssn,
                    elementSelector: p => p)
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

    private async Task<Dictionary<int, PayProfitReady>> getReadyPayProfitData()
    {
        await connection.OpenAsync();

        string query =
            "select payprof_badge, PAYPROF_SSN, PY_PS_YEARS, PY_PS_AMT, PY_PS_ETVA from PROFITSHARE.PAYPROFIT";

        var data = new Dictionary<int, PayProfitReady>();
        var command = new OracleCommand(query, connection);

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
