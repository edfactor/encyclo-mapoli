using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Services;
public class ParticipantTotalRatioDto
{
    public required long Ssn { get; set; }
    public required double Ratio { get; set; }
}
