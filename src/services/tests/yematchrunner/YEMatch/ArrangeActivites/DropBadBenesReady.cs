using YEMatch.YEMatch.Activities;
using YEMatch.YEMatch.AssertActivities;

namespace YEMatch.YEMatch.ArrangeActivites;

/* See https://demoulas.atlassian.net/browse/PS-1268 */

public class DropBadBenesReady : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        if (ActivityFactory.isNewScramble())
        {
            int expectedBenes = 3;
            int expectedDeletes = 79;
 
            await VerifyDelete(expectedBenes, "delete from payben where pyben_payssn in (700010556, 700010521, 700010561) ");
            await VerifyDelete(expectedDeletes, "delete from profit_detail where pr_det_s_sec_number IN (700010556, 700010521, 700010561) ");
        }
        else
        {
            int expectedBenes = 2;
            int expectedDeletes = 63;
            await VerifyDelete(expectedBenes, "delete from payben where pyben_payssn in (700010556, 700010596 )");
            await VerifyDelete(expectedDeletes, "delete from profit_detail where pr_det_s_sec_number IN ( 700010556, 700010596 )");

        }

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
