using YEMatch.AssertActivities;

namespace YEMatch.ArrangeActivites;

public class TrimTo14Employees : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        const string condition = " not in (select dem_ssn from demographics where py_scod = 'A' and dem_badge < 700100)";

        await VerifyDelete(116395, $"delete from profit_detail where PR_DET_S_SEC_NUMBER {condition}");
        await VerifyDelete(10481, $"delete from payprofit where payprof_ssn {condition}");
        await VerifyDelete(10481, $"delete from demographics where dem_ssn {condition}");
        await VerifyDelete(176, "delete from payben");

        return new Outcome(Name(), Name(), "delete from ...", OutcomeStatus.Ok, "deleted", null, false);
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
