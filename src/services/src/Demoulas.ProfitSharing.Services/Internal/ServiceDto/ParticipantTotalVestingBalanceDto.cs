namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

internal sealed class ParticipantTotalVestingBalanceDto : ParticipantTotalBalanceDto
{
    internal decimal? VestedBalance { get; set; }
    internal decimal? VestingPercent { get; set; }
    internal decimal? CurrentBalance { get; set; }
    internal byte? YearsInPlan { get; set; }
}
