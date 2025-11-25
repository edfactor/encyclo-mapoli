import type {
  FilterParams,
  FrozenProfitYearRequest,
  PagedReportResponse,
  ProfitYearRequest,
  SortedPaginationRequestDto
} from "../common/api";

export interface EmployeeDetails {
  id: number;
  badgeNumber: number;
  psnSuffix: number;
  payFrequencyId: number;
  isEmployee: boolean;
  firstName: string;
  lastName: string;
  fullName: string;
  address: string;
  addressCity: string;
  addressState: string;
  addressZipCode: string;
  dateOfBirth: string;
  age?: string;
  ssn: string;
  yearToDateProfitSharingHours: number;
  yearsInPlan: number;
  percentageVested: number;
  contributionsLastYear: boolean;
  enrollmentId: number;
  enrollment: string;
  hireDate: string;
  terminationDate: string | null;
  reHireDate: string | null;
  storeNumber: number;
  beginPSAmount: number;
  currentPSAmount: number;
  beginVestedAmount: number;
  currentVestedAmount: number;
  currentEtva: number;
  previousEtva: number;
  employmentStatus?: string;
  department: string;
  payClassification: string;
  gender: string;
  phoneNumber: string;
  workLocation: string;
  receivedContributionsLastYear: boolean;
  fullTimeDate: string;
  terminationReason: string;
  missives: number[] | null;
  allocationFromAmount: number;
  allocationToAmount: number;
  badgesOfDuplicateSsns: number[];
}

export interface EligibleEmployeesRequestDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface WagesCurrentYearParticipant {
  badgeNumber: number;
  incomeCurrentYear: number;
  hoursCurrentYear: number;
  storeNumber: number;
  isExecutive: boolean;
}

export interface EmployeeWagesForYear {
  badgeNumber: number;
  incomeCurrentYear: number;
  hoursCurrentYear: number;
  storeNumber: number;
  isExecutive: boolean;
}

export interface EmployeeWagesForYearRequestDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
  useFrozenData?: boolean;
}

export interface EmployeeWagesForYearResponse extends PagedReportResponse<EmployeeWagesForYear> {
  totalHoursCurrentYearWages?: number;
  totalIncomeCurrentYearWages?: number;
}

export interface EligibleEmployee {
  oracleHcmId: number;
  badgeNumber: number;
  fullName: string;
  departmentId: number;
  department: string;
  storeNumber: number;
  isExecutive: boolean;
}

export interface EligibleEmployeeResponseDto extends PagedReportResponse<EligibleEmployee> {
  numberReadOnFrozen: number;
  numberNotSelected: number;
  numberWritten: number;
}

export interface YearEndProfitSharingReportRequest extends FilterParams {
  profitYear: number;
  useFrozenData?: boolean;
  pagination: SortedPaginationRequestDto;
}

export interface YearEndProfitSharingEmployee {
  badgeNumber: number;
  employeeName: string;
  storeNumber: number;
  employeeTypeCode: string;
  employmentTypeName: string;
  dateOfBirth: Date;
  age: string;
  ssn: string;
  wages: number;
  hours: number;
  points: number;
  isUnder21: boolean;
  isNew: boolean;
  employeeStatus: string;
  balance: number;
  yearsInPlan: number;
  terminationDate: Date | null;
}

export interface YearEndProfitSharingReportResponse extends PagedReportResponse<YearEndProfitSharingEmployee> {
  wagesTotal: number;
  hoursTotal: number;
  pointsTotal: number;
  balanceTotal: number;
  numberOfEmployees: number;
  numberOfNewEmployees: number;
  numberOfEmployeesUnder21: number;
  numberOfEmployeesInPlan: number;
}

export interface YearEndProfitSharingReportTotalsResponse {
  wagesTotal: number;
  hoursTotal: number;
  pointsTotal: number;
  balanceTotal: number;
  numberOfEmployees: number;
  numberOfNewEmployees: number;
  numberOfEmployeesUnder21: number;
  numberOfEmployeesInPlan: number;
}

export interface YearEndProfitSharingReportSummaryLineItem {
  subgroup: string;
  lineItemPrefix: string;
  lineItemTitle: string;
  numberOfMembers: number;
  totalWages: number;
  totalBalance: number;
  totalHours: number;
  totalPoints: number;
  totalPriorBalance: number;
}

export interface YearEndProfitSharingReportSummaryResponse {
  lineItems: YearEndProfitSharingReportSummaryLineItem[];
}

export interface ProfitSharingLabelsRequest extends FrozenProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface ProfitSharingLabel {
  storeNumber: number;
  payClassificationId: string; // changed from number to string per backend
  payClassificationName: string;
  departmentId: number;
  departmentName: string;
  badgeNumber: number;
  employeeName: string;
  firstName: string;
  address1: string;
  city: string;
  state: string;
  postalCode: string;
}
