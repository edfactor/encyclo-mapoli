using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.Extensions;

internal static class EntityExtensions
{
    /// <summary>
    /// Generates a PSN (Personal Serial Number) by concatenating the EmployeeId and PsnSuffix properties.
    /// </summary>
    /// <returns>
    /// A long value representing the generated PSN.
    /// </returns>
    public static long GetPsn(this Beneficiary beneficiary)
    {
        long.TryParse($"{beneficiary.EmployeeId}{beneficiary.PsnSuffix:D4}", out long psn);
        return psn;
    }
}
