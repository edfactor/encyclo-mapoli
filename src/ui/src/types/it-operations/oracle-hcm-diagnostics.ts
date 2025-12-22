/// <summary>
/// OracleHcm sync metadata with four timestamp fields.
/// </summary>
export interface OracleHcmSyncMetadata {
  demographicCreatedAtUtc: string | null;
  demographicModifiedAtUtc: string | null;
  payProfitCreatedAtUtc: string | null;
  payProfitModifiedAtUtc: string | null;
}

/// <summary>
/// Single demographic sync audit record.
/// </summary>
export interface DemographicSyncAuditRecord {
  id: number;
  badgeNumber: number;
  oracleHcmId: number;
  message: string;
  propertyName: string | null;
  invalidValue: string | null;
  userName: string | null;
  created: string;
}

/// <summary>
/// Response from clear sync error records endpoint.
/// </summary>
export interface ClearAuditResponse {
  deletedCount: number;
}
