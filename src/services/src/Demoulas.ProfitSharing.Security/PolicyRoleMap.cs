namespace Demoulas.ProfitSharing.Security;

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

        // Master inquiry is broad read; allow Finance, Clerks, Admin, Auditor, and HR-ReadOnly.
        [Policy.CanRunMasterInquiry] = [Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.AUDITOR, Role.ITDEVOPS, Role.HARDSHIPADMINISTRATOR, Role.HR_READONLY],

        // New, clearer name for beneficiary CRUD; keep mapping identical.
        [Policy.CanManageBeneficiaries] = [Role.FINANCEMANAGER, Role.HARDSHIPADMINISTRATOR, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.BENEFICIARY_ADMINISTRATOR],

        // Freeze is an operational integrity action; restrict to IT DevOps only.
        [Policy.CanFreezeDemographics] = [Role.ITDEVOPS],

        // Distribution views are read-only; allow Finance, Clerks, Admin, Auditor, and IT DevOps.
        [Policy.CanViewDistributions] = [Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.AUDITOR, Role.ITDEVOPS],

        // Reversing profit details is a sensitive operation; restrict to Finance and Admin.
        [Policy.CanReverseProfitDetails] = [Role.FINANCEMANAGER, Role.ADMINISTRATOR],

        // Administration pages/actions are restricted to IT DevOps + System Admin.
        [Policy.CanManageAdministration] = [Role.ITDEVOPS, Role.ADMINISTRATOR],

        // Audit viewing for security and compliance teams.
        [Policy.CanViewAudits] = [Role.ADMINISTRATOR, Role.AUDITOR, Role.ITDEVOPS],

        // Duplicate names and birthdays cleanup report: same access as year-end reports plus HR-ReadOnly.
        [Policy.CanViewDuplicateNamesAndBirthdays] = [Role.FINANCEMANAGER, Role.ADMINISTRATOR, Role.AUDITOR, Role.ITDEVOPS, Role.HR_READONLY],

        // Unmask SSN values: highly restricted to SSN-Unmasking role only.
        [Policy.CanUnmaskSsn] = [Role.SSN_UNMASKING],

        [Policy.CanProcessChecks] = [Role.FINANCEMANAGER, Role.ADMINISTRATOR, Role.DISTRIBUTIONSCLERK, Role.ITDEVOPS, Role.HARDSHIPADMINISTRATOR]
    };

    public static string[] GetRoles(string policyName) => Map.TryGetValue(policyName, out var roles) ? roles : [];
}
