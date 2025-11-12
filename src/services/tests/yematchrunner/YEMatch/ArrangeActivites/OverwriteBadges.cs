using YEMatch.AssertActivities;

namespace YEMatch.ArrangeActivites;

// Overwirites the badges in both READY and SMART - to ensure we are using the frozen editions, not the live versions
public class OverwriteBadges : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        await SmtSql("CREATE SEQUENCE test_badge_seq START WITH 41 INCREMENT BY 1");
        int s = await SmtSql("update demographic set BADGE_NUMBER = test_badge_seq.nextval");
        await SmtSql("drop SEQUENCE test_badge_seq");

        await RdySql("CREATE SEQUENCE test_badge_seq START WITH 67 INCREMENT BY 1");
        int r = await RdySql("update  demographics set DEM_BADGE = test_badge_seq.nextval");
        await RdySql("drop SEQUENCE test_badge_seq");

        if (r != s)
        {
            throw new InvalidOperationException("Mismatch in number of rows updated");
        }

        return new Outcome(Name(), Name(), $"Terminated {r} employees on SMART and READY", OutcomeStatus.Ok, "deleted", null, false);
    }
}
