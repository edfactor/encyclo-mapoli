using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

#pragma warning disable S2933

internal sealed class PayBenReader
{
    List<PAYBEN1_REC> benes = new();
    private int pos = -1;
    public OracleConnection Connection { get; set; }

    public void dataload()
    {
        string query = "SELECT * FROM PROFITSHARE.PAYBEN";
        using (var command = new OracleCommand(query, Connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {

                    var record = new PAYBEN1_REC
                    {
                        PYBEN_PSN1 = reader.GetInt64(reader.GetOrdinal("PYBEN_PSN")),
                        PYBEN_PAYSSN1 = reader.GetInt64(reader.GetOrdinal("PYBEN_PAYSSN")),
                        PYBEN_TYPE1 = reader.IsDBNull(reader.GetOrdinal("PYBEN_TYPE"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PYBEN_TYPE")),
                        PYBEN_PERCENT1 = reader.GetInt64(reader.GetOrdinal("PYBEN_PERCENT")),
                        PYBEN_NAME1 = reader.IsDBNull(reader.GetOrdinal("PYBEN_NAME"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PYBEN_NAME")),
                        PYBEN_ADD1 = reader.IsDBNull(reader.GetOrdinal("PYBEN_ADD"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PYBEN_ADD")),
                        PYBEN_CITY1 = reader.IsDBNull(reader.GetOrdinal("PYBEN_CITY"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PYBEN_CITY")),
                        PYBEN_STATE1 = reader.IsDBNull(reader.GetOrdinal("PYBEN_STATE"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PYBEN_STATE")),
                        PYBEN_ZIP1 = reader.GetInt64(reader.GetOrdinal("PYBEN_ZIP")),
                        PYBEN_RELATION1 = reader.IsDBNull(reader.GetOrdinal("PYBEN_RELATION"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PYBEN_RELATION")),
                        PYBEN_DOBIRTH1 = reader.GetInt64(reader.GetOrdinal("PYBEN_DOBIRTH")),
                        PYBEN_PSDISB1 = reader.GetDecimal(reader.GetOrdinal("PYBEN_PSDISB")),
                        PYBEN_PSAMT1 = reader.GetDecimal(reader.GetOrdinal("PYBEN_PSAMT")),
                        PYBEN_PROF_EARN1 = reader.GetDecimal(reader.GetOrdinal("PYBEN_PROF_EARN")),
                        PYBEN_PROF_EARN21 = reader.GetDecimal(reader.GetOrdinal("PYBEN_PROF_EARN2"))

                    };
                    benes.Add(record);
                }
            }
        }

    }

    public string Read(PAYBEN1_REC pbrec)
    {
        if (pos == -1)
        {
            dataload();
            pos = 0;
        }
        if (pos < benes.Count)
        {
            var l = benes[pos];
            pbrec.PYBEN_PSN1 = l.PYBEN_PSN1;
            pbrec.PYBEN_PAYSSN1 = l.PYBEN_PAYSSN1;
            pbrec.PYBEN_TYPE1 = l.PYBEN_TYPE1;
            pbrec.PYBEN_PERCENT1 = l.PYBEN_PERCENT1;
            pbrec.PYBEN_NAME1 = l.PYBEN_NAME1;
            pbrec.PYBEN_ADD1 = l.PYBEN_ADD1;
            pbrec.PYBEN_CITY1 = l.PYBEN_CITY1;
            pbrec.PYBEN_STATE1 = l.PYBEN_STATE1;
            pbrec.PYBEN_ZIP1 = l.PYBEN_ZIP1;
            pbrec.PYBEN_RELATION1 = l.PYBEN_RELATION1;
            pbrec.PYBEN_DOBIRTH1 = l.PYBEN_DOBIRTH1;
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
        return pos >= benes.Count;
    }

    public string? findByPSN(PAYBEN_REC pbrec)
    {
        var l =benes.Where(b => b.PYBEN_PSN1 == pbrec.PYBEN_PSN).First();
        pbrec.PYBEN_PSN = l.PYBEN_PSN1;
        pbrec.PYBEN_PAYSSN = l.PYBEN_PAYSSN1;
        pbrec.PYBEN_TYPE = l.PYBEN_TYPE1;
        pbrec.PYBEN_PERCENT = l.PYBEN_PERCENT1;
        pbrec.PYBEN_NAME = l.PYBEN_NAME1;
        pbrec.PYBEN_ADD = l.PYBEN_ADD1;
        pbrec.PYBEN_CITY = l.PYBEN_CITY1;
        pbrec.PYBEN_STATE = l.PYBEN_STATE1;
        pbrec.PYBEN_ZIP = l.PYBEN_ZIP1;
        pbrec.PYBEN_RELATION = l.PYBEN_RELATION1;
        pbrec.PYBEN_DOBIRTH = l.PYBEN_DOBIRTH1;
        pbrec.PYBEN_PSDISB = l.PYBEN_PSDISB1;
        pbrec.PYBEN_PSAMT = l.PYBEN_PSAMT1;
        pbrec.PYBEN_PROF_EARN = l.PYBEN_PROF_EARN1;
        pbrec.PYBEN_PROF_EARN2 = l.PYBEN_PROF_EARN21;
        return "00";
    }
}
