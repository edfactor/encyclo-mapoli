import type { PagedReportResponse, SortedPaginationRequestDto } from "../common/api";

export interface QPAY066BTerminatedWithVestedBalanceRequest {
  profitYear: number;
  pagination: SortedPaginationRequestDto;
}

export interface QPAY066BTerminatedEmployee {
  storeNumber: number;
  badgeNumber: number;
  fullName: string;
  payClassificationId: number;
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
