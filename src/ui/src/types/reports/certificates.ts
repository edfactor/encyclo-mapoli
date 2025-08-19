import { PagedReportResponse, ProfitYearRequest, SortedPaginationRequestDto } from "../common/api";

export interface CertificatePrintRequest extends ProfitYearRequest, SortedPaginationRequestDto {
  ssns?: number[];
  badgeNumbers?: number[];
}

export interface CertificateDownloadRequest extends ProfitYearRequest {
  ssns?: number[];
  badgeNumbers?: number[];
}

export interface CertificateReprintResponse {
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
  terminationDate?: string;
  enrollmentId?: number;
  profitShareHours: number;
  street1: string;
  city?: string;
  state?: string;
  postalCode?: string;
  certificateSort: number;
  annuitySingleRate?: number;
  annuityJointRate?: number;
  monthlyPaymentSingle?: number;
  monthlyPaymentJoint?: number;
}

export type CertificatesReportResponse = PagedReportResponse<CertificateReprintResponse>;
