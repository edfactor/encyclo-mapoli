using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

/// <summary>
/// Response indicating success of clearing demographic sync audit records.
/// </summary>
[NoMemberDataExposed]
public class ClearAuditResponse
{
    /// <summary>Number of records deleted.</summary>
    public int DeletedCount { get; set; }

    public static ClearAuditResponse ResponseExample()
    {
        return new ClearAuditResponse
        {
            DeletedCount = 42
        };
    }
}
