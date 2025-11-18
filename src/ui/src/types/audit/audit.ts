export interface AuditChangeEntryDto {
  id: number;
  columnName: string;
  originalValue: string | null;
  newValue: string | null;
}

export interface AuditEventDto {
  auditEventId: number;
  tableName: string | null;
  operation: string;
  primaryKey: string | null;
  userName: string;
  createdAt: string; // DateTimeOffset as ISO string
  changesJson?: AuditChangeEntryDto[] | null;
}

export interface AuditSearchRequestDto {
  tableName?: string | null;
  operation?: string | null;
  userName?: string | null;
  startTime?: string | null; // DateTimeOffset as ISO string
  endTime?: string | null; // DateTimeOffset as ISO string
  skip: number;
  take: number;
  sortBy?: string;
  isSortDescending?: boolean;
}
