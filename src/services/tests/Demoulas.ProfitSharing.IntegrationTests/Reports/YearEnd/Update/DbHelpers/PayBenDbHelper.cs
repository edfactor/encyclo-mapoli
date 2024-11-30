using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;


namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;

#pragma warning disable S2933

internal sealed class PayBenDbHelper
{
    public readonly List<PAYBEN1_REC> rows = new();
    public List<PAYBEN1_REC> rows2 = new();

    private IProfitSharingDataContextFactory dbContextFactory;

    public PayBenDbHelper(OracleConnection connection, IProfitSharingDataContextFactory dbContextFactory)
    {
        loadData(connection);
        loadData2(dbContextFactory);

        // Ensure SmartDB Matches ReadyDB
        if (!rows.SequenceEqual(rows2))
        {
            for (int i = 0; i < rows.Count; i++)
            {
                if (!rows[i].Equals(rows2[i]))
                {
                    throw new IOException("Smart data does not match Ready data!!");
                }
            }
        }
    }

    public void loadData(OracleConnection connection)
    {
        string query = "SELECT * FROM PROFITSHARE.PAYBEN order by PYBEN_NAME, PYBEN_PSN desc";
        using (OracleCommand command = new(query, connection))
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
                            : reader.GetString(reader.GetOrdinal("PYBEN_NAME")).Trim(),
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

    public void loadData2(IProfitSharingDataContextFactory dbContextFactory)
    {
        rows2 = dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.Beneficiaries.OrderBy(b=>b.Contact.ContactInfo.FullName).ThenByDescending(b=>b.EmployeeId*10000+b.PsnSuffix).Select(b => new PAYBEN1_REC
                {
                    PYBEN_PSN1 = Convert.ToInt64(b.Psn),
                    PYBEN_PAYSSN1 = b.Contact.Ssn,
                    PYBEN_NAME1 = b.Contact.ContactInfo.FullName,
                    PYBEN_PSDISB1 = b.Distribution,
                    PYBEN_PSAMT1 = b.Amount,
                    PYBEN_PROF_EARN1 = b.Earnings,
                    PYBEN_PROF_EARN21 = b.SecondaryEarnings
                }).ToListAsync()
        ).GetAwaiter().GetResult();
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
