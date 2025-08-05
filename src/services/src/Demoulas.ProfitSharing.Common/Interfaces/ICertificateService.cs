using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface ICertificateService
{
    public Task<string> GetCertificateFile(ProfitYearRequest request, CancellationToken token);
}
