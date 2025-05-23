using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
public sealed record CreateBeneficiaryResponse
{
    public int BeneficiaryId { get; set; }
    public short PsnSuffix { get; set; }
    public bool ContactExisted { get; set; }

    public static CreateBeneficiaryResponse SampleResponse()
    {
        return new CreateBeneficiaryResponse { PsnSuffix = 1000, ContactExisted = false, BeneficiaryId = 20015 };
    }
}
