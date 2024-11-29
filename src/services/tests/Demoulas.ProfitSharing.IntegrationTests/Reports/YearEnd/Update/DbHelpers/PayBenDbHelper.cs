using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;

#pragma warning disable S2933

internal sealed class PayBenDbHelper
{
    public readonly List<PAYBEN1_REC> rows = new();
    private readonly OracleConnection Connection;
    private int pos;

    public PayBenDbHelper(OracleConnection connection)
    {
        Connection = connection;
        loadData();
    }

    public void loadData()
    {
        string query = "SELECT * FROM PROFITSHARE.PAYBEN";
        using (OracleCommand command = new(query, Connection))
        {
            using (OracleDataReader? reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    PAYBEN1_REC record = new()
                    {
                        PYBEN_PSN1 = reader.GetInt64(reader.GetOrdinal("PYBEN_PSN")),
                        PYBEN_PAYSSN1 = reader.GetInt32(reader.GetOrdinal("PYBEN_PAYSSN")),
                        PYBEN_NAME1 = reader.IsDBNull(reader.GetOrdinal("PYBEN_NAME"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PYBEN_NAME")),
                        PYBEN_PSDISB1 = reader.GetDecimal(reader.GetOrdinal("PYBEN_PSDISB")),
                        PYBEN_PSAMT1 = reader.GetDecimal(reader.GetOrdinal("PYBEN_PSAMT")),
                        PYBEN_PROF_EARN1 = reader.GetDecimal(reader.GetOrdinal("PYBEN_PROF_EARN")),
                        PYBEN_PROF_EARN21 = reader.GetDecimal(reader.GetOrdinal("PYBEN_PROF_EARN2"))
                    };
                    rows.Add(record);
                }
            }
        }
    }

    public string Read(PAYBEN1_REC pbrec)
    {
        if (pos < rows.Count)
        {
            PAYBEN1_REC l = rows[pos];
            pbrec.PYBEN_PSN1 = l.PYBEN_PSN1;
            pbrec.PYBEN_PAYSSN1 = l.PYBEN_PAYSSN1;
            pbrec.PYBEN_NAME1 = l.PYBEN_NAME1;
            pbrec.PYBEN_PSDISB1 = l.PYBEN_PSDISB1;
            pbrec.PYBEN_PSAMT1 = l.PYBEN_PSAMT1;
            pbrec.PYBEN_PROF_EARN1 = l.PYBEN_PROF_EARN1;
            pbrec.PYBEN_PROF_EARN21 = l.PYBEN_PROF_EARN21;
            pos++;
            return "00";
        }

        return "NOT FOUND";
    }

    public bool isEOF()
    {
        return pos >= rows.Count;
    }

    public string? findByPSN(PAYBEN_REC pbrec)
    {
        PAYBEN1_REC l = rows.Where(b => b.PYBEN_PSN1 == pbrec.PYBEN_PSN).First();
        pbrec.PYBEN_PSN = l.PYBEN_PSN1;
        pbrec.PYBEN_PAYSSN = l.PYBEN_PAYSSN1;
        pbrec.PYBEN_NAME = l.PYBEN_NAME1;
        pbrec.PYBEN_PSAMT = l.PYBEN_PSAMT1;
        pbrec.PYBEN_PROF_EARN = l.PYBEN_PROF_EARN1;
        pbrec.PYBEN_PROF_EARN2 = l.PYBEN_PROF_EARN21;
        return "00";
    }
}
