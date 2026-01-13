using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Services.Internal.Interfaces;

public interface IEmbeddedSqlService
{
    IQueryable<ParticipantTotal> GetTotalBalanceAlt(IProfitSharingDbContext ctx, short profitYear);
    IQueryable<ParticipantTotalYear> GetYearsOfServiceAlt(IProfitSharingDbContext ctx, short profitYear, DateOnly asOfDate);
    IQueryable<ParticipantTotalRatio> GetVestingRatioAlt(IProfitSharingDbContext ctx, short profitYear,
        DateOnly asOfDate);
    IQueryable<ParticipantTotalVestingBalance> TotalVestingBalanceAlt(IProfitSharingDbContext ctx,
        short employeeYear, short profitYear, DateOnly asOfDate);

    IQueryable<ParticipantTotal> GetTotalComputedEtvaAlt(IProfitSharingDbContext ctx, short profitYear);

    IQueryable<ProfitShareTotal?> GetProfitShareTotals(IProfitSharingDbContext ctx, short profitYear,
        DateOnly fiscalEndDate,
        short min_hours, DateOnly birthdate_21, CancellationToken cancellationToken);
    IQueryable<ProfitDetailRollup> GetTransactionsBySsnForProfitYearForOracle(IProfitSharingDbContext ctx, short profitYear);
}
