namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IEmployeeLookupService
{
    Task<bool> BadgeExistsAsync(int badgeNumber, CancellationToken cancellationToken = default);
}
