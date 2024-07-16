using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.UnitTests.Extensions;
internal static class FakeHelperExtensions
{
    internal static long ConvertSsnToLong(this string ssn)
    {
        // Remove non-numeric characters if any (e.g., dashes)
        string numericSsn = new string(ssn.Where(char.IsDigit).ToArray());

        // Convert to long
        return long.Parse(numericSsn);
    }
}
