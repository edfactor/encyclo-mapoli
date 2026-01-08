using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

public class Employee
{
    public required Demographic Demographic { get; set; }
    public required PayProfit PayProfit { get; set; }

    public required List<ProfitDetail> ProfitDetails { get; set; }
}
