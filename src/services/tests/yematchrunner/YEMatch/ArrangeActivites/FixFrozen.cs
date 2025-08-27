using YEMatch.YEMatch.AssertActivities;

namespace YEMatch.YEMatch.ArrangeActivites;

public class FixFrozen
    : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        // This employee, 707137 / 700007178, was terminated in 2024, then rehired in 2025.  
        // We adjust the frozen to see that he was terminated
        await RdySql($"update DEMO_PROFSHARE set PY_REHIRE_DT = 0, PY_SCOD = 'T', PY_TERM = 'A', PY_TERM_DT = '20241206' where dem_ssn = 700007178");

        return new Outcome(Name(), Name(), "Ensured REHIRE Employee is Terminated in 2024", OutcomeStatus.Ok, "Ensured REHIRE Employee is Terminated in 2024", null, false);
    }
}
