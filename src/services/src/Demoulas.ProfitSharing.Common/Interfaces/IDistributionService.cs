using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IDistributionService
{
    Task<PaginatedResponseDto<DistributionSearchResponse>> SearchAsync(DistributionSearchRequest request, CancellationToken cancellationToken);
    Task<CreateOrUpdateDistributionResponse> CreateDistribution(CreateDistributionRequest request, CancellationToken cancellationToken);
    Task<Result<CreateOrUpdateDistributionResponse>> UpdateDistribution(UpdateDistributionRequest request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteDistribution(int distributionId, CancellationToken cancellationToken);
    Task<Result<DistributionRunReportSummaryResponse[]>> GetDistributionRunReportSummary(CancellationToken cancellationToken);
    Task<Result<PaginatedResponseDto<DistributionsOnHoldResponse>>> GetDistributionsOnHold(SortedPaginationRequestDto request, CancellationToken cancellationToken);
    Task<Result<PaginatedResponseDto<ManualChecksWrittenResponse>>> GetManualCheckDistributions(SortedPaginationRequestDto request, CancellationToken cancellationToken);
    Task<Result<PaginatedResponseDto<DistributionRunReportDetail>>> GetDistributionRunReport(DistributionRunReportRequest request, CancellationToken cancellationToken);
}
