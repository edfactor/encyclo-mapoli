using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;

public sealed record GetProfitSharingAdjustmentsRequest : IProfitYearRequest
{
    public short ProfitYear { get; set; }

    public required int BadgeNumber { get; init; }

    public required int SequenceNumber { get; init; }

    public bool GetAllRows { get; init; }
}
