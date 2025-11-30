using System.Diagnostics.CodeAnalysis;
using Oracle.ManagedDataAccess.Client;
using Shouldly;

#pragma warning disable CS0162 // Unreachable code detected

namespace YEMatch.AssertActivities;

[SuppressMessage("Minor Code Smell", "S1199:Nested code blocks should not be used")]
public class SanityCheckEmployeeAndBenes : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        // Lets ensure the number of employees match    
        {
            int rdy = await RdySql("select count(*) from payprofit");
            int smrt = await SmtSql("select count(*) from pay_profit where profit_year = 2024");
            rdy.ShouldBe(smrt);
        }

        {
            int rdy = await RdySql("select count(*) from payprofit");
            int smrt = await SmtSql("select count(*) from pay_profit where profit_year = 2025");
            rdy.ShouldBe(smrt);
        }

        // Lets ensure the number of benes match    
        {
            int rdy = await RdySql("select count(*) from payben");
            int smrt = await SmtSql("select count(*) from beneficiary");
            rdy.ShouldBe(smrt);
        }

        // Check the rehire guy - we are switching years, so these people have moved along.
        if (false)
        {
            Table? smrtRehire = await SmtQuery(
                """
                select * from (
                        select 'live' system, EMPLOYMENT_STATUS_ID, TO_NUMBER(TO_CHAR(TERMINATION_DATE, 'YYYYMMDD')) TERMINATION_DATE, TERMINATION_CODE_ID, ssn from demographic
                    union  
                        -- This sql joins demographics and demographics_history using
                         -- frozen_state to help select the active history
                        SELECT
                        'frzn',
                        d.EMPLOYMENT_STATUS_ID,
                        TO_NUMBER(TO_CHAR(d.TERMINATION_DATE, 'YYYYMMDD')) TERMINATION_DATE,
                        d.TERMINATION_CODE_ID,
                        demo.ssn
                            FROM DEMOGRAPHIC_HISTORY d
                        INNER JOIN DEMOGRAPHIC demo
                        ON d.DEMOGRAPHIC_ID = demo.ID
                        INNER JOIN FROZEN_STATE f
                        ON f.PROFIT_YEAR = 2024
                        AND f.IS_ACTIVE <> 0
                        AND f.AS_OF_DATETIME >= d.VALID_FROM
                        AND f.AS_OF_DATETIME < d.VALID_TO
                    )
                WHERE SSN in (700007178, 700009305)
                order by system, ssn
                """
            );

            Table? readyRehire = await RdyQuery(
                """
                        select * from (
                            select 'live' system, lower(PY_SCOD) EMPLOYMENT_STATUS_ID, NULLIF(PY_TERM_DT,0) TERMINATION_DATE, NULLIF(PY_TERM,' ') TERMINATION_CODE_ID, dem_ssn SSN from demographics
                            union all
                            select 'frzn', lower(PY_SCOD), NULLIF(PY_TERM_DT,0), NULLIF(PY_TERM,' '), dem_ssn  from demo_profshare
                            )
                        where ssn in (700007178, 700009305)
                        order by system, ssn
                """);
            if (smrtRehire != readyRehire)
            {
                Console.WriteLine($"smart\n{smrtRehire}\n ready\n{readyRehire}\n");
            }

            smrtRehire.ShouldBeEquivalentTo(readyRehire);
        }


        return new Outcome(Name(), Name(), "", OutcomeStatus.Ok, "", null, false);
    }


    public async Task<Table> SmtQuery(string sql)
    {
        await using OracleConnection conn = new(SmartConnString);
        await conn.OpenAsync();
        return await Query(conn, sql);
    }

    public async Task<Table> RdyQuery(string sql)
    {
        await using OracleConnection conn = new(ReadyConnString);
        await conn.OpenAsync();
        return await Query(conn, sql);
    }

    public static async Task<Table> Query(OracleConnection connection, string sql)
    {
        await using OracleCommand? command = connection.CreateCommand();
        command.CommandText = sql;

        await using OracleDataReader? reader = await command.ExecuteReaderAsync();

        List<Row> rows = new();

        while (await reader.ReadAsync())
        {
            Dictionary<string, object?> values = new(reader.FieldCount, StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                object? value = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                values[columnName] = value;
            }

            rows.Add(new Row(values));
        }

        return new Table(rows);
    }
}
