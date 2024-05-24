using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;

namespace Demoulas.ProfitSharing.Data.Interfaces;

public interface IProfitSharingDataContextFactory : IDataContextFactory<ProfitSharingDbContext, ProfitSharingReadOnlyDbContext>
{
}
