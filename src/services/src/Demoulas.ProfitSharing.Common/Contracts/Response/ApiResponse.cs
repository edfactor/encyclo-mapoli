using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed record ApiResponse<T>(
    DateOnly StartDate,
    DateOnly EndDate,
    T Data) : IHasDateRange;
