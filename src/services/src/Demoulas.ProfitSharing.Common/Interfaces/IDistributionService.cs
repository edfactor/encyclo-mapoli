using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IDistributionService
{
    Task<PaginatedResponseDto<DistributionSearchResponse>> SearchAsync(DistributionSearchRequest request, CancellationToken cancellationToken);
    Task<CreateOrUpdateDistributionResponse> CreateDistributionAsync(CreateDistributionRequest request, CancellationToken cancellationToken);
    Task<Result<CreateOrUpdateDistributionResponse>> UpdateDistributionAsync(UpdateDistributionRequest request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteDistributionAsync(int distributionId, CancellationToken cancellationToken);
    Task<Result<DistributionRunReportSummaryResponse[]>> GetDistributionRunReportSummaryAsync(CancellationToken cancellationToken);
    Task<Result<PaginatedResponseDto<DistributionsOnHoldResponse>>> GetDistributionsOnHoldAsync(SortedPaginationRequestDto request, CancellationToken cancellationToken);
    Task<Result<PaginatedResponseDto<ManualChecksWrittenResponse>>> GetManualCheckDistributionsAsync(SortedPaginationRequestDto request, CancellationToken cancellationToken);
    Task<Result<PaginatedResponseDto<DistributionRunReportDetail>>> GetDistributionRunReportAsync(DistributionRunReportRequest request, CancellationToken cancellationToken);
    Task<Result<PaginatedResponseDto<DisbursementReportDetailResponse>>> GetDisbursementReportAsync(ProfitYearRequest request, CancellationToken cancellationToken);
}
