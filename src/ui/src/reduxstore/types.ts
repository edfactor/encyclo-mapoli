import { ISortParams, Paged, PaginationParams } from "smart-ui-library";

export enum ImpersonationRoles {
  FinanceManager = "Finance-Manager",
  DistributionsClerk = "Distributions-Clerk",
  HardshipAdministrator = "Hardship-Administrator",
  ProfitSharingAdministrator = "Profit-Sharing-Administrator",
  ItSupport = "IT-Support",
}

export interface SortedPaginationRequestDto extends PaginationParams, ISortParams {}

export interface ProfitYearRequest {
  profitYear: number;
}

export interface DemographicBadgesNotInPayprofitResponse {
  reportName: string;
  reportDate: string;
  response: Paged<DemographicBadgesNotInPayprofit>;
}

export interface DemographicBadgesNotInPayprofit {
  badgeNumber: number;
  ssn: number;
  empoyeeName: string;
  store: number;
  status: string;
  statusName: string;
}

export interface DemographicBadgesNotInPayprofitRequestDto {
  pagination: PaginationParams;
}

export interface PagedReportResponse<T> {
  reportName: string;
  reportDate: string;
  response: Paged<T>;
}

export interface DistributionsAndForfeituresRequestDto extends ProfitYearRequest {
  startMonth?: number;
  endMonth?: number;
  includeOutgoingForfeitures?: boolean;
  pagination: PaginationParams;
}

