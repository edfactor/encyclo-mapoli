import { Paged, PaginationParams } from "smart-ui-library";

export enum ImpersonationRoles {
  FinanceManager = "Finance-Manager",
  DistributionsClerk = "Distribution-Clerk",
  HardshipAdministrator = "Hardship-Administrator",
  ProfitSharingAdministrator = "Profit-Sharing-Administrator"
}

interface ImpersonationRequest {
  impersonation: ImpersonationRoles;
}

export interface DemographicBadgesNotInPayprofitResponse {
  reportName: string;
  reportDate: string;
  response: Paged<DemographicBadgesNotInPayprofit>;
}

export interface DemographicBadgesNotInPayprofit {
  employeeBadge: number;
  employeeSsn: number;
  empoyeeName: string;
  store: number;
  status: string;
}

export interface DemographicBadgesNotInPayprofitRequestDto extends ImpersonationRequest {
  pagination: PaginationParams;
}

export interface PagedReportResponse<T> {
  reportName: string;
  reportDate: string;
  response: Paged<T>;
}

export interface DistributionsAndForfeituresRequestDto extends ImpersonationRequest {
  startMonth?: number;
  endMonth?: number;
  includeOutgoingForfeitures?: boolean;
  profitYear: number;
  pagination: PaginationParams;
}

export interface DistributionsAndForfeitures {
  badgeNumber: number;
  employeeName: string;
  employeeSsn: string;
  loanDate: string;
  distributionAmount: number;
  stateTax: number;
  federalTax: number;
  forfeitAmount: number;
  age: number;
  taxCode: string | null;
  otherName: string | null;
  otherSsn: string | null;
  enrolled: boolean | null;
}

export interface DuplicateSSNAddress {
  street: string;
  street2: string | null;
  city: string;
  state: string;
  postalCode: string;
  countryIso: string;
}

export interface DuplicateSSNDetail {
  badgeNumber: number;
  ssn: string;
  name: string;
  address: DuplicateSSNAddress;
  hireDate: string;
  terminationDate: string | null;
  rehireDate: string | null;
  status: string;
  storeNumber: number;
  profitSharingRecords: number;
  hoursCurrentYear: number;
  hoursLastYear: number;
  incomeCurrentYear: number;
}

export interface DuplicateSSNsRequestDto extends ImpersonationRequest {
  profitYear: number;
  pagination: PaginationParams;
}

export interface MissingCommasInPYNameRequestDto extends ImpersonationRequest {
  pagination: PaginationParams;
}

export interface MissingCommasInPYName {
  exployeeBadge: number;
  employeeSsn: number;
  employeeName: string;
}

export interface DuplicateNameAndBirthdayRequestDto extends ImpersonationRequest {
  profitYear: number;
  pagination: PaginationParams;
}

export interface NegativeEtvaForSSNsOnPayprofitRequestDto extends ImpersonationRequest {
  profitYear: number;
  pagination: PaginationParams;
}

export interface DuplicateNameBirthdayAddress {
  street: string;
  street2: string | null;
  city: string;
  state: string;
  postalCode: string;
  countryIso: string;
}

export interface DuplicateNameAndBirthday {
  badgeNumber: number;
  ssn: string;
  name: string;
  dateOfBirth: string;
  address: DuplicateNameBirthdayAddress;
  years: number;
  hireDate: string;
  terminationDate: string | null;
  status: string;
  storeNumber: number;
  count: number;
  netBalance: number;
  hoursCurrentYear: number;
  incomeCurrentYear: number;
}

export interface NegativeEtvaForSSNsOnPayProfit {
  employeeBadge: number;
  employeeSsn: number;
  etvaValue: number;
}

export interface MilitaryAndRehireRequestDto extends ImpersonationRequest {
  pagination: PaginationParams;
}

export interface MilitaryAndRehire {
  departmentId: number;
  badgeNumber: number;
  ssn: string;
  fullName: string;
  dateOfBirth: string;
  terminationDate: string;
}

export interface MilitaryAndRehireForfeituresRequestDto extends ImpersonationRequest {
  reportingYear: string;
  profitYear: number;
  pagination: PaginationParams;
}

export interface ForfeitureDetail {
  profitYear: number;
  forfeiture: number;
  remark: string;
}

export interface MilitaryAndRehireForfeiture {
  badgeNumber: number;
  fullName: string;
  ssn: string;
  reHiredDate: string;
  companyContributionYears: number;
  hoursCurrentYear: number;
  details: ForfeitureDetail[];
}

export interface MilitaryAndRehireProfitSummaryRequestDto extends ImpersonationRequest {
  reportingYear: string;
  profitYear: number;
  pagination: PaginationParams;
}

export interface MilitaryAndRehireProfitSummary {
  badgeNumber: number;
  fullName: string;
  ssn: string;
  storeNumber: number;
  hireDate: string;
  terminationDate: string;
  reHiredDate: string;
  companyContributionYears: number;
  hoursCurrentYear: number;
  profitYear: number;
  forfeiture: number;
  remark: string;
  enrollmentId: number;
  netBalanceLastYear: number;
  vestedBalanceLastYear: number;
  employmentStatusId: string;
  profitCodeId: number;
}

