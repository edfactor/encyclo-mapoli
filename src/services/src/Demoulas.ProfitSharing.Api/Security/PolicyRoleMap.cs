using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Api.Security;

/// <summary>
/// Central map of authorization policies to allowed roles. Used by both runtime registration and Swagger enrichment.
/// </summary>
public static class PolicyRoleMap
{
    public static readonly IReadOnlyDictionary<string, string[]> Map = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        // Reporting: finance reports are read-only. Allow Finance, Admin, and Auditor.
        [Policy.CanViewYearEndReports] = [Role.FINANCEMANAGER, Role.ADMINISTRATOR, Role.AUDITOR, Role.ITDEVOPS],

        // Balance views can be needed by Finance, Clerks, Admin, and HR (for participant context). Auditors are read-only.
        [Policy.CanViewBalances] = [Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.HARDSHIPADMINISTRATOR, Role.AUDITOR, Role.ITDEVOPS],

        // Year-end processes are highly sensitive; restrict to Finance and Admin only.
        [Policy.CanRunYearEndProcesses] = [Role.FINANCEMANAGER, Role.ADMINISTRATOR],

        // Master inquiry is broad read; allow Finance, Clerks, Admin and Auditor.
        [Policy.CanRunMasterInquiry] = [Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.AUDITOR, Role.ITDEVOPS, Role.HARDSHIPADMINISTRATOR],

        // New, clearer name for beneficiary CRUD; keep mapping identical.
        [Policy.CanManageBeneficiaries] = [Role.HARDSHIPADMINISTRATOR, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR],

        // IT roles can freeze and add demographics to support data integrity and operational needs.
        [Policy.CanFreezeDemographics] = [Role.ITDEVOPS, Role.ITOPERATIONS, Role.HARDSHIPADMINISTRATOR, Role.ADMINISTRATOR],
    };

    public static string[] GetRoles(string policyName) => Map.TryGetValue(policyName, out var roles) ? roles : [];
}
