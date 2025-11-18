using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

public sealed class EmbeddedSqlService : IEmbeddedSqlService
{
    public IQueryable<ParticipantTotal> GetTotalBalanceAlt(IProfitSharingDbContext ctx, short profitYear)
    {

        FormattableString query = GetTotalBalanceQuery(profitYear);

        return ctx.ParticipantTotals.FromSqlInterpolated(query);
    }

    public IQueryable<ParticipantTotalYear> GetYearsOfServiceAlt(IProfitSharingDbContext ctx, short profitYear, DateOnly asOfDate)
    {
        var query = GetYearsOfServiceQuery(profitYear, asOfDate);
        return ctx.ParticipantTotalYears.FromSqlRaw(query);
    }

    public IQueryable<ParticipantTotalRatio> GetVestingRatioAlt(IProfitSharingDbContext ctx, short profitYear,
        DateOnly asOfDate)
    {

        var query = GetVestingRatioQuery(profitYear, asOfDate);

        return ctx.ParticipantTotalRatios.FromSqlRaw(query);
    }

    public IQueryable<ParticipantTotal> GetTotalComputedEtvaAlt(IProfitSharingDbContext ctx, short profitYear)
    {
        FormattableString query = $@"SELECT
          pd.SSN,
          NVL(SUM(CASE WHEN pd.PROFIT_CODE_ID = {ProfitCode.Constants.IncomingQdroBeneficiary.Id}  THEN pd.CONTRIBUTION ELSE 0 END), 0)
        + NVL(SUM(CASE WHEN pd.PROFIT_CODE_ID = {ProfitCode.Constants.Incoming100PercentVestedEarnings.Id}  THEN pd.EARNINGS     ELSE 0 END), 0)
        - NVL(SUM(CASE WHEN pd.PROFIT_CODE_ID = {ProfitCode.Constants.Outgoing100PercentVestedPayment.Id}  THEN pd.FORFEITURE   ELSE 0 END), 0)
          AS total
        FROM PROFIT_DETAIL pd
        WHERE pd.PROFIT_YEAR <= {profitYear}
        GROUP BY pd.SSN";

        return ctx.ParticipantEvtaTotals.FromSqlInterpolated(query);
    }

    public IQueryable<ParticipantTotalVestingBalance> TotalVestingBalanceAlt(IProfitSharingDbContext ctx,
        short employeeYear, short profitYear, DateOnly asOfDate)
    {
        var totalBalanceQuery = GetTotalBalanceQuery(profitYear);
        var vestingRatioQuery = GetVestingRatioQuery(profitYear, asOfDate);
        var yearsOfServiceQuery = GetYearsOfServiceQuery(employeeYear, asOfDate);

        var query = $@"
SELECT 
    bal.Ssn, 
    CASE 
        WHEN vr.DEMOGRAPHIC_ID IS NULL 
            THEN CASE WHEN vr.BENEFICIARY_CONTACT_ID IS NULL 
                         THEN 0 
                         ELSE vr.BENEFICIARY_CONTACT_ID 
                  END
        ELSE vr.DEMOGRAPHIC_ID 
    END AS ID,
    
    CASE 
        WHEN bal.TOTAL = 0 THEN 0
        ELSE 
            ((bal.total + pdWrap.FORFEITURES - (pdWrap.PROF_6_CONTRIB + pdWrap.PROF_8_EARNINGS - pdWrap.PROF_9_FORFEIT)) * vr.RATIO) 
            + ((pdWrap.PROF_6_CONTRIB + pdWrap.PROF_8_EARNINGS - pdWrap.PROF_9_FORFEIT) - pdWrap.FORFEITURES) 
    END AS VESTEDBALANCE,

    bal.TOTAL AS CURRENTBALANCE,
    yip.YEARS,
    vr.RATIO,
    PROF_5_FORFEIT as ALLOCTOBENE,
    PROF_6_CONTRIB as ALLOCFROMBENE
FROM (
    {totalBalanceQuery}
) bal
LEFT JOIN (
    {vestingRatioQuery}
) vr ON bal.SSN = vr.SSN
LEFT JOIN (
    {yearsOfServiceQuery}
) yip ON bal.SSN  = yip.SSN AND vr.DEMOGRAPHIC_ID = yip.DEMOGRAPHIC_ID
LEFT JOIN (
    SELECT pd.SSN,
           SUM(CASE WHEN pd.PROFIT_CODE_ID IN (1,2,3,5) THEN pd.FORFEITURE ELSE 0 END) AS FORFEITURES,
           SUM(CASE WHEN pd.PROFIT_CODE_ID = 5 THEN pd.FORFEITURE ELSE 0 END) AS PROF_5_FORFEIT,
           SUM(CASE WHEN pd.PROFIT_CODE_ID = 6 THEN pd.CONTRIBUTION ELSE 0 END) AS PROF_6_CONTRIB,
           SUM(CASE WHEN pd.PROFIT_CODE_ID = 8 THEN pd.EARNINGS ELSE 0 END) AS PROF_8_EARNINGS,
           SUM(CASE WHEN pd.PROFIT_CODE_ID = 9 THEN pd.FORFEITURE ELSE 0 END) AS PROF_9_FORFEIT
    FROM PROFIT_DETAIL pd 
    WHERE pd.PROFIT_YEAR  <= {profitYear}
    GROUP BY pd.SSN
) pdWrap ON bal.SSN = pdWrap.SSN
";

        return ctx.ParticipantTotalVestingBalances.FromSqlRaw(query);

    }


