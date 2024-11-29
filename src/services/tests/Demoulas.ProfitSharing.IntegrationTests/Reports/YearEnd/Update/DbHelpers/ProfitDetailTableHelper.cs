using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;

public class ProfitDetailTableHelper
{
#pragma warning disable S2933

    private List<PROFIT_DETAIL> records = new();
    public long ssn;
    private int pos;
    private readonly OracleConnection connection;

    public ProfitDetailTableHelper(OracleConnection connection)
    {
        this.connection = connection;
    }

    public void loadAllRecordsFor(long ssn)
    {
        records = new List<PROFIT_DETAIL>();
        pos = 0;
        this.ssn = ssn;

        string query = $"SELECT * FROM PROFITSHARE.profit_detail where PR_DET_S_SEC_NUMBER = {ssn}";
        using (OracleCommand command = new(query, connection))
        {
            using (OracleDataReader? reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    PROFIT_DETAIL record = new()
                    {
                        PROFIT_YEAR = reader.GetDecimal(reader.GetOrdinal("PROFIT_YEAR")),
                        PROFIT_CLIENT = reader.GetInt64(reader.GetOrdinal("PROFIT_CLIENT")),
                        PROFIT_CODE = reader.IsDBNull(reader.GetOrdinal("PROFIT_CODE"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PROFIT_CODE")),
                        PROFIT_CONT = reader.GetDecimal(reader.GetOrdinal("PROFIT_CONT")),
                        PROFIT_EARN = reader.GetDecimal(reader.GetOrdinal("PROFIT_EARN")),
                        PROFIT_FORT = reader.GetDecimal(reader.GetOrdinal("PROFIT_FORT")),
                        PROFIT_MDTE = reader.GetInt64(reader.GetOrdinal("PROFIT_MDTE")),
                        PROFIT_YDTE = reader.GetInt64(reader.GetOrdinal("PROFIT_YDTE")),
                        PROFIT_CMNT = reader.IsDBNull(reader.GetOrdinal("PROFIT_CMNT"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PROFIT_CMNT")),
                        PROFIT_ZEROCONT = reader.IsDBNull(reader.GetOrdinal("PROFIT_ZEROCONT"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PROFIT_ZEROCONT")),
                        PROFIT_FED_TAXES = reader.GetDecimal(reader.GetOrdinal("PROFIT_FED_TAXES")),
                        PROFIT_STATE_TAXES = reader.GetDecimal(reader.GetOrdinal("PROFIT_STATE_TAXES")),
                        PROFIT_TAX_CODE = reader.IsDBNull(reader.GetOrdinal("PROFIT_TAX_CODE"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PROFIT_TAX_CODE"))
                    };
                    records.Add(record);
                }
            }
        }
    }

    public int LoadNextRecord(int ssn, PROFIT_DETAIL profit_detail)
    {
        if (ssn != this.ssn)
        {
            loadAllRecordsFor(ssn);
        }

        if (pos < records.Count)
        {
            PROFIT_DETAIL shared = profit_detail;
            PROFIT_DETAIL l = records[pos];
            shared.PROFIT_YEAR = l.PROFIT_YEAR;
            shared.PROFIT_CLIENT = l.PROFIT_CLIENT;
            shared.PROFIT_CODE = l.PROFIT_CODE;
            shared.PROFIT_CONT = l.PROFIT_CONT;
            shared.PROFIT_EARN = l.PROFIT_EARN;
            shared.PROFIT_FORT = l.PROFIT_FORT;
            shared.PROFIT_MDTE = l.PROFIT_MDTE;
            shared.PROFIT_YDTE = l.PROFIT_YDTE;
            shared.PROFIT_CMNT = l.PROFIT_CMNT;
            shared.PROFIT_ZEROCONT = l.PROFIT_ZEROCONT;
            shared.PROFIT_FED_TAXES = l.PROFIT_FED_TAXES;
            shared.PROFIT_STATE_TAXES = l.PROFIT_STATE_TAXES;
            shared.PROFIT_TAX_CODE = l.PROFIT_TAX_CODE;
            pos++;
            return 0;
        }

        return 77;
    }
}
