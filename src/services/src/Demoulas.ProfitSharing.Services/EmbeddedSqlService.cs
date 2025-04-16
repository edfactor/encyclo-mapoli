using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

public interface IEmbeddedSqlService
{
    IQueryable<ParticipantTotal> GetTotalBalanceAlt(IProfitSharingDbContext ctx, short profitYear);
    IQueryable<ParticipantTotalYear> GetYearsOfServiceAlt(IProfitSharingDbContext ctx, short profitYear);
    IQueryable<ParticipantTotalRatio> GetVestingRatioAlt(IProfitSharingDbContext ctx, short profitYear,
        DateOnly asOfDate);
    IQueryable<ParticipantTotalVestingBalance> TotalVestingBalanceAlt(IProfitSharingDbContext ctx,
        short employeeYear, short profitYear, DateOnly asOfDate);
}

public sealed class EmbeddedSqlService : IEmbeddedSqlService
{
    public IQueryable<ParticipantTotal> GetTotalBalanceAlt(IProfitSharingDbContext ctx, short profitYear)
    {

        var query = GetTotalBalanceQuery(profitYear);

        return ctx.ParticipantTotals.FromSqlRaw(query);
    }

    public IQueryable<ParticipantTotalYear> GetYearsOfServiceAlt(IProfitSharingDbContext ctx, short profitYear)
    {
        var query = GetYearsOfServiceQuery(profitYear);
        return ctx.ParticipantTotalYears.FromSqlRaw(query);
    }

    public IQueryable<ParticipantTotalRatio> GetVestingRatioAlt(IProfitSharingDbContext ctx, short profitYear,
        DateOnly asOfDate)
    {
        
        var query = GetVestingRatioQuery(profitYear, asOfDate);

        return ctx.ParticipantTotalRatios.FromSqlRaw(query);
    }

