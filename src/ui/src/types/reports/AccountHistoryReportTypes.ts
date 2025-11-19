import { SortedPaginationRequestDto } from "../common/api";

export interface AccountHistoryReportRequest {
  badgeNumber: number;
  startDate?: string; // ISO format date
  endDate?: string; // ISO format date
  pagination: SortedPaginationRequestDto;
}

export interface AccountHistoryReportResponse {
  id: number;
  badgeNumber: number;
  fullName: string;
  ssn: string; // Masked for security
  profitYear: number;
  contributions: number;
  earnings: number;
  forfeitures: number;
  withdrawals: number;
  endingBalance: number;
  comment?: string;
}

export interface PaginatedResponseDto<T> {
  pageSize: number | null;
  currentPage: number | null;
  totalPages: number | null;
  resultHash: string | null;
  total: number;
  isPartialResult: boolean;
  timeoutOccurred: boolean;
  results: T[];
}

export interface AccountHistoryReportTotals {
  totalContributions: number;
  totalEarnings: number;
  totalForfeitures: number;
  totalWithdrawals: number;
}

export interface AccountHistoryReportPaginatedResponse {
  reportName: string;
  reportDate: string;
  startDate: string;
  endDate: string;
  response: PaginatedResponseDto<AccountHistoryReportResponse>;
  cumulativeTotals?: AccountHistoryReportTotals;
}

export interface ReportResponseBase<T> {
  reportName: string;
  reportDate: string;
  startDate: string;
  endDate: string;
  dataSource?: string;
  response: PaginatedResponseDto<T>;
}

export interface AccountHistoryReportFilterParams {
  badgeNumber: string;
  startDate: string; // ISO format
  endDate: string; // ISO format
}
