import type { PagedReportResponse, SortedPaginationRequestDto } from "../common/api";

export interface QPAY066BTerminatedWithVestedBalanceRequest {
  profitYear: number;
  pagination: SortedPaginationRequestDto;
}

export interface QPAY066BTerminatedEmployee {
  storeNumber: number;
  badgeNumber: number;
  fullName: string;
  payClassificationId: string; // changed from number to string per backend DTO change
  payClassificationName: string;
  beginningBalance: number;
  earnings: number;
  contributions: number;
  forfeitures: number;
  distributions: number;
  endingBalance: number;
  vestedAmount: number;
  vestedPercent: number;
  dateOfBirth: string;
  hireDate: string;
  terminationDate: string;
  enrollmentId: number;
  profitShareHours: number;
  street1: string;
  city: string;
  state: string;
  postalCode: string;
  certificateSort: number;
}

export interface QPAY066BTerminatedWithVestedBalanceResponse {
  reportName: string;
  reportDate: string;
  startDate: string;
  endDate: string;
  response: PagedReportResponse<QPAY066BTerminatedEmployee>;
}

export interface ProfitSharingUnder21ReportRequest {
  profitYear: number;
  pagination: SortedPaginationRequestDto;
}

export interface ProfitSharingUnder21ReportDetail {
  storeNumber: number;
  badgeNumber: number;
  firstName: string;
  lastName: string;
  ssn: string;
  profitSharingYears: number;
  isNew: boolean;
  thisYearHours: number;
  lastYearHours: number;
  hireDate: string;
  fullTimeDate: string | null;
  terminationDate: string | null;
  dateOfBirth: string;
  age: number;
  employmentStatusId: string;
  currentBalance: number;
  enrollmentId: number;
  isExecutive: boolean;
}

export interface ProfitSharingUnder21TotalForStatus {
  totalVested: number;
  partiallyVested: number;
  partiallyVestedButLessThanThreeYears: number;
}

export interface ProfitSharingUnder21ReportResponse extends PagedReportResponse<ProfitSharingUnder21ReportDetail> {
  reportName: string;
  reportDate: string;
  startDate: string;
  endDate: string;
  activeTotals: ProfitSharingUnder21TotalForStatus;
  inactiveTotals: ProfitSharingUnder21TotalForStatus;
  terminatedTotals: ProfitSharingUnder21TotalForStatus;
  totalUnder21: number;
}
