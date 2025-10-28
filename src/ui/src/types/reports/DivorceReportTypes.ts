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

export interface ReportResponseBase<T> {
  data: T[];
  message?: string;
  isSuccess: boolean;
}

export interface DivorceReportFilterParams {
  badgeNumber: string;
  startDate: string; // ISO format
  endDate: string; // ISO format
}
