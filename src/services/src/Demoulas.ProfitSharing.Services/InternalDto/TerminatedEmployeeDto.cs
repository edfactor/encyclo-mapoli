using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.InternalDto;
public sealed record TerminatedEmployeeDto
{
    public required Demographic Demographic { get; set; }
    public required PayProfit? PayProfit { get; set; }
}

