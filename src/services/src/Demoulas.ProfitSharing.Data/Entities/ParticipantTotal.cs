using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class ParticipantTotal
{
    public int Ssn { get; set; }
    public decimal? Total { get; set; }
}
