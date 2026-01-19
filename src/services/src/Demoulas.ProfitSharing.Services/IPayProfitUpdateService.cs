using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services;

public interface IPayProfitUpdateService
{
    Task SetZeroContributionReasonAsync(IQueryable<PayProfit> records, byte zeroContributionReasonId, CancellationToken cancellationToken);
    Task SetEnrollmentIdAsync(short profitYear, CancellationToken ct);
}
