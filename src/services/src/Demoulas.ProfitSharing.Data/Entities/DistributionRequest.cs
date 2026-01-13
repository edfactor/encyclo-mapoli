using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

public class DistributionRequest : ModifiedBase
{
    public required int Id { get; set; }

    public required int DemographicId { get; set; }

    public byte ReasonId { get; set; }
    public required DistributionRequestReason Reason { get; set; }

    public char StatusId { get; set; }
    public required DistributionRequestStatus Status { get; set; }

    public byte TypeId { get; set; }
    public required DistributionRequestType Type { get; set; }

    public string? ReasonText { get; set; }

    public string? ReasonOtherText { get; set; }

    public decimal AmountRequested { get; set; }

    public decimal? AmountAuthorized { get; set; }

    public DateOnly DateRequested { get; set; }

    public DateOnly? DateDecided { get; set; }

    public char TaxCodeId { get; set; }
    public required TaxCode TaxCode { get; set; }

}
