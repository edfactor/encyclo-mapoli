using YEMatch.YEMatch.AssertActivities;

namespace YEMatch.YEMatch.ArrangeActivites;

// Makes everybody 19 years old.
public class SetDateOfBirthTo19YearsAgo : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        DateTime dob = DateTime.Today.AddYears(-19);
        string dobDate = dob.ToString("yyyy-MM-dd"); // for TO_DATE
        string dobNumber = dob.ToString("yyyyMMdd"); // for NUMBER(8,0)

        int s = await SmtSql($"update demographic set date_of_birth = TO_DATE('{dobDate}', 'YYYY-MM-DD')");
        int r = await RdySql($"update demographics set py_dob = {dobNumber}");

        if (r != s)
        {
            throw new InvalidOperationException("Mismatch in number of rows updated");
        }

        return new Outcome(Name(), Name(), $"Terminated {r} employees on SMART and READY", OutcomeStatus.Ok, "deleted", null, false);
    }
}
