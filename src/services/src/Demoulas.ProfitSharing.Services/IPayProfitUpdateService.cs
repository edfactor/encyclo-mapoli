using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services;

public interface IPayProfitUpdateService
{
    Task SetZeroContributionReason(IQueryable<PayProfit> records, byte zeroContributionReasonId, CancellationToken cancellationToken);
    Task SetEnrollmentId(short profitYear, CancellationToken ct);
}