export interface DistributionsAndForfeitures {
  badgeNumber: number;
  employeeName: string;
  ssn: string;
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

export interface DuplicateSSNsRequestDto {
  pagination: PaginationParams;
}

export interface MissingCommasInPYNameRequestDto {
  pagination: PaginationParams;
}

export interface MissingCommasInPYName {
  exployeeBadge: number;
  ssn: number;
  employeeName: string;
}

export interface DuplicateNameAndBirthdayRequestDto extends ProfitYearRequest {
  pagination: PaginationParams;
}

export interface NegativeEtvaForSSNsOnPayprofitRequestDto extends ProfitYearRequest {
  pagination: PaginationParams;
}

export interface EmployeeWagesForYearRequestDto extends ProfitYearRequest {
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
  badgeNumber: number;
  ssn: number;
  etvaValue: number;
}

export interface EmployeesOnMilitaryLeaveRequestDto {
  pagination: PaginationParams;
}

export interface EmployeesOnMilitaryLeaveResponse {
  departmentId: number;
  badgeNumber: number;
  ssn: string;
  fullName: string;
  dateOfBirth: string;
  terminationDate: string;
}

export interface MilitaryAndRehireForfeituresRequestDto extends ProfitYearRequest {
  reportingYear: string;
  pagination: PaginationParams;
}

export interface ForfeitureDetail extends ProfitYearRequest {
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

export interface ExecutiveHoursAndDollarsRequestDto extends ProfitYearRequest {
  badgeNumber?: number;
  socialSecurity?: number;
  fullNameContains?: string;
  hasExecutiveHoursAndDollars: boolean;
  isMonthlyPayroll: boolean;
  pagination: PaginationParams;
}

// This is the state of an editable executive's hours and dollars
export interface ExecutiveHoursAndDollarsRow {
  badgeNumber: number;
  executiveHours: number;
  executiveDollars: number;
}

// This structure is used to store the state of the Manage
// Executive Hours and Dollars Grid, which allows editing
// and writing of data
export interface ExecutiveHoursAndDollarsGrid {
  executiveHoursAndDollars: ExecutiveHoursAndDollarsRow[];
  profitYear: number | null;
}

export interface ExecutiveHoursAndDollars {
  badgeNumber: number;
  fullName: string;
  storeNumber: number;
  socialSecurity: number;
  hoursExecutive: number;
  incomeExecutive: number;
  currentHoursYear: number;
  currentIncomeYear: number;
  payFrequencyId: number;
  employmentStatusId: string;
}

export interface EmployeeWagesForYear {
  badgeNumber: number;
  hoursExecutive: number;
  incomeExecutive: number;
}

export interface EligibleEmployeesRequestDto extends ProfitYearRequest {
  pagination: PaginationParams;
}

export interface EligibleEmployee {
  oracleHcmId: number;
  badgeNumber: number;
  fullName: string;
  departmentId: number;
  department: string;
}

export interface EligibleEmployeeResponseDto {
  numberReadOnFrozen: number;
  numberNotSelected: number;
  numberWritten: number;
  reportName: string;
  reportDate: string;
  response: Paged<EligibleEmployee>;
}

export interface ForfeituresAndPointsQueryParams extends ProfitYearRequest {
  useFrozenData: boolean;
}

export interface ProfitAndReportingQueryParams extends ProfitYearRequest {
  reportingYear: string;
}
export interface ExecutiveHoursAndDollarsQueryParams extends ProfitYearRequest {
  badgeNumber: number;
  socialSecurity: number;
  fullNameContains: string;
  hasExecutiveHoursAndDollars: boolean;
  isMonthlyPayroll: boolean;
}

export interface DistributionsAndForfeituresQueryParams extends ProfitYearRequest {
  startMonth?: number;
  endMonth?: number;
  includeOutgoingForfeitures?: boolean;
}

export interface BaseDateRangeParams {
  startDate: Date;
  endDate: Date;
}
export interface MasterInquirySearch {
  startProfitYear?: Date | null;
  endProfitYear?: Date | null;
  startProfitMonth?: number | null;
  endProfitMonth?: number | null;
  socialSecurity?: number | null;
  name?: string | null;
  badgeNumber?: number | null;
  comment?: string | null;
  paymentType: "all" | "hardship" | "payoffs" | "rollovers";
  memberType: "all" | "employees" | "beneficiaries" | "none";
  contribution?: number | null;
  earnings?: number | null;
  forfeiture?: number | null;
  payment?: number | null;
  voids: boolean;
  pagination: SortedPaginationRequestDto;
}

export interface MasterInquiryDetail extends ProfitYearRequest {
  id: number;
  ssn: number;
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
  badgeNumber?: number;
  federalTaxes: number;
  stateTaxes: number;
  taxCodeId?: string;
  commentTypeId?: number;
  commentRelatedCheckNumber?: number;
  commentRelatedState?: string;
  commentRelatedOracleHcmId?: number;
  commentRelatedPsnSuffix?: number;
  commentIsPartialTransaction?: boolean;
  profitCodeName?: string;
  zeroContributionReasonName?: string;
  taxCodeName?: string;
  commentTypeName?: string;
}

export interface MasterInquiryRequest {
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
  name?: string;
  comment?: string;
  paymentType?: number;
  memberType?: number;
  badgeNumber?: number;
  pagination: SortedPaginationRequestDto;
}

export enum FrozenReportsByAgeRequestType {
  Total = "Total",
  FullTime = "FullTime",
  PartTime = "PartTime"
}

export interface FrozenReportsByAgeRequest extends ProfitYearRequest {
  pagination: PaginationParams;
  reportType: FrozenReportsByAgeRequestType;
}
export interface FrozenReportsForfeituresAndPointsRequest extends ProfitYearRequest {
  pagination: PaginationParams;
  useFrozenData: boolean;
}
export interface ProfitSharingDistributionsByAge {
  reportName: string;
  reportDate: string;
  reportType: FrozenReportsByAgeRequestType;
  hardshipTotalEmployees: number;
  regularTotalAmount: number;
  regularTotalEmployees: number;
  hardshipTotalAmount: number;
  distributionTotalAmount: number;
  totalEmployees: number;
  bothHardshipAndRegularEmployees: number;
  bothHardshipAndRegularAmount: number;
  response: Paged<ProfitSharingDistributionsByAgeResponse>;
}

export interface ProfitSharingDistributionsByAgeResponse {
  age: number;
  employeeCount: number;
  regularEmployeeCount: number;
  hardshipEmployeeCount: number;
  amount: number;
  hardshipAmount: number;
  regularAmount: number;
  employmentType: string;
  commentTypeId: number | null;
}

export interface ContributionsByAge {
  reportName: string;
  reportDate: string;
  reportType: FrozenReportsByAgeRequestType;
  totalEmployees: number;
  totalAmount: number;
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
  totalAmount: number;
  response: Paged<ForfeituresByAgeDetail>;
}

export interface ForfeituresAndPointsDetail {
  badgeNumber: number;
  employeeName: string;
  ssn: string;
  forfeitures: number;
  forfeiturePoints: number;
  earningPoints: number;
  benefificaryPsn: number;
}
export interface ForfeituresAndPoints {
  reportName: string;
  reportDate: string;
  useFrozenData: boolean;
  response: Paged<ForfeituresAndPointsDetail>;
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
  enrollmentId: number;
  enrollment: string;
  badgeNumber: string;
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
}

export interface MasterInquiryResponseType {
  employeeDetails: EmployeeDetails | null;
  inquiryResults: Paged<MasterInquiryDetail>;
}
export interface BalanceByDetailBase {
  employeeCount: number;
  currentBalance: number;
  beneficiaryCount?: number;
  vestedBalance?: number;
  currentBeneficiaryBalance?: number;
  currentBeneficiaryVestedBalance?: number;
  fullTimeCount?: number;
  partTimeCount?: number;
}

export interface BalanceByBase<TDetail extends BalanceByDetailBase> {
  reportName: string;
  reportDate: string;
  reportType: FrozenReportsByAgeRequestType;
  balanceTotalAmount: number;
  vestedTotalAmount?: number;
  totalMembers: number;
  totalBeneficiaries: number;
  totalBeneficiariesAmount?: number;
  totalBeneficiariesVestedAmount?: number;
  totalEmployee: number; // Derived
  totalEmployeeAmount: number; // Derived
  totalEmployeesVestedAmount: number; // Derived
  totalFullTimeCount?: number;
  totalPartTimeCount?: number;
  response: Paged<TDetail>;
}

export interface BalanceByAgeDetail extends BalanceByDetailBase {
  age: number;
}

export interface BalanceByAge extends BalanceByBase<BalanceByAgeDetail> {}

export interface BalanceByYearsDetail extends BalanceByDetailBase {
  age: number;
}

export interface BalanceByYears extends BalanceByBase<BalanceByAgeDetail> {}

export interface VestedAmountsByAge {
  totalFullTime100PercentAmount: number;
  totalFullTimePartialAmount: number;
  totalFullTimeNotVestedAmount: number;
  totalPartTime100PercentAmount: number;
  totalPartTimePartialAmount: number;
  totalPartTimeNotVestedAmount: number;
  totalBeneficiaryCount: number;
  totalBeneficiaryAmount: number;
  totalFullTimeCount: number;
  totalNotVestedCount: number;
  totalPartialVestedCount: number;
  reportName: string;
  reportDate: string;
  response: Paged<VestedAmountsByAgeDetail>;
}
export interface VestedAmountsByAgeDetail {
  age: number;
  fullTime100PercentCount: number;
  fullTime100PercentAmount: number;
  fullTimePartialCount: number;
  fullTimePartialAmount: number;
  fullTimeNotVestedCount: number;
  fullTimeNotVestedAmount: number;
  partTime100PercentCount: number;
  partTime100PercentAmount: number;
  partTimePartialCount: number;
  partTimePartialAmount: number;
  partTimeNotVestedCount: number;
  partTimeNotVestedAmount: number;
  beneficiaryCount: number;
  beneficiaryAmount: number;
  fullTimeCount: number;
  notVestedCount: number;
  partialVestedCount: number;
}

export interface TerminationRequest {
  profitYear: number;
  pagination: PaginationParams;
}

export interface TerminationDetail {
  badgeNumber: number;
  psnSuffix: number;
  name: string;
  beginningBalance: number;
  beneficiaryAllocation: number;
  distributionAmount: number;
  forfeit: number;
  endingBalance: number;
  vestedBalance: number;
  dateTerm: string;
  ytdPsHours: number;
  vestedPercent: number;
  age: number;
  enrollmentCode: number;
}

export interface TerminationResponse {
  totalVested: number;
  totalForfeit: number;
  totalEndingBalance: number;
  totalBeneficiaryAllocation: number;
  reportName: string;
  reportDate: string;
  response: Paged<TerminationDetail[]>;
}

export interface ProfitShareUpdateRequest {
  profitYear: number;
  contributionPercent: number;
  earningsPercent: number;
  incomingForfeiturePercent: number;
  maxAllowedContributions: number;

