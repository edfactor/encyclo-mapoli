namespace Demoulas.ProfitSharing.Security;

/// <summary>
/// Centralized policy names used by [Authorize(Policy = ...)] to guard business actions.
/// </summary>
public static class Policy
{
    /// <summary>
    /// View year-end financial reports (read-only). Sensitive finance data visibility.
    /// </summary>
    public static readonly string CanViewYearEndReports = "CAN_VIEW_YEAR_END_REPORTS";

    /// <summary>
    /// View pay classification types (reference data, read-only).
    /// </summary>
    public static readonly string CanViewPayClassificationTypes = "CAN_VIEW_PAY_CLASSIFICATION_TYPES";

    /// <summary>
    /// View pay/profit record information (read-only finance data views).
    /// </summary>
    public static readonly string CanGetPayProfitRecords = "CAN_GET_PAY_PROFIT_RECORDS";

    /// <summary>
    /// View balances (participant or aggregated), read-only.
    /// </summary>
    public static readonly string CanViewBalances = "CAN_VIEW_BALANCES";

    /// <summary>
    /// Freeze demographics (operational integrity action; should be rare and controlled).
    /// </summary>
    public static readonly string CanFreezeDemographics = "CAN_FREEZE_DEMOGRAPHICS";

    /// <summary>
    /// Execute year-end financial processes (allocations/close) with system-wide effects.
    /// </summary>
    public static readonly string CanRunYearEndProcesses = "CAN_RUN_YEAR_END_PROCESSES";

    /// <summary>
    /// Run master inquiry (broad data inquiry across participants/records).
    /// </summary>
    public static readonly string CanRunMasterInquiry = "CAN_RUN_MASTER_INQUIRY";

    /// <summary>
    /// Manage beneficiaries (CRUD). New policy name for clarity; mapped to same roles as maintain.
    /// </summary>
    public static readonly string CanManageBeneficiaries = "CAN_MANAGE_BENEFICIARIES";
}
