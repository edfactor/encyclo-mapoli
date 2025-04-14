using System.ComponentModel.DataAnnotations.Schema;

namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;
public sealed class ParticipantTotalDto
{
    [Column("SSN")]
    internal int Ssn { get; set; }
    [Column("TOTAL")]
    internal decimal? Total { get; set; }
}
