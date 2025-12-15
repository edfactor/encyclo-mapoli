using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;

public sealed record GetProfitSharingAdjustmentsRequest : IProfitYearRequest
{
    public short ProfitYear { get; set; }

    public required long OracleHcmId { get; init; }

    public required int SequenceNumber { get; init; }
}
