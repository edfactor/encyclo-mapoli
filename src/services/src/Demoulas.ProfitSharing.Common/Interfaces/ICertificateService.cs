using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface ICertificateService
{
    public Task<string> GetCertificateFile(CerficatePrintRequest request, CancellationToken token);
    public Task<ReportResponseBase<CertificateReprintResponse>> GetMembersWithBalanceActivityByStore(CerficatePrintRequest request, CancellationToken token);
}
