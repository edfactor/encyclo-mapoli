using YEMatch.YEMatch.AssertActivities;

namespace YEMatch.YEMatch.ArrangeActivites;

// Updates the READY to match the computed values by SMART
// a work around for ensuring the systems calculate the same amount.
public class SmartPay456 : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        await SmtSql(
            $"update pay_profit set PS_CERTIFICATE_ISSUED_DATE = null, EMPLOYEE_TYPE_ID = 0,  points_earned=0, zero_contribution_reason_id = (case when zero_contribution_reason_id < 6 then 0 else zero_contribution_reason_id end)");

        return new Outcome(Name(), Name(), "Cleared cert, newemp, points earned, zerocont", OutcomeStatus.Ok, "", null, false);
    }
}
