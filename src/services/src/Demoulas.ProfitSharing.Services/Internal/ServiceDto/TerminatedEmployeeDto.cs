using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

internal sealed record TerminatedEmployeeDto
{
    internal required Demographic Demographic { get; set; }
}