  adjustmentBadge: number;
  adjustmentContributionAmount: number;
  adjustmentEarningsAmount: number;
  adjustmentIncomingForfeitureAmount: number;

  adjustmentSecondaryBadge: number;
  adjustmentSecondaryEarningsAmount: number;
}

export interface ProfitShareUpdateDetail {
  badgePSn: string;
  name: string;
  beginningBalance: number;
  beneficiaryAllocation: number;
  distributionAmount: number;
  forfeit: number;
  endingBalance: number;
  vestedBalance: number;
  dateTerm: string;
  ytdPsHours: number;
  vestedPercent: number;
  age: number;
  enrollmentCode: number;
}

export interface ProfitShareUpdateResponse {
  isLoading: boolean;
  totalVested: number;
  totalForfeit: number;
  totalEndingBalance: number;
  totalBeneficiaryAllocation: number;
  reportName: string;
  reportDate: string;
  response: Paged<ProfitShareUpdateDetail[]>;
}

export interface ProfitShareEditDetail {
  badgePSn: string;
  name: string;
  beginningBalance: number;
  beneficiaryAllocation: number;
  distributionAmount: number;
  forfeit: number;
  endingBalance: number;
  vestedBalance: number;
  dateTerm: string;
  ytdPsHours: number;
  vestedPercent: number;
  age: number;
  enrollmentCode: number;
}

export interface ProfitShareEditResponse {
  isLoading: boolean; // this feels like a hack, it means display the table with the spinner.

