using YEMatch.YEMatch.AssertActivities;

namespace YEMatch.YEMatch.ArrangeActivites;

public class DropBadEmployee : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        // See https://demoulas.atlassian.net/browse/PS-1380

        // Bad Employee Data, Looks like a fresh hire in 2025, but has hours for last year.
        // Ssn 700009305 / Badge 709264
        await VerifyDelete(1, "delete from demographics where dem_ssn = 700009305");
        await VerifyDelete(1, "delete from payprofit where payprof_ssn = 700009305");
        
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