    public IQueryable<ProfitShareTotal> GetProfitShareTotals(IProfitSharingDbContext ctx, short profitYear, DateOnly fiscalEndDate,
       short min_hours, DateOnly birthdate_21, CancellationToken cancellationToken)
    {
        string query = @$"/*-----------------------------------------------------------
  Bind variables                                             
    :p_profit_year      – Profit year being reported on       
    :p_fiscal_end_date  – End-of-year date (same value used   
                           when the report is built)          
    :p_birthdate_21     – :p_fiscal_end_date – 21 years       
    :p_min_hours        – ReferenceData.MinimumHoursForContribution()                        
-----------------------------------------------------------*/
WITH balances AS (
    /* 1️⃣  History-to-date balance per participant --------*/
    SELECT bal.ssn, bal.total
    FROM  (
        /* identical text as EmbeddedSqlService.GetTotalBalanceQuery */
        SELECT pd.ssn,
               SUM(CASE WHEN pd.profit_code_id = 0 THEN  pd.contribution ELSE 0 END) +
               SUM(CASE WHEN pd.profit_code_id IN (0,2) THEN pd.earnings     ELSE 0 END) +
               SUM(CASE WHEN pd.profit_code_id = 0 THEN  pd.forfeiture   ELSE 0 END) +
               SUM(CASE WHEN pd.profit_code_id IN (1,3,5)
                         THEN -pd.forfeiture ELSE 0 END) +
               SUM(CASE WHEN pd.profit_code_id = 2
                         THEN -pd.forfeiture ELSE 0 END) +
               (  SUM(CASE WHEN pd.profit_code_id = 6 THEN pd.contribution ELSE 0 END)
                + SUM(CASE WHEN pd.profit_code_id = 8 THEN pd.earnings     ELSE 0 END)
                + SUM(CASE WHEN pd.profit_code_id = 9 THEN -pd.forfeiture  ELSE 0 END) )
               AS total
        FROM   profit_detail pd
        WHERE  pd.profit_year <= {profitYear}
        GROUP  BY pd.ssn
    ) bal
),
employees AS (
    /* 2️⃣  One row per employee / beneficiary for this year */
    SELECT  d.ssn,
            /* same formulas the LINQ uses */
            pp.total_income   AS wages,
            pp.total_hours    AS hours,
            pp.points_earned                                 points_earned,
            d.employment_status_id                          emp_status,
            d.termination_date                              term_date,
            d.date_of_birth                                 dob,
            NVL(bal.total,0)                                balance
    FROM    pay_profit  pp
      JOIN  demographic d         ON d.id = pp.demographic_id
      LEFT  JOIN balances bal     ON bal.ssn = d.ssn
    WHERE   pp.profit_year = {profitYear}
            /* —> add any extra WHERE clauses that ApplyRequestFilters
                   currently injects (store, hours range, age range, etc.) */
)
SELECT
    /* numeric totals --------------------------------------*/
    SUM(wages)                                                      AS wages_total,
    SUM(hours)                                                      AS hours_total,
    SUM(points_earned)                                              AS points_total,

    /* terminated employees prior to fiscal year-end --------*/
    SUM(CASE WHEN emp_status = '{EmploymentStatus.Constants.Terminated}'
              AND term_date      < TO_DATE('{fiscalEndDate.ToString("yyyy-MM-dd")}','YYYY-MM-DD')
             THEN wages ELSE 0 END)                                 AS terminated_wages_total,
    SUM(CASE WHEN emp_status = '{EmploymentStatus.Constants.Terminated}'
              AND term_date      < TO_DATE('{fiscalEndDate.ToString("yyyy-MM-dd")}','YYYY-MM-DD')
             THEN hours ELSE 0 END)                                 AS terminated_hours_total,
    SUM(CASE WHEN emp_status = '{EmploymentStatus.Constants.Terminated}'
            AND term_date      < TO_DATE('{fiscalEndDate.ToString("yyyy-MM-dd")}','YYYY-MM-DD')
                     THEN points_earned ELSE 0 END)                AS terminated_points_total,

    /* head-counts ------------------------------------------*/
    COUNT(*)                                                        AS number_of_employees,
    SUM(CASE WHEN balance = 0
              AND hours  > {min_hours}
             THEN 1 ELSE 0 END)                                     AS number_of_new_employees,
    SUM(CASE WHEN dob > TO_DATE('{birthdate_21.ToString("yyyy-MM-dd")}','YYYY-MM-DD')
             THEN 1 ELSE 0 END)                                     AS number_of_employees_under21
FROM   employees
";


        return ctx.ProfitShareTotals.FromSqlRaw(query);
    }

