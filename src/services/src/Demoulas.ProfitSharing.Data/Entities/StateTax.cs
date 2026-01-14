using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

public class StateTax : ModifiedBase
{
    public required string Abbreviation { get; set; }
    public required decimal Rate { get; set; }

}
