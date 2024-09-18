using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public class TerminatedEmployeeAndBeneficiaryReportRequestDto
{
    [DefaultValue("01/07/2023")]
    public required DateOnly startDate { get; set; } = new DateOnly(2023,1,7);
    [DefaultValue("01/02/2024")]
    public required DateOnly endDate { get; set; } = new DateOnly(2024, 1, 2);
    [DefaultValue("2023.0")]
    public required decimal profitShareYear { get; set; } = 2023.0m;
}
