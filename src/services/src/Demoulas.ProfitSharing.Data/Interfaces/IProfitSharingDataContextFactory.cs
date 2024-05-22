using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Interfaces;

public interface IProfitSharingDataContextFactory : IDataContextFactory<ProfitSharingDbContext, ProfitSharingReadOnlyDbContext>
{
}
