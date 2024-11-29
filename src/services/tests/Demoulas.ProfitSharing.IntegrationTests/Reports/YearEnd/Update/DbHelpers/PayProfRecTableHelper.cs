using Elastic.Clients.Elasticsearch.Graph;
using Oracle.ManagedDataAccess.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;

public class PayProfRecTableHelper
{
    public PayProfRecTableHelper(OracleConnection connection)
    {
      
        loadData(connection);
    }

    public List<PAYPROF_REC> rows { get; set; } = new();


    public void loadData(OracleConnection connection)
    {
        List<PAYPROF_REC> payProfRecords = new List<PAYPROF_REC>();
        string query = "SELECT * FROM PROFITSHARE.PAYPROFIT";

        using (OracleCommand command = new OracleCommand(query, connection))
        {
            using (OracleDataReader? reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    PAYPROF_REC record = new PAYPROF_REC
                    {
                        PAYPROF_BADGE = reader.GetInt64(reader.GetOrdinal("PAYPROF_BADGE")),
                        PAYPROF_SSN = reader.GetInt32(reader.GetOrdinal("PAYPROF_SSN")),
                        PY_PS_ENROLLED = reader.GetInt64(reader.GetOrdinal("PY_PS_ENROLLED")),
                        PY_PS_YEARS = reader.GetInt64(reader.GetOrdinal("PY_PS_YEARS")),
                        PY_PS_AMT = reader.GetDecimal(reader.GetOrdinal("PY_PS_AMT")),
                        PY_PROF_NEWEMP = reader.GetInt64(reader.GetOrdinal("PY_PROF_NEWEMP")),
                        PY_PROF_POINTS = reader.GetInt64(reader.GetOrdinal("PY_PROF_POINTS")),
                        PY_PROF_EARN = reader.GetDecimal(reader.GetOrdinal("PY_PROF_EARN")),
                        PY_PS_ETVA = reader.GetDecimal(reader.GetOrdinal("PY_PS_ETVA")),
                        PY_PRIOR_ETVA = reader.GetDecimal(reader.GetOrdinal("PY_PRIOR_ETVA")),
                        PY_PROF_ETVA = reader.GetDecimal(reader.GetOrdinal("PY_PROF_ETVA")),
                        PY_PROF_EARN2 = reader.GetDecimal(reader.GetOrdinal("PY_PROF_EARN2")),
                        PY_PROF_ETVA2 = reader.GetDecimal(reader.GetOrdinal("PY_PROF_ETVA2")),

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
