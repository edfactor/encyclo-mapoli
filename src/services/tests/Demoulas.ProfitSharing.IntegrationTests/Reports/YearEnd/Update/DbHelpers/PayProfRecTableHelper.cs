using Demoulas.ProfitSharing.Common.Contracts.Services;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;

public class EmployeeDataHelper
{
    public EmployeeDataHelper(OracleConnection connection, IProfitSharingDataContextFactory dbContextFactory, short profitYear)
    {
        loadData(connection);
        /**
         * SMART ETVA Calculation not right

        List<PAYPROF_REC> rows2 = loadData2(dbContextFactory, profitYear);

        if (!rows.SequenceEqual(rows2))
        {
            for (int i = 0; i < rows.Count; i++)
            {
                if (!rows[i].Equals(rows2[i]))
                {
                    throw new IOException("Smart data does not match Ready data!!" Attribute );
                }
            }
        }
         */


    }

    private List<EmployeeFinancials> loadData2(IProfitSharingDataContextFactory dbContextFactory, short profitYear)
    {
        TotalService totalService = new TotalService(null, null);

        var bals = dbContextFactory.UseReadOnlyContext(ctx =>
            totalService.TotalVestingBalance(ctx, (short)(profitYear-1), new DateOnly(2024, 1, 1)) 
                .ToDictionaryAsync(
                    keySelector: p => p.Ssn,
                    elementSelector: p => p)).GetAwaiter().GetResult();


        var pps =  dbContextFactory.UseReadOnlyContext(ctx =>
        {
            return ctx.PayProfits
                .Where(pp => pp.ProfitYear == profitYear - 1) 
                .Include(pp => pp.Demographic)
                .OrderBy(pp => pp.Demographic.EmployeeId)
                .Select(pp => new EmployeeFinancials
                {
                    EmployeeId = pp.Demographic.EmployeeId,
                    Ssn = pp.Demographic.Ssn,
                    EnrolledId = pp.EnrollmentId,
                    YearsInPlan = pp.YearsInPlan,
                    CurrentAmount = 0m,
                    EmployeeTypeId = pp.EmployeeTypeId,
                    PointsEarned = (long)pp.PointsEarned, // PointsEarned should be a whole number
                    Contributions = 0,
                    IncomeForfeiture = 0,
                    Earnings = 0,
                    EtvaAfterVestingRules = 0,
                    EarningsOnEtva = 0,
                    SecondaryEarnings = 0,
                    SecondaryEtvaEarnings = 0
                }).ToListAsync();
        }).GetAwaiter().GetResult();

        foreach (var pp in pps)
        {
            pp.CurrentAmount = bals[pp.Ssn].CurrentBalance;
        }

        return pps;

    }

    public List<EmployeeFinancials> rows { get; set; } = new();


    public void loadData(OracleConnection connection)
    {
        List<EmployeeFinancials> payProfRecords = new();
        string query = "SELECT * FROM PROFITSHARE.PAYPROFIT pp join PROFITSHARE.Demographics d on d.dem_badge = pp.PAYPROF_BADGE order by PAYPROF_BADGE";

        using (OracleCommand command = new(query, connection))
        {
            using (OracleDataReader? reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    EmployeeFinancials record = new()
                    {
                        Name = reader.GetString(reader.GetOrdinal("PY_NAM")),
                        EmployeeId = reader.GetInt64(reader.GetOrdinal("PAYPROF_BADGE")),
                        Ssn = reader.GetInt32(reader.GetOrdinal("PAYPROF_SSN")),
                        EnrolledId = reader.GetInt64(reader.GetOrdinal("PY_PS_ENROLLED")),
                        YearsInPlan = reader.GetInt64(reader.GetOrdinal("PY_PS_YEARS")),
                        CurrentAmount = reader.GetDecimal(reader.GetOrdinal("PY_PS_AMT")),
                        EtvaAfterVestingRules = reader.GetDecimal(reader.GetOrdinal("PY_PS_ETVA")),
                        EmployeeTypeId = reader.GetInt64(reader.GetOrdinal("PY_PROF_NEWEMP")),
                        PointsEarned = reader.GetInt64(reader.GetOrdinal("PY_PROF_POINTS")),

                        EarningsOnEtva = reader.GetDecimal(reader.GetOrdinal("PY_PROF_ETVA")), // Earnings on ETVA

                        Earnings = 0m, // reader.GetDecimal(reader.GetOrdinal("PY_PROF_EARN")),
                        SecondaryEarnings = 0m, // reader.GetDecimal(reader.GetOrdinal("PY_PROF_EARN2")),
                        SecondaryEtvaEarnings = 0m, //reader.GetDecimal(reader.GetOrdinal("PY_PROF_ETVA2"))
                    };
                    rows.Add(record);
                }
            }
        }
    }

    public EmployeeFinancials findByBadge(long payprofBadge)
    {
        return rows.Where(r => r.EmployeeId == payprofBadge).First();
    }

    public bool HasRecordBySsn(long payprofSsn)
    {
        return rows.Where(r => r.Ssn == payprofSsn).Count() != 0;
    }
}
