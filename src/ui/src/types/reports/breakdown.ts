import type { PagedReportResponse, ProfitYearRequest, SortedPaginationRequestDto } from "../common/api";

export interface BreakdownByStoreRequest extends ProfitYearRequest {
  storeNumber?: number;
  storeManagement?: boolean;
  badgeNumber?: number;
  employeeName?: string;
  pagination: SortedPaginationRequestDto;
}

export interface BreakdownByStoreEmployee {
  badgeNumber: number;
  beginningBalance: number;
  certificateSort: number;
  city: string;
  contributions: number;
  dateOfBirth: string;
  departmentId: number;
  distributions: number;
  earnings: number;
  employmentStatusId: string;
  endingBalance: number;
  enrollmentId: number;
  forfeitures: number;
  fullName: string;
  hireDate: string;
  isExecutive: boolean;
  payClassificationId: string; // changed from number to string to match backend refactor
  payClassificationName: string;
  payFrequencyId: number;
  postalCode: string;
  profitShareHours: number;
  ssn: string;
  state: string;
  storeNumber: number;
  street1: string;
  terminationDate: string;
  vestedAmount: number;
  vestedPercentage: number;
}

export interface BreakdownByStoreTotals {
  totalNumberEmployees: number;
  totalBeginningBalances: number;
  totalEarnings: number;
  totalContributions: number;
  totalForfeitures: number;
  totalDisbursements: number;
  totalEndBalances: number;
  totalVestedBalance: number;
}

export interface GrandTotalsByStoreResponseDto {
  rows: GrandTotalsByStoreRowDto[];
}

export interface GrandTotalsByStoreRowDto {
  category: string;
  store700: number;
  store701: number;
  store800: number;
  store801: number;
  store802: number;
  store900: number;
  storeOther: number;
  rowTotal: number;
}

export interface BreakdownByStoreResponse extends PagedReportResponse<BreakdownByStoreEmployee> {
  totalBeginningBalance: number;
  totalEarnings: number;
  totalContribution: number;
  totalForfeiture: number;
  totalDistribution: number;
  totalEndingBalance: number;
  totalVestedAmount: number;
}

export interface Under21BreakdownByStoreRequest extends ProfitYearRequest {
  isSortDescending?: boolean;
  pagination: SortedPaginationRequestDto;
}

export interface Under21BreakdownByStoreEmployee {
  storeNumber: number;
  badgeNumber: number;
  fullName: string;
  beginningBalance: number;
  earnings: number | null;
  contributions: number | null;
  forfeitures: number | null;
  distributions: number | null;
  endingBalance: number;
  vestedAmount: number;
  vestingPercentage: number;
  dateOfBirth: string;
  age: number;
  enrollmentId: number;
}
// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface Under21BreakdownByStoreResponse extends PagedReportResponse<Under21BreakdownByStoreEmployee> {
  // Under21BreakdownByStoreResponse extends PagedReportResponse
}

export interface Under21InactiveRequest extends ProfitYearRequest {
  isSortDescending?: boolean;
  pagination: SortedPaginationRequestDto;
}

export interface Under21InactiveEmployee {
  badgeNumber: number;
  lastName: string;
  firstName: string;
  birthDate: string;
  hireDate: string;
  terminationDate: string;
  age: number;
  enrollmentId: number;
}
// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface Under21InactiveResponse extends PagedReportResponse<Under21InactiveEmployee> {
  // Under21InactiveResponse extends PagedReportResponse
}

export interface Under21TotalsRequest extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface Under21TotalsResponse {
  numberOfEmployees: number;
  numberOfActiveUnder21With1to2Years: number;
  numberOfActiveUnder21With20to80PctVested: number;
  numberOfActiveUnder21With100PctVested: number;
  numberOfInActiveUnder21With1to2Years: number;
  numberOfInActiveUnder21With20to80PctVested: number;
  numberOfInActiveUnder21With100PctVested: number;
  numberOfTerminatedUnder21With1to2Years: number;
  numberOfTerminatedUnder21With20to80PctVested: number;
  numberOfTerminatedUnder21With100PctVested: number;
  totalBeginningBalance: number | null;
  totalEarnings: number;
  totalContributions: number;
  totalForfeitures: number;
  totalDisbursements: number;
  totalEndingBalance: number | null;
  totalVestingBalance: number;
}
