using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities.Audit;
public sealed class BeneficiaryArchive : Beneficiary
{
    public int ArchiveId { get; set; }  
    public DateOnly DeleteDate { get; set; }
    public required string DeletedBy { get; set; }
}
