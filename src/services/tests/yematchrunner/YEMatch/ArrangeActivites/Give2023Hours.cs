using YEMatch.AssertActivities;

namespace YEMatch.ArrangeActivites;

/*
 * The profitshare dataset (aka the scramble data) does not have hours for 2023, yet our vesting algorithm (which uses the database) expects the hours to be available for the current year.
 * This activity adjusts the hours for 2023 for people with contributions or V-ONLY so they have the minimum 1000 hours.
 */

public class Give2023Hours
    : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        int cnt = await SmtSql(
            "update pay_profit set current_hours_year = 1000 where profit_year = 2023 and demographic_id in (select d.id from demographic d join profit_detail pd on pd.ssn = d.ssn where profit_year = 2023 and YEARS_OF_SERVICE_CREDIT = 1 )");

        if (cnt == 5)
        {
            throw new InvalidOperationException("Mismatch in number of rows updated");
        }

        return new Outcome(Name(), Name(), "", OutcomeStatus.Ok, "updated hours", null, false);
    }
}
