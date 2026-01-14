using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface ICertificateService
{
    Task<Result<string>> GetCertificateFile(CerficatePrintRequest request, CancellationToken token);
    Task<Result<ReportResponseBase<CertificateReprintResponse>>> GetMembersWithBalanceActivityByStore(CerficatePrintRequest request, CancellationToken token);
}