    public IQueryable<ParticipantTotalVestingBalance> TotalVestingBalanceAlt(IProfitSharingDbContext ctx,
        short employeeYear, short profitYear, DateOnly asOfDate)
    {
        var totalBalanceQuery = GetTotalBalanceQuery(profitYear);
        var vestingRatioQuery = GetVestingRatioQuery(profitYear, asOfDate);
        var yearsOfServiceQuery = GetYearsOfServiceQuery(employeeYear);

        var query = $@"
SELECT bal.Ssn, 
	   CASE WHEN ((bal.total + pdWrap.FORFEITURES - (pdWrap.PROF_6_CONTRIB + pdWrap.PROF_8_EARNINGS - pdWrap.PROF_9_FORFEIT)) * vr.RATIO) 
                    + ((pdWrap.PROF_6_CONTRIB + pdWrap.PROF_8_EARNINGS - pdWrap.PROF_9_FORFEIT)-pdWrap.FORFEITURES) > 0 THEN 
				 ((bal.total + pdWrap.FORFEITURES - (pdWrap.PROF_6_CONTRIB + pdWrap.PROF_8_EARNINGS - pdWrap.PROF_9_FORFEIT)) * vr.RATIO) 
                    + ((pdWrap.PROF_6_CONTRIB + pdWrap.PROF_8_EARNINGS - pdWrap.PROF_9_FORFEIT)-pdWrap.FORFEITURES) ELSE 0 END AS VESTEDBALANCE,
          bal.TOTAL AS CURRENTBALANCE,
          yip.YEARS,
          vr.RATIO
FROM (
{totalBalanceQuery}
) bal
LEFT JOIN (
{vestingRatioQuery}	
) vr ON bal.SSN = vr.SSN
LEFT JOIN (
{yearsOfServiceQuery}
) yip ON bal.SSN  = yip.SSN
LEFT JOIN (
	SELECT pd.SSN,
		   Sum(CASE WHEN pd.PROFIT_CODE_ID IN (1,2,3,5) THEN pd.FORFEITURE ELSE 0 END) AS FORFEITURES,
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

    private static string GetTotalBalanceQuery(short profitYear)
    {
        var query = @$"
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
        var birthDate65 = asOfDate.AddYears(-65);
        var beginningOfYear = asOfDate.AddYears(-1).AddDays(1);
        var yearsOfCreditQuery = GetYearsOfServiceQuery(profitYear);

        var query = @$"
SELECT m.SSN,
  CASE WHEN m.IS_EMPLOYEE = 0 THEN 1 ELSE --Beneficiaries are always 100% vested
  CASE WHEN m.DATE_OF_BIRTH < TO_DATE('{birthDate65.ToString("yyyy-MM-dd")}','YYYY-MM-DD') AND (m.IS_EMPLOYEE = 1 AND (m.TERMINATION_DATE IS NULL OR m.TERMINATION_DATE < TO_DATE('{beginningOfYear.ToString("yyyy-MM-dd")}','YYYY-MM-DD'))) THEN 1 ELSE --Otherwise, If over 65, and not terminated this year, 100% vested
  CASE WHEN m.ENROLLMENT_ID  IN (3, 4) THEN 1 ELSE --Otherwise, If enrollment has forfeitures, 100%
  CASE WHEN m.IS_EMPLOYEE = 1 AND m.TERMINATION_CODE_ID = 'Z' THEN 1 ELSE --Otherwise, If deceased, mark for 100% vested
  CASE WHEN m.ZERO_CONTRIBUTION_REASON_ID = 6 THEN 1 ELSE --Otherwise, If zero contribution reason is 65 or over, first contribution more than 5 years ago, 100% vested
  CASE WHEN m.YEARS_OF_SERVICE < 3 THEN 0 ELSE --Otherwise, If total years (including the present one) is 0, 1, or 2, 0% Vested
  CASE WHEN m.YEARS_OF_SERVICE = 3 THEN .2 ELSE --Otherwise, If total years (including the present one) is 3, 20% Vested
  CASE WHEN m.YEARS_OF_SERVICE = 4 THEN .4 ELSE --Otherwise, If total years (including the present one) is 4, 40% Vested
  CASE WHEN m.YEARS_OF_SERVICE = 5 THEN .6 ELSE --Otherwise, If total years (including the present one) is 5, 60% Vested
  CASE WHEN m.YEARS_OF_SERVICE = 6 THEN .8 ELSE --Otherwise, If total years (including the present one) is 6, 80% Vested
  CASE WHEN m.YEARS_OF_SERVICE > 6 THEN 1 ELSE --Otherwise, If total years (including the present one) is more than 6, 100% Vested
  0 END END END END END END END END END END END AS RATIO
FROM (
	SELECT d.SSN, d.DATE_OF_BIRTH, 1 AS IS_EMPLOYEE, d.TERMINATION_DATE, pp.ENROLLMENT_ID, d.TERMINATION_CODE_ID, pp.ZERO_CONTRIBUTION_REASON_ID, yos.YEARS + CASE WHEN pp.ENROLLMENT_ID = 2 THEN 1 ELSE 0 END + CASE WHEN pp.CURRENT_HOURS_YEAR  + pp.HOURS_EXECUTIVE  > 1000 THEN 1 ELSE 0 END AS YEARS_OF_SERVICE
	FROM DEMOGRAPHIC d
	LEFT JOIN PAY_PROFIT pp ON d.ID = pp.DEMOGRAPHIC_ID AND pp.PROFIT_YEAR  = {profitYear}
	LEFT JOIN (
		{yearsOfCreditQuery}
	) yos ON d.SSN = yos.SSN
	
	UNION ALL
	
	SELECT bc.SSN, bc.DATE_OF_BIRTH, 0 AS IS_EMPLOYEE, NULL AS TERMINATION_DATE, NULL AS ENROLLMENT_ID, NULL AS TERMINATION_CODE_ID, NULL AS ZERO_CONTRIBUTION_REASON_ID, 0 AS YEARS_OF_SERVICE
	FROM BENEFICIARY_CONTACT bc
	WHERE bc.SSN NOT IN (SELECT SSN FROM DEMOGRAPHIC)
) m
";
        return query;
    }
    private static string GetYearsOfServiceQuery(short profitYear)
    {
        var query = @$"
SELECT max_for_year.SSN, SUM(max_for_year.YOS_CREDIT ) YEARS
		FROM (
			SELECT pd.SSN, pd.PROFIT_YEAR, MAX(pd.YEARS_OF_SERVICE_CREDIT) YOS_CREDIT
			FROM PROFIT_DETAIL pd 
			WHERE pd.PROFIT_YEAR  <= {profitYear}
			GROUP BY pd.SSN, pd.PROFIT_YEAR
		) max_for_year
		GROUP BY max_for_year.SSN

";
        return query;
    }


}
