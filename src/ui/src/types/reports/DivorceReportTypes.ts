export interface DivorceReportRequest {
  badgeNumber: number;
  startDate?: string; // ISO format date
  endDate?: string; // ISO format date
}

export interface DivorceReportResponse {
  badgeNumber: number;
  fullName: string;
  ssn: string; // Masked for security
  profitYear: number;
  totalContributions: number;
  totalWithdrawals: number;
  totalDistributions: number;
  totalDividends: number;
  totalForfeitures: number;
  endingBalance: number;
  cumulativeContributions: number;
  cumulativeWithdrawals: number;
  cumulativeDistributions: number;
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

export interface ReportResponseBase<T> {
  reportName: string;
  reportDate: string;
  startDate: string;
  endDate: string;
  response: PaginatedResponseDto<T>;
}

export interface DivorceReportFilterParams {
  badgeNumber: string;
  startDate: string; // ISO format
  endDate: string; // ISO format
}
