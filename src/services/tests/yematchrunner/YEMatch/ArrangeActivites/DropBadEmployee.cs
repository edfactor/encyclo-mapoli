namespace YEMatch;

public class DropBadEmployee : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        // See https://demoulas.atlassian.net/browse/PS-1380
        
        // Looks like a fresh hire in 2025, but has hours for last year.
        // Badge 709264
        await VerifyDelete(1, "delete from demographics where dem_ssn = 700009305");
        await VerifyDelete(1, "delete from payprofit where payprof_ssn = 700009305");

        
        // Rehire in 2025, worked > 1000 in 2024.
        // Badge 707137
        await VerifyDelete(1, "delete from demographics where dem_ssn = 700007178");
        await VerifyDelete(1, "delete from payprofit where payprof_ssn = 700007178");
        await VerifyDelete(2, "delete from profit_detail where PR_DET_S_SEC_NUMBER = 700007178");

        return new Outcome(Name(), Name(), "delete employee ...", OutcomeStatus.Ok, "deleted", null, false);
    }

    private async Task VerifyDelete(int expectedDeletes, string sqlStr)
    {
        int rowsAffected = await RdySql(sqlStr);
        if (rowsAffected != expectedDeletes)
        {
            throw new InvalidOperationException($"delete failed. effected={rowsAffected}/expected={expectedDeletes} sql: " + sqlStr);
        }

        Console.WriteLine($"Deleted {rowsAffected} rows of: {sqlStr}");
    }
}
