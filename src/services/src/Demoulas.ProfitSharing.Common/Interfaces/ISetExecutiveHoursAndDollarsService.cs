using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface ISetExecutiveHoursAndDollarsService
{
    Task SetExecutiveHoursAndDollars(short profitYear, List<SetExecutiveHoursAndDollarsDto> executiveHoursAndDollarsDtos);

}
