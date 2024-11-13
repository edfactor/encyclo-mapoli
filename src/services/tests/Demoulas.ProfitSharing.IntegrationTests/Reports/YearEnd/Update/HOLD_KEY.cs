using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

using static Utils;

public class HOLD_KEY
{
    public long HOLD_BADGE { get; set; } // PIC 9(7).

    public override string ToString()
    {
        return
            rformat(HOLD_BADGE, "long", "9(7).")
            ;
    }
}
