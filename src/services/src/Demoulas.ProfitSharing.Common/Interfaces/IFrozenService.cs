using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IFrozenService
{
    Task<SetFrozenStateResponse> FreezeDemographics(short profitYear, DateTime asOfDateTime, CancellationToken cancellationToken = default);
}
