namespace Demoulas.ProfitSharing.Security;

/// <summary>
/// Centralized application role names that must match Okta role claims 1:1.
/// </summary>
public static class Role
{
    /// <summary>
    /// Break-glass administrators with full authority across the application; use sparingly and audit.
    /// </summary>
    public const string ADMINISTRATOR = "System-Administrator";

    /// <summary>
    /// Finance owners who can run profit sharing operations (e.g., year-end) and view finance data.
    /// </summary>
    public const string FINANCEMANAGER = "Finance-Manager";

    /// <summary>
    /// Operational finance users who can view inquiries/results and perform day-to-day read-only tasks.
    /// </summary>
    public const string DISTRIBUTIONSCLERK = "Distributions-Clerk";

    /// <summary>
    /// HR/Benefits administrators managing demographics and beneficiaries (non-finance operations).
    /// </summary>
    public const string HARDSHIPADMINISTRATOR = "Hardship-Administrator";

    /// <summary>
    /// Allows a user to impersonate another for support with explicit audit/controls (no extra data rights).
    /// </summary>
    public const string IMPERSONATION = "Impersonation";

    /// <summary>
    /// DevOps support role for diagnostics; read-only to business data and cannot mutate state.
    /// </summary>
    public const string ITDEVOPS = "IT-DevOps";

    /// <summary>
    /// IT operations support; similar to DevOps, diagnostics-only without business data mutations.
    /// </summary>
    public const string ITOPERATIONS = "IT-Operations";

    /// <summary>
    /// Users allowed to view Executive financial details.
    /// </summary>
    public const string EXECUTIVEADMIN = "Executive-Administrator";

    /// <summary>
    /// Read-only access for audit/reporting and verification; no create/update/delete operations.
    /// </summary>
    public const string AUDITOR = "Auditor";
}
