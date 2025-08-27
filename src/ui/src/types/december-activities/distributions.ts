import type { SortedPaginationRequestDto, ProfitYearRequest, PagedReportResponse } from "../common/api";

export interface DistributionsAndForfeituresRequestDto extends ProfitYearRequest {
  startDate?: string;
  endDate?: string;
  pagination: SortedPaginationRequestDto;
}

export interface DistributionsAndForfeitureTotalsResponse extends PagedReportResponse<DistributionsAndForfeitures> {
  distributionTotal: number;
  stateTaxTotal: number;
  federalTaxTotal: number;
  forfeitureTotal: number;
  stateTaxTotals: Record<string, number>;
}

export interface DistributionsAndForfeitures {
  badgeNumber: number;
  psnSuffix: number;
  employeeName: string;
  ssn: string;
  date: string;
  distributionAmount: number;
  stateTax: number;
  state: string | null;
  federalTax: number;
  forfeitAmount: number;
  age: number;
  taxCode: string | null;
  otherName: string | null;
  otherSsn: string | null;
  enrolled: boolean | null;
}

export interface DistributionsAndForfeituresQueryParams extends ProfitYearRequest {
  startDate?: string;
  endDate?: string;
}
