using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface ICertificateService
{
    Task<string> GetCertificateFile(CerficatePrintRequest request, CancellationToken token);
    Task<ReportResponseBase<CertificateReprintResponse>> GetMembersWithBalanceActivityByStore(CerficatePrintRequest request, CancellationToken token);
}
