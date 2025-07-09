using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
public sealed record UpdateBeneficiaryPercentageRequest
{
    public int Id { get; set; }
    public int? Percentage { get; set; }

    public static  UpdateBeneficiaryPercentageRequest SampleRequest()
    {
        return new UpdateBeneficiaryPercentageRequest()
        {
            Id = 1,
            Percentage = 100
        };
    }
}
