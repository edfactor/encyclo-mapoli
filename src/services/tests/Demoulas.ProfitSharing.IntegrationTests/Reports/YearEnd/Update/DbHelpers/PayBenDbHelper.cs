using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;


namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;

#pragma warning disable S2933

internal sealed class PayBenDbHelper
{
    public readonly List<BeneficiaryFinancials> rows = new();
    public List<BeneficiaryFinancials> rows2 = new();

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
                    BeneficiaryFinancials record = new()
                    {
                        Psn = reader.GetInt64(reader.GetOrdinal("PYBEN_PSN")),
                        Ssn = reader.GetInt32(reader.GetOrdinal("PYBEN_PAYSSN")),
                        Name = reader.IsDBNull(reader.GetOrdinal("PYBEN_NAME"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PYBEN_NAME")).Trim(),
//                        Distributions = reader.GetDecimal(reader.GetOrdinal("PYBEN_PSDISB")),
                        CurrentAmount = reader.GetDecimal(reader.GetOrdinal("PYBEN_PSAMT")),
                        Earnings = reader.GetDecimal(reader.GetOrdinal("PYBEN_PROF_EARN")),
                        SecondaryEarnings = reader.GetDecimal(reader.GetOrdinal("PYBEN_PROF_EARN2"))
                    };
                    rows.Add(record);
                }
            }
        }
    }

    public void loadData2(IProfitSharingDataContextFactory dbContextFactory)
    {
        rows2 = dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.Beneficiaries.OrderBy(b=>b.Contact.ContactInfo.FullName).ThenByDescending(b=>b.EmployeeId*10000+b.PsnSuffix).Select(b => new BeneficiaryFinancials
            {
                    Psn = Convert.ToInt64(b.Psn),
                    Ssn = b.Contact.Ssn,
                    Name = b.Contact.ContactInfo.FullName,
//                    Distributions = b.Distribution,
                    CurrentAmount = b.Amount,
                    Earnings = b.Earnings,
                    SecondaryEarnings = b.SecondaryEarnings
                }).ToListAsync()
        ).GetAwaiter().GetResult();
    }


    public string? findByPSN(BeneficiaryFinancials pbrec)
    {
        BeneficiaryFinancials l = rows.Where(b => b.Psn == pbrec.Psn).First();
        pbrec.Psn = l.Psn;
        pbrec.Ssn = l.Ssn;
        pbrec.Name = l.Name;
        pbrec.CurrentAmount = l.CurrentAmount;
        pbrec.Earnings = l.Earnings;
        pbrec.SecondaryEarnings = l.SecondaryEarnings;
        return "00";
    }

    public BeneficiaryFinancials findByPSN(long psn)
    {
      return rows.Where(b => b.Psn == psn).First();
    }
}
