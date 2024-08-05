using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface ISynchronizationService
{
    Task<bool> SendSynchronizationRequest(OracleHcmJobRequest request, CancellationToken cancellationToken = default);
}
