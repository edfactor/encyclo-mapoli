namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;


using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

public class ScaffoldFile
{
    String name;
     bool hasRead = false;
    int reads = 0;
    bool eof = false;
    public List<PAYPROF_REC> rows { get; set; } = new();
    public OracleConnection connection { get; set; }

    public ScaffoldFile(string name)
    {
        this.name = name;
    }

    internal bool isEOF()
    {
       return eof;
    }

    internal PAYPROF_REC Read()
    {
        if (!hasRead)
        {
            hasRead = true;
            loadData();
        }

        if (reads < rows.Count)
        {
            var record = rows[reads];
            reads++;
            return record;
        } else{
                Console.WriteLine("Hit EOF");
                eof = true;
                return null;
        }
    }

        public void loadData()
        {
            var payProfRecords = new List<PAYPROF_REC>();
            string query = "SELECT * FROM PAYPROFIT";

            using (var command = new OracleCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var record = new PAYPROF_REC
                        {
                            PAYPROF_BADGE = reader.GetInt64(reader.GetOrdinal("PAYPROF_BADGE")),
                            PAYPROF_SSN = reader.GetInt64(reader.GetOrdinal("PAYPROF_SSN")),
                            PY_PH = reader.GetDecimal(reader.GetOrdinal("PY_PH")),
                            PY_PD = reader.GetDecimal(reader.GetOrdinal("PY_PD")),
                            PY_WEEKS_WORK = reader.GetInt64(reader.GetOrdinal("PY_WEEKS_WORK")),
                            PY_PROF_CERT = reader.IsDBNull(reader.GetOrdinal("PY_PROF_CERT")) ? null : reader.GetString(reader.GetOrdinal("PY_PROF_CERT")),
                            PY_PS_ENROLLED = reader.GetInt64(reader.GetOrdinal("PY_PS_ENROLLED")),
                            PY_PS_YEARS = reader.GetInt64(reader.GetOrdinal("PY_PS_YEARS")),
                            PY_PROF_BENEFICIARY = reader.GetInt64(reader.GetOrdinal("PY_PROF_BENEFICIARY")),
                            PY_PROF_INITIAL_CONT = reader.GetInt64(reader.GetOrdinal("PY_PROF_INITIAL_CONT")),
                            PY_PS_AMT = reader.GetDecimal(reader.GetOrdinal("PY_PS_AMT")),
                            PY_PS_VAMT = reader.GetDecimal(reader.GetOrdinal("PY_PS_VAMT")),
                            PY_PH_LASTYR = reader.GetDecimal(reader.GetOrdinal("PY_PH_LASTYR")),
                            PY_PD_LASTYR = reader.GetDecimal(reader.GetOrdinal("PY_PD_LASTYR")),
                            PY_PROF_NEWEMP = reader.GetInt64(reader.GetOrdinal("PY_PROF_NEWEMP")),
                            PY_PROF_POINTS = reader.GetInt64(reader.GetOrdinal("PY_PROF_POINTS")),
                            PY_PROF_CONT = reader.GetDecimal(reader.GetOrdinal("PY_PROF_CONT")),
                            PY_PROF_FORF = reader.GetDecimal(reader.GetOrdinal("PY_PROF_FORF")),
                            PY_VESTED_FLAG = reader.IsDBNull(reader.GetOrdinal("PY_VESTED_FLAG")) ? null : reader.GetString(reader.GetOrdinal("PY_VESTED_FLAG")),
                            PY_PROF_MAXCONT = reader.GetInt64(reader.GetOrdinal("PY_PROF_MAXCONT")),
                            PY_PROF_ZEROCONT = reader.GetInt64(reader.GetOrdinal("PY_PROF_ZEROCONT")),
                            PY_WEEKS_WORK_LAST = reader.GetInt64(reader.GetOrdinal("PY_WEEKS_WORK_LAST")),
                            PY_PROF_EARN = reader.GetDecimal(reader.GetOrdinal("PY_PROF_EARN")),
                            PY_PS_ETVA = reader.GetDecimal(reader.GetOrdinal("PY_PS_ETVA")),
                            PY_PRIOR_ETVA = reader.GetDecimal(reader.GetOrdinal("PY_PRIOR_ETVA")),
                            PY_PROF_ETVA = reader.GetDecimal(reader.GetOrdinal("PY_PROF_ETVA")),
                            PY_PROF_EARN2 = reader.GetDecimal(reader.GetOrdinal("PY_PROF_EARN2")),
                            PY_PROF_ETVA2 = reader.GetDecimal(reader.GetOrdinal("PY_PROF_ETVA2")),
                            PY_PH_EXEC = reader.GetInt64(reader.GetOrdinal("PY_PH_EXEC")),
                            PY_PD_EXEC = reader.GetDecimal(reader.GetOrdinal("PY_PD_EXEC"))
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
