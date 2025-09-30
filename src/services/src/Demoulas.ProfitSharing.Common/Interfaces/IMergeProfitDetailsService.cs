using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IMergeProfitDetailsService
{
    Task MergeProfitDetailsToDemographic(int sourceDemographic, int targetDemographic, CancellationToken cancellationToken = default);
}