    public IQueryable<ProfitDetailRollup> GetTransactionsBySsnForProfitYearForOracle(IProfitSharingDbContext ctx, short profitYear)
    {
        FormattableString query = @$"
SELECT pd.SSN Ssn   ,  
       Sum(pd.CONTRIBUTION) TOTAL_CONTRIBUTIONS,
       Sum(pd.EARNINGS ) TOTAL_EARNINGS,
       Sum(CASE pd.PROFIT_CODE_ID WHEN {ProfitCode.Constants.IncomingContributions.Id} THEN pd.FORFEITURE 
                                  WHEN {ProfitCode.Constants.OutgoingForfeitures.Id} THEN -pd.FORFEITURE
                                  ELSE 0
           END) TOTAL_FORFEITURES,
       Sum(CASE WHEN pd.PROFIT_CODE_ID  != {ProfitCode.Constants.IncomingContributions.Id} THEN pd.FORFEITURE ELSE 0 END) TOTAL_PAYMENTS,
       Sum(CASE WHEN pd.PROFIT_CODE_ID IN ({ProfitCode.Constants.OutgoingForfeitures.Id}, {ProfitCode.Constants.OutgoingDirectPayments.Id}, {ProfitCode.Constants.Outgoing100PercentVestedPayment.Id}) THEN -pd.FORFEITURE ELSE 0 END) DISTRIBUTION,
       Sum(CASE pd.PROFIT_CODE_ID WHEN {ProfitCode.Constants.OutgoingXferBeneficiary.Id} THEN -pd.FORFEITURE
                                  WHEN {ProfitCode.Constants.IncomingQdroBeneficiary.Id} THEN pd.CONTRIBUTION 
                                  ELSE 0 END) BENEFICIARY_ALLOCATION,
       Sum(pd.CONTRIBUTION + pd.EARNINGS + CASE WHEN pd.PROFIT_CODE_ID = {ProfitCode.Constants.IncomingContributions.Id} THEN pd.FORFEITURE ELSE -pd.FORFEITURE END) CURRENT_BALANCE,
       Sum(CASE WHEN pd.PROFIT_YEAR_ITERATION = {ProfitDetail.Constants.ProfitYearIterationMilitary} THEN pd.CONTRIBUTION ELSE 0 END) MILITARY_TOTAL,
       Sum(CASE WHEN pd.PROFIT_YEAR_ITERATION = {ProfitDetail.Constants.ProfitYearIterationClassActionFund} THEN pd.EARNINGS ELSE 0 END) CLASS_ACTION_FUND_TOTAL,
       Sum(CASE WHEN (pd.PROFIT_CODE_ID = {ProfitCode.Constants.Outgoing100PercentVestedPayment.Id} AND (pd.COMMENT_TYPE_ID IN ({CommentType.Constants.TransferOut.Id},{CommentType.Constants.QdroOut.Id})) 
                  OR (pd.PROFIT_CODE_ID = {ProfitCode.Constants.OutgoingXferBeneficiary.Id})) THEN pd.FORFEITURE ELSE 0 END) PAID_ALLOCATIONS_TOTAL,
       -- This is the distributions total as used on PAY443, it is not used elsewhere.
       Sum(CASE WHEN pd.PROFIT_CODE_ID IN ({ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id},{ProfitCode.Constants.OutgoingDirectPayments.Id})
                  OR (pd.PROFIT_CODE_ID = {ProfitCode.Constants.Outgoing100PercentVestedPayment.Id} AND (pd.COMMENT_TYPE_ID NOT IN ({CommentType.Constants.TransferOut.Id},{CommentType.Constants.QdroOut.Id}) OR pd.COMMENT_TYPE_ID IS NULL)) THEN pd.FORFEITURE ELSE 0 END) DISTRIBUTIONS_TOTAL,
       Sum(CASE WHEN pd.PROFIT_CODE_ID = {ProfitCode.Constants.IncomingQdroBeneficiary.Id} THEN pd.CONTRIBUTION ELSE 0 END) ALLOCATIONS_TOTAL,
       Sum(CASE WHEN pd.PROFIT_CODE_ID = {ProfitCode.Constants.OutgoingForfeitures.Id} THEN pd.FORFEITURE ELSE 0 END) FORFEITS_TOTAL
FROM PROFIT_DETAIL pd
WHERE pd.PROFIT_YEAR= {profitYear}
GROUP BY pd.SSN";

        return ctx.ProfitDetailRollups.FromSqlInterpolated(query);
    }