export interface ExecutiveHoursAndDollarsRequestDto extends ImpersonationRequest {
  badgeNumber?: number;
  fullNameContains?: string;
  profitYear: number;
  hasExecutiveHoursAndDollars: boolean;
  pagination: PaginationParams;
}

export interface ExecutiveHoursAndDollars {
  badgeNumber: number;
  fullName: string;
  storeNumber: number;
  hoursExecutive: number;
  incomeExecutive: number;
  currentHoursYear: number;
  currentIncomeYear: number;
  payFrequencyId: number;
  employmentStatusId: string;
}

export interface EligibleEmployeesRequestDto extends ImpersonationRequest {
  profitYear: number;
  pagination: PaginationParams;
}

export interface EligibleEmployee {
  oracleHcmId: number;
  badgeNumber: number;
  fullName: string;
}

export interface EligibleEmployeeResponseDto {
  numberReadOnFrozen: number;
  numberNotSelected: number;
  numberWritten: number;
  reportName: string;
  reportDate: string;
  response: Paged<EligibleEmployee>;
}

export interface MasterInquiryDetail {
  id: number;
  ssn: number;
  profitYear: number;
  profitYearIteration: number;
  distributionSequence: number;
  profitCodeId: number;
  contribution: number;
  earnings: number;
  forfeiture: number;
  monthToDate: number;
  yearToDate: number;
  remark?: string;
  zeroContributionReasonId?: number;
  federalTaxes: number;
  stateTaxes: number;
  taxCodeId?: string;
  commentTypeId?: number;
  commentRelatedCheckNumber?: number;
  commentRelatedState?: string;
  commentRelatedOracleHcmId?: number;
  commentRelatedPsnSuffix?: number;
  commentIsPartialTransaction?: boolean;
}

export interface MasterInquryRequest extends ImpersonationRequest {
  startProfitYear?: number;
  endProfitYear?: number;
  startProfitMonth?: number;
  endProfitMonth?: number;
  profitCode?: number;
  contributionAmount?: number;
  earningsAmount?: number;
  forfeitureAmount?: number;
  paymentAmount?: number;
  socialSecurity?: number;
  comment?: string
  pagination: PaginationParams;
}
export enum FrozenReportsByAgeRequestType {
  Total = "Total",
  FullTime = "FullTime",
  PartTime = "PartTime",
}


export interface FrozenReportsByAgeRequest extends ImpersonationRequest {
  profitYear: number;
  pagination: PaginationParams;
  reportType: FrozenReportsByAgeRequestType;
}

export interface ProfitSharingDistributionsByAge {
  reportName: string;
  reportDate: string;
  reportType: FrozenReportsByAgeRequestType;
  hardshipTotalEmployees: number;
  regularTotalAmount:number;
  regularTotalEmployees: number;
  hardshipTotalAmount: number;
  distributionTotalAmount: number;
  response: Paged<ProfitSharingDistributionsByAgeResponse>;
}

export interface ProfitSharingDistributionsByAgeResponse {
  age: number;
  employeeCount: number;
  amount: number;
  employmentType: string;
  commentTypeId: number | null;
}

export interface ContributionsByAge {
  reportName: string;
  reportDate: string;
  reportType: FrozenReportsByAgeRequestType;
  totalEmployees: number;
  distributionTotalAmount: number;
  response: Paged<ContributionsByAgeDetail>;
}

export interface ContributionsByAgeDetail {
  age: number;
  employeeCount: number;
  amount: number;
}

export interface ForfeituresByAge {
  reportName: string;
  reportDate: string;
  reportType: FrozenReportsByAgeRequestType;
  totalEmployees: number;
  distributionTotalAmount: number;
  response: Paged<ForfeituresByAgeDetail>;
}

export interface ForfeituresByAgeDetail {
  age: number;
  employeeCount: number;
  amount: number;
}

export interface EmployeeDetails {
	firstName: string;
	lastName: string;
  address: string;
	addressCity: string;
	addressState: string;
	addressZipCode: string;
	dateOfBirth: string;
	ssn: string;
	yearToDateProfitSharingHours: number;
	yearsInPlan: number;
	percentageVested: number;
	contributionsLastYear: boolean;
	enrolled: boolean;
	employeeId: string;
  hireDate: string;
  terminationDate: string | null;
  reHireDate: string | null;
  storeNumber: number;
  beginPSAmount: number;
  currentPSAmount: number;
  beginVestedAmount: number;
  currentVestedAmount: number;
}

export interface MasterInquiryResponseType {
  employeeDetails: EmployeeDetails | null;
  inquiryResults: Paged<MasterInquiryDetail>;
}
export interface BalanceByAge {
  reportName: string;
  reportDate: string;
  reportType: FrozenReportsByAgeRequestType;
  balanceTotalAmount: number;
  vestedTotalAmount : number;
  totalMembers: number;
  totalBeneficiaries: number;
  totalBeneficiariesAmount : number;
  totalBeneficiariesVestedAmount : number;
  totalNonBeneficiaries: number;
  totalNonBeneficiariesAmount: number;
  totalNonBeneficiariesVestedAmount : number;
  response: Paged<BalanceByAgeDetail>;
}

export interface BalanceByAgeDetail {
  age: number;
  employeeCount: number;
  currentBalance: number;
  vestedBalance: number;
  beneficiaryCount : number;
}