  beginningBalance: number;
  contributionGrandTotal: number;
  incomingForfeitureGrandTotal: number;
  earningsGrandTotal: number;

  reportName: string;
  reportDate: string;
  response: Paged<ProfitShareEditDetail[]>;
}

export interface ProfitShareMasterResponse {
  isLoading: boolean;
  reportName: string;
  beneficiariesEffected?: number;
  employeesEffected?: number;
  etvasEffected?: number;
}

export interface YearEndProfitSharingReportResponse {
  badgeNumber: number;
  employeeName: string;
  storeNumber: number;
  employeeTypeCode: string;
  employmentTypeName: string;
  dateOfBirth: Date;
  age: number;
  ssn: string;
  wages: number;
  hours: number;
  points: number;
  isUnder21: boolean;
  isNew: boolean;
  employeeStatus: string;
  balance: number;
  yearsInPlan: number;
}
export interface FrozenStateResponse {
  id: number;
  profitYear: number;
  frozenBy: string;
  asOfDateTime: string;
  createdDateTime: string;
  isActive: boolean;
}

interface FreezeDemographicsRequest {
  asOfDateTime: string;
  profitYear: number;
}

export interface ProfallData {
  badge: number;
  employeeName: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
}

export interface MilitaryContributionRequest extends ProfitYearRequest {
  badgeNumber: number;
  pagination: PaginationParams;
}

export interface YearEndProfitSharingReportRequest {
  isYearEnd: boolean;
  minimumAgeInclusive: number;
  maximumAgeInclusive: number;
  minimumHoursInclusive: number;
  maximumHoursInclusive: number;
  includeActiveEmployees: boolean;
  includeInactiveEmployees: boolean;
  includeEmployeesTerminatedThisYear: boolean;
  includeTerminatedEmployees: boolean;
  includeBeneficiaries: boolean;
  includeEmployeesWithPriorProfitSharingAmounts: boolean;
  includeEmployeesWithNoPriorProfitSharingAmounts: boolean;
  profitYear: number;
  pagination: PaginationParams;
}

export interface CreateMilitaryContributionRequest extends ProfitYearRequest {
  badgeNumber: number;
  contributionAmount: number;
}

export interface MilitaryContribution {
  contributionDate: Date | null;
  contributionAmount: number | null;
}
export interface YearEndProfitSharingEmployee {
  badgeNumber: number;
  employeeName: string;
  storeNumber: number;
  employeeTypeCode: string;
  employmentTypeName: string;
  dateOfBirth: Date;
  age: number;
  ssn: string;
  wages: number;
  hours: number;
  points: number;
  isUnder21: boolean;
  isNew: boolean;
  employeeStatus: string;
  balance: number;
  yearsInPlan: number;
}