    private static FormattableString GetTotalBalanceQuery(short profitYear)
    {
        FormattableString query = @$"
SELECT
    pd.SSN as Ssn,
    --Contributions + Earnings + EtvaForfeitures + Distributions + Forfeitures + VestedEarnings
    --Contributions:
    SUM(CASE WHEN pd.PROFIT_CODE_ID = 0 THEN pd.CONTRIBUTION ELSE 0 END)
    --Earnings:
  + SUM(CASE WHEN pd.PROFIT_CODE_ID IN (0,2) THEN pd.EARNINGS ELSE 0 END)
    --EtvaForfeitures
  + SUM(CASE WHEN pd.PROFIT_CODE_ID = 0 THEN pd.FORFEITURE ELSE 0 END)
    --Distributions
  + SUM(CASE WHEN pd.PROFIT_CODE_ID  IN (1,3,5) THEN pd.FORFEITURE * -1 ELSE 0 END)
    --Forfeitures
  + SUM(CASE WHEN pd.PROFIT_CODE_ID = 2 THEN pd.FORFEITURE * -1 ELSE 0 END)
   --VestedEarnings
  + (
      SUM(CASE WHEN pd.PROFIT_CODE_ID = 6 THEN pd.CONTRIBUTION ELSE 0 END) +
      SUM(CASE WHEN pd.PROFIT_CODE_ID  = 8 THEN pd.EARNINGS  ELSE 0 END) + 
      SUM(CASE WHEN pd.PROFIT_CODE_ID = 9 THEN pd.FORFEITURE * -1 ELSE 0 END)
    ) AS Total
  FROM PROFIT_DETAIL pd
 WHERE pd.PROFIT_YEAR  <= {profitYear}
 GROUP BY pd.SSN
";
        return query;
    }
    private static string GetVestingRatioQuery(short profitYear, DateOnly asOfDate)
    {
        var initialContributionFiveYearsAgo = asOfDate.AddYears(-5).Year;
        var birthDate65 = asOfDate.AddYears(-65);
        var yearsOfCreditQuery = GetYearsOfServiceQuery(profitYear, asOfDate);
        var initialContributionYearQuery = GetInitialContributionYearQuery();

        var query = @$"
SELECT m.SSN,
       m.DEMOGRAPHIC_ID,
       m.BENEFICIARY_CONTACT_ID,
  CASE WHEN m.IS_EMPLOYEE = 0 THEN 1 ELSE --Beneficiaries are always 100% vested
  CASE WHEN
        -- If employee is active and age > 65, then 100%
        (m.termination_date IS NULL OR m.termination_date > TO_DATE('{asOfDate.ToString("yyyy-MM-dd")}', 'YYYY-MM-DD') )
        AND m.initial_contr_year < {initialContributionFiveYearsAgo}
        AND m.date_of_birth < TO_DATE('{birthDate65.ToString("yyyy-MM-dd")}', 'YYYY-MM-DD')
        THEN 1 ELSE              
  CASE WHEN m.ENROLLMENT_ID  IN (3, 4) THEN 1 ELSE --Otherwise, If enrollment has forfeitures, 100%
  CASE WHEN m.IS_EMPLOYEE = 1 AND m.TERMINATION_CODE_ID = 'Z' AND TERMINATION_DATE<  TO_DATE('{asOfDate.ToString("yyyy-MM-dd")}', 'YYYY-MM-DD')  THEN 1 ELSE --Otherwise, If deceased, mark for 100% vested
  CASE WHEN m.ZERO_CONTRIBUTION_REASON_ID = 6 THEN 1 ELSE --Otherwise, If zero contribution reason is 65 or over, first contribution more than 5 years ago, 100% vested
  CASE WHEN m.YEARS_OF_SERVICE < 3 THEN 0 ELSE --Otherwise, If total years (including the present one) is 0, 1, or 2, 0% Vested
  CASE WHEN m.YEARS_OF_SERVICE = 3 THEN .2 ELSE --Otherwise, If total years (including the present one) is 3, 20% Vested
  CASE WHEN m.YEARS_OF_SERVICE = 4 THEN .4 ELSE --Otherwise, If total years (including the present one) is 4, 40% Vested
  CASE WHEN m.YEARS_OF_SERVICE = 5 THEN .6 ELSE --Otherwise, If total years (including the present one) is 5, 60% Vested
  CASE WHEN m.YEARS_OF_SERVICE = 6 THEN .8 ELSE --Otherwise, If total years (including the present one) is 6, 80% Vested
  CASE WHEN m.YEARS_OF_SERVICE > 6 THEN 1 ELSE --Otherwise, If total years (including the present one) is more than 6, 100% Vested
  0 END END END END END END END END END END END AS RATIO
FROM (
    SELECT d.ID AS DEMOGRAPHIC_ID, NULL AS BENEFICIARY_CONTACT_ID, d.SSN, d.DATE_OF_BIRTH, 1 AS IS_EMPLOYEE, d.TERMINATION_DATE, pp.ENROLLMENT_ID, d.TERMINATION_CODE_ID, pp.ZERO_CONTRIBUTION_REASON_ID, INITIAL_CONTR_YEAR, yos.YEARS + CASE WHEN pp.ENROLLMENT_ID = 2 THEN 1 ELSE 0 END AS YEARS_OF_SERVICE
    FROM DEMOGRAPHIC d
    LEFT JOIN PAY_PROFIT pp ON d.ID = pp.DEMOGRAPHIC_ID AND pp.PROFIT_YEAR  = {profitYear}
    LEFT JOIN (
        {yearsOfCreditQuery}
    ) yos ON d.SSN = yos.SSN AND d.ID = yos.DEMOGRAPHIC_ID
    LEFT JOIN (
        {initialContributionYearQuery}
    ) initcontrib on d.ssn = initcontrib.ssn 
    UNION ALL
    SELECT NULL AS DEMOGRAPHIC_ID, bc.ID AS BENEFICIARY_CONTACT_ID, bc.SSN, bc.DATE_OF_BIRTH, 0 AS IS_EMPLOYEE, NULL AS TERMINATION_DATE, NULL AS ENROLLMENT_ID, NULL AS TERMINATION_CODE_ID, NULL AS ZERO_CONTRIBUTION_REASON_ID, 0 AS YEARS_OF_SERVICE, null as INITIAL_CONTR_YEAR
    FROM BENEFICIARY_CONTACT bc
    WHERE bc.SSN NOT IN (SELECT SSN FROM DEMOGRAPHIC)
) m
";
        return query;
    }

