using Demoulas.ProfitSharing.Common.Contracts.Services;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;

public class PayProfRecTableHelper
{
    public PayProfRecTableHelper(OracleConnection connection, IProfitSharingDataContextFactory dbContextFactory, short profitYear)
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

    private List<PAYPROF_REC> loadData2(IProfitSharingDataContextFactory dbContextFactory, short profitYear)
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
                .Select(pp => new PAYPROF_REC
                {
                    PAYPROF_BADGE = pp.Demographic.EmployeeId,
                    PAYPROF_SSN = pp.Demographic.Ssn,
                    PY_PS_ENROLLED = pp.EnrollmentId,
                    PY_PS_YEARS = pp.YearsInPlan,
                    PY_PS_AMT = 0m,
                    PY_PROF_NEWEMP = pp.EmployeeTypeId,
                    PY_PROF_POINTS = (long)pp.PointsEarned, // PointsEarned should be a whole number
                    PY_PROF_CONT = 0,
                    PY_PROF_FORF = 0,
                    PY_PROF_EARN = 0,
                    PY_PS_ETVA = 0,
                    PY_PROF_ETVA = 0,
                    PY_PROF_EARN2 = 0,
                    PY_PROF_ETVA2 = 0
                }).ToListAsync();
        }).GetAwaiter().GetResult();

        foreach (var pp in pps)
        {
            pp.PY_PS_AMT = bals[pp.PAYPROF_SSN].CurrentBalance;
        }

        return pps;

    }

    public List<PAYPROF_REC> rows { get; set; } = new();


    public void loadData(OracleConnection connection)
    {
        List<PAYPROF_REC> payProfRecords = new();
        string query = "SELECT * FROM PROFITSHARE.PAYPROFIT pp join PROFITSHARE.Demographics d on d.dem_badge = pp.PAYPROF_BADGE order by PAYPROF_BADGE";

        using (OracleCommand command = new(query, connection))
        {
            using (OracleDataReader? reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    PAYPROF_REC record = new()
                    {
                        Name = reader.GetString(reader.GetOrdinal("PY_NAM")),
                        PAYPROF_BADGE = reader.GetInt64(reader.GetOrdinal("PAYPROF_BADGE")),
                        PAYPROF_SSN = reader.GetInt32(reader.GetOrdinal("PAYPROF_SSN")),
                        PY_PS_ENROLLED = reader.GetInt64(reader.GetOrdinal("PY_PS_ENROLLED")),
                        PY_PS_YEARS = reader.GetInt64(reader.GetOrdinal("PY_PS_YEARS")),
                        PY_PS_AMT = reader.GetDecimal(reader.GetOrdinal("PY_PS_AMT")),
                        PY_PS_ETVA = reader.GetDecimal(reader.GetOrdinal("PY_PS_ETVA")),
                        PY_PROF_NEWEMP = reader.GetInt64(reader.GetOrdinal("PY_PROF_NEWEMP")),
                        PY_PROF_POINTS = reader.GetInt64(reader.GetOrdinal("PY_PROF_POINTS")),

                        PY_PROF_ETVA = reader.GetDecimal(reader.GetOrdinal("PY_PROF_ETVA")), // Earnings on ETVA

                        PY_PROF_EARN = 0m, // reader.GetDecimal(reader.GetOrdinal("PY_PROF_EARN")),
                        PY_PROF_EARN2 = 0m, // reader.GetDecimal(reader.GetOrdinal("PY_PROF_EARN2")),
                        PY_PROF_ETVA2 = 0m, //reader.GetDecimal(reader.GetOrdinal("PY_PROF_ETVA2"))
                    };
                    rows.Add(record);
                }
            }
        }
    }

    public PAYPROF_REC findByBadge(long payprofBadge)
    {
        return rows.Where(r => r.PAYPROF_BADGE == payprofBadge).First();
    }

    public bool HasRecordBySsn(long payprofSsn)
    {
        return rows.Where(r => r.PAYPROF_SSN == payprofSsn).Count() != 0;
    }
}
