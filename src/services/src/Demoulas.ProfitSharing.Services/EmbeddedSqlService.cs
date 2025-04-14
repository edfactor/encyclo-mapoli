using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

public interface IEmbeddedSqlService
{
    IQueryable<ParticipantTotal> GetTotalBalanceAlt(IProfitSharingDbContext ctx, short profitYear);
}

public sealed class EmbeddedSqlService : IEmbeddedSqlService
{
    public IQueryable<ParticipantTotal> GetTotalBalanceAlt(IProfitSharingDbContext ctx, short profitYear)
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

            return ctx.ParticipantTotals.FromSql(query);
        }
    
}