    public static string GetYearsOfServiceQuery(short profitYear, DateOnly asOfDate)
    {
        var aged18Date = asOfDate.AddYears(-18);
        string query = @$"
SELECT d.ID AS DEMOGRAPHIC_ID, pd.SSN, SUM(pd.YEARS_OF_SERVICE_CREDIT)
               + CASE WHEN NOT EXISTS (SELECT 1 FROM PROFIT_DETAIL pd0 WHERE pd0.PROFIT_YEAR = {profitYear} AND pd0.PROFIT_CODE_ID = {ProfitCode.Constants.IncomingContributions.Id} AND pd.SSN  = pd0.SSN AND pd0.PROFIT_YEAR_ITERATION = 0)
                  AND ( NVL(MAX(pp.TOTAL_HOURS), 0) >= {ReferenceData.MinimumHoursForContribution()} 
                        AND MAX(d.DATE_OF_BIRTH) <= TO_DATE('{aged18Date.ToString("yyyy-MM-dd")}', 'yyyy-mm-dd'))
               THEN 1 ELSE 0 END
                 AS YEARS
            FROM PROFIT_DETAIL pd
           INNER JOIN DEMOGRAPHIC d ON pd.SSN = d.SSN
       LEFT JOIN PAY_PROFIT pp ON pp.DEMOGRAPHIC_ID = d.ID AND pp.PROFIT_YEAR = {profitYear}
           WHERE pd.PROFIT_YEAR <= {profitYear}
        GROUP BY d.ID, pd.SSN
        /* MAX() is used for pp.TOTAL_HOURS and d.DATE_OF_BIRTH to satisfy Oracle's GROUP BY requirements.
           Since the JOIN on PAY_PROFIT uses DEMOGRAPHIC_ID + PROFIT_YEAR (which form a unique key per year),
           and each DEMOGRAPHIC.ID has exactly one DATE_OF_BIRTH, MAX() will always return a single value
           per group and does not alter the query logic. This approach avoids including these columns in the
           GROUP BY clause, which would incorrectly create separate groups for each distinct value. */
";
        return query;
    }

    public static FormattableString GetInitialContributionYearQuery()
    {
        FormattableString query = @$"
   SELECT SSN, MIN(profit_year) INITIAL_CONTR_YEAR
            FROM PROFIT_DETAIL
            WHERE PROFIT_CODE_ID = 0 /*ProfitCode.Constants.IncomingContributions*/ AND CONTRIBUTION !=0
            GROUP BY SSN";
        return query;
    }

}
