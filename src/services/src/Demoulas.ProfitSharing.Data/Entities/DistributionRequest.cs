using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Was PROFIT_DIST_REQ  in READY system.
 */

namespace Demoulas.ProfitSharing.Data.Entities;
public class DistributionRequest
{
    public required long Id { get; set; }

    public required long PSN { get; set; }

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

    public required TaxCode TaxCode { get; set; }

}
