namespace Demoulas.ProfitSharing.Data.Entities.Audit;

public sealed class AuditEvent
{
    public long Id { get; set; }
    public required string? TableName { get; set; }
    public required string Operation { get; set; }
    public string? PrimaryKey { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<AuditChangeEntry>? ChangesJson { get; set; }
    public string? ChangesHash { get; set; }

    /// <summary>
    /// Defines standard audit operation names for consistent tracking across the application.
    /// These operation names are used when creating AuditEvent records to identify the type of action being logged.
    /// </summary>
    public static class AuditOperations
    {
        /// <summary>Report data export/archive operation</summary>
        public const string Archive = "Archive";

        /// <summary>Sensitive PII unmasking - SSN access for compliance/verification purposes</summary>
        public const string UnmaskSsn = "UnmaskSsn";

        /// <summary>Sensitive data modification (create)</summary>
        public const string Create = "Create";

        /// <summary>Sensitive data modification (update)</summary>
        public const string Update = "Update";

        /// <summary>Sensitive data modification (delete)</summary>
        public const string Delete = "Delete";

        /// <summary>Sensitive data access for auditing/compliance</summary>
        public const string SensitiveAccess = "SensitiveAccess";

        /// <summary>Security-related operation (role change, permission modification)</summary>
        public const string SecurityOperation = "SecurityOperation";
    }
}
