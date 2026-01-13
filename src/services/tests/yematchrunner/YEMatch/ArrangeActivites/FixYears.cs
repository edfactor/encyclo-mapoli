using YEMatch.AssertActivities;

namespace YEMatch.ArrangeActivites;

// Updates the READY Years to match the computed values by SMART
// a work around for ensuring the systems calculate the same amount.
public class FixYears : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        string markdown = """
                          | 700004 | 700000045 | 24 | 23 | 1989.5 |
                          | 700196 | 700000237 | 48 | 57 | Monthly PY_FREQ=2 |
                          | 705931 | 700005972 | 17 | 19 | Monthly PY_FREQ=2 |
                          """;
        Dictionary<string, string> dict = markdown
            .Split('\n')
            .Skip(2)
            .Select(line => line.Split('|').Select(part => part.Trim()).ToArray())
            .Where(parts => parts.Length > 3)
            .ToDictionary(parts => parts[1], parts => parts[4]);

        foreach ((string badge, string years) in dict)
        {
            await RdySql($"update payprofit set PY_PS_YEARS = {years} where payprof_badge = {badge}");
        }

        return new Outcome(Name(), Name(), "Updated YEARS", OutcomeStatus.Ok, "updated Years", null, false);
    }
}
