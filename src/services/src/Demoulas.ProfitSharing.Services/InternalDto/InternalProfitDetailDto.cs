namespace Demoulas.ProfitSharing.Services.InternalDto;
internal sealed record InternalProfitDetailDto
{
    public long OracleHcmId { get; set; }
    public int BadgeNumber { get; set; }
    public decimal NetBalance { get; set; }
    public decimal VestedBalance { get; set; }
}
