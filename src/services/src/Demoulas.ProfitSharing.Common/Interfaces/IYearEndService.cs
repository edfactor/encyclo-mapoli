using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IYearEndService
{
    Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSSNs(PaginationRequestDto req, CancellationToken ct);

    Task<ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>> GetPayProfitBadgesNotInDemographics(PaginationRequestDto req, CancellationToken ct = default);

    Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetNegativeETVAForSSNsOnPayProfitResponse(PaginationRequestDto req, CancellationToken cancellationToken = default);
    Task<ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>> GetMismatchedSsnsPayprofitAndDemographicsOnSameBadge(PaginationRequestDto req, CancellationToken cancellationToken = default);
    Task<ReportResponseBase<NamesMissingCommaResponse>> GetNamesMissingComma(PaginationRequestDto req, CancellationToken cancellationToken = default);

    Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdays(PaginationRequestDto req, CancellationToken cancellationToken = default);
    Task<ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>> GetPayrollDuplicateSsnsOnPayprofit(PaginationRequestDto req, CancellationToken cancellationToken = default);
    Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetDemographicBadgesNotInPayProfit(PaginationRequestDto req,CancellationToken cancellationToken = default);
}
