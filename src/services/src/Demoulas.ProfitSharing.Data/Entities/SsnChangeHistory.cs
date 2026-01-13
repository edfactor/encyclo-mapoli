
using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

public abstract class SsnChangeHistory : ModifiedBase
{
    public int Id { get; set; }

    public int OldSsn { get; set; }

    public int NewSsn { get; set; }
}
