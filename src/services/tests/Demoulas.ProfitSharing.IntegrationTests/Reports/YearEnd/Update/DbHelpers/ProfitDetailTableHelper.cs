using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;

public class ProfitDetailTableHelper
{
#pragma warning disable S2933
#pragma warning disable IDE0052

    private List<PROFIT_DETAIL> rows = new();
    private List<PROFIT_DETAIL> rows2 = new();
    public long ssn;
    private int pos;
    private readonly OracleConnection connection;
    private IProfitSharingDataContextFactory dbContextFactory;
    private readonly short profitYear;

    public ProfitDetailTableHelper(OracleConnection connection, IProfitSharingDataContextFactory dbContextFactory, short profitYear)
    {
        this.connection = connection;
        this.dbContextFactory = dbContextFactory;
        this.profitYear = profitYear;
    }

    public void loadAllRecordsFor(long ssn)
    {
        rows = new List<PROFIT_DETAIL>();
        pos = 0;
        this.ssn = ssn;

        string query = $"SELECT * FROM PROFITSHARE.profit_detail where PR_DET_S_SEC_NUMBER = {ssn} and profit_year = {profitYear} order by PROFIT_YEAR, PROFIT_MDTE, PROFIT_FED_TAXES";
        using (OracleCommand command = new(query, connection))
        {
            using (OracleDataReader? reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    PROFIT_DETAIL record = new()
                    {
                        PROFIT_YEAR = reader.GetDecimal(reader.GetOrdinal("PROFIT_YEAR")),
                        //PROFIT_CLIENT = reader.GetInt64(reader.GetOrdinal("PROFIT_CLIENT")),
                        PROFIT_CODE = Convert.ToByte(reader.GetString(reader.GetOrdinal("PROFIT_CODE"))),
                        PROFIT_CONT = reader.GetDecimal(reader.GetOrdinal("PROFIT_CONT")),
                        PROFIT_EARN = reader.GetDecimal(reader.GetOrdinal("PROFIT_EARN")),
                        PROFIT_FORT = reader.GetDecimal(reader.GetOrdinal("PROFIT_FORT")),
                        PROFIT_MDTE = reader.GetInt64(reader.GetOrdinal("PROFIT_MDTE")),
                        PROFIT_YDTE = 0, //reader.GetInt64(reader.GetOrdinal("PROFIT_YDTE")),
                        PROFIT_CMNT = reader.IsDBNull(reader.GetOrdinal("PROFIT_CMNT"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PROFIT_CMNT")).Trim(),
                        PROFIT_FED_TAXES = reader.GetDecimal(reader.GetOrdinal("PROFIT_FED_TAXES")),
                        PROFIT_STATE_TAXES = reader.GetDecimal(reader.GetOrdinal("PROFIT_STATE_TAXES")),
                        PROFIT_TAX_CODE = reader.IsDBNull(reader.GetOrdinal("PROFIT_TAX_CODE"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PROFIT_TAX_CODE")).Trim()
                    };
                    rows.Add(record);
                }
            }
        }
    }

    public void loadAllRecordsFor2(long ssn)
    {
        rows2 = dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.ProfitDetails.Where(pd => pd.Ssn == ssn && pd.ProfitYear == profitYear )
                .OrderBy(pd=>pd.ProfitYear).ThenBy(pd=>pd.ProfitYearIteration).ThenBy((pd=>pd.MonthToDate)).ThenBy(pd=>pd.FederalTaxes)
                .Select(pd => new PROFIT_DETAIL
            {
                PROFIT_YEAR = Convert.ToDecimal($"{pd.ProfitYear}.{pd.ProfitYearIteration}"),
                PROFIT_CODE = pd.ProfitCodeId,
                PROFIT_CONT = pd.Contribution,
                PROFIT_EARN = pd.Earnings,
                PROFIT_FORT = pd.Forfeiture,
                PROFIT_MDTE = pd.MonthToDate,
                PROFIT_YDTE = 0 , // pd.YearToDate,
                PROFIT_CMNT = pd.Remark,
                PROFIT_FED_TAXES = pd.FederalTaxes,
                PROFIT_STATE_TAXES = pd.StateTaxes,
                PROFIT_TAX_CODE = pd.TaxCodeId + ""
            }).ToListAsync()
        ).GetAwaiter().GetResult();
    }



    public int LoadNextRecord(int ssn, PROFIT_DETAIL profit_detail)
    {
        if (ssn != this.ssn)
        {
            loadAllRecordsFor(ssn);
            loadAllRecordsFor2(ssn);

            if(!new HashSet<PROFIT_DETAIL>(rows).SetEquals(rows2)){
                
                // Shoot.
                throw new IOException("Smart does not match Ready");
            }
        }

        if (pos < rows.Count)
        {
            PROFIT_DETAIL shared = profit_detail;
            PROFIT_DETAIL l = rows[pos];
            shared.PROFIT_YEAR = l.PROFIT_YEAR;
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

