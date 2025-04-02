namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;
internal sealed class ParticipantTotalVestingBalanceDto
{
    internal int Ssn { get; set; }
    internal decimal? VestedBalance { get; set; }
    internal decimal? TotalDistributions { get; set; }
    internal decimal? Etva { get; set; }
    internal decimal? VestingPercent { get; set; }
    internal decimal? CurrentBalance { get; set; }
    internal byte? YearsInPlan { get; set; }
}
