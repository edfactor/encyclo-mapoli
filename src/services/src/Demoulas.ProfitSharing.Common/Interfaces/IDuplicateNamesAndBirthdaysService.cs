using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IDuplicateNamesAndBirthdaysService
{
    Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdaysAsync(
        DuplicateNamesAndBirthdaysRequest req,
        CancellationToken cancellationToken = default);

    Task<DuplicateNamesAndBirthdaysCachedResponse?> GetCachedDuplicateNamesAndBirthdaysAsync(
        DuplicateNamesAndBirthdaysRequest request, CancellationToken cancellationToken = default);

    Task ForceRefreshCacheAsync(CancellationToken cancellationToken = default);
}
