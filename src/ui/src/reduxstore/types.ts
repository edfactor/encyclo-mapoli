import { ICellRendererParams, IHeaderParams } from "ag-grid-community";
import { ISortParams, Paged, PaginationParams } from "smart-ui-library";

export enum ImpersonationRoles {
  FinanceManager = "Finance-Manager",
  DistributionsClerk = "Distributions-Clerk",
  HardshipAdministrator = "Hardship-Administrator",
  ProfitSharingAdministrator = "Profit-Sharing-Administrator",
  ItOperations = "IT-Operations"
}

export interface SortedPaginationRequestDto extends PaginationParams, ISortParams {}

export interface ProfitYearRequest {
  profitYear: number;
}

export interface YearRangeRequest {
  beginProfitYear: number;
  endProfitYear: number;
}

export interface CalendarResponseDto {
  fiscalBeginDate: string;
  fiscalEndDate: string;
}

export interface FrozenProfitYearRequest extends ProfitYearRequest {
  useFrozenData: boolean;
}

export interface BadgeNumberRequest extends FrozenProfitYearRequest {
  badgeNumber: number | null;
}

export interface ReportsByAgeParams extends ProfitYearRequest {
  reportType: FrozenReportsByAgeRequestType;
}

export type DemographicBadgesNotInPayprofitResponse = PagedReportResponse<DemographicBadgesNotInPayprofit>;

export interface DemographicBadgesNotInPayprofit {
  badgeNumber: number;
  ssn: number;
  employeeName: string;
  store: number;
  status: string;
  statusName: string;
}

export interface DemographicBadgesNotInPayprofitRequestDto {
  pagination: SortedPaginationRequestDto;
}

export interface PagedReportResponse<T> {
  reportName: string;
  reportDate: string;
  startDate: string;
  endDate: string;
  dataSource: string;
  response: Paged<T>;
}

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
  employmentStatusName: string;
  storeNumber: number;
  profitSharingRecords: number;
  hoursCurrentYear: number;
  hoursLastYear: number;
  incomeCurrentYear: number;
}

export interface DuplicateSSNsRequestDto {
  pagination: SortedPaginationRequestDto;
}

export interface MissingCommasInPYNameRequestDto {
  pagination: SortedPaginationRequestDto;
}

export interface MissingCommasInPYName {
  badgeNumber: number;
  ssn: number;
  employeeName: string;
}

export interface DuplicateNameAndBirthdayRequestDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface NegativeEtvaForSSNsOnPayprofitRequestDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface EmployeeWagesForYearRequestDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface GrossWagesReportDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
  minGrossAmount?: number;
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
  employmentStatusName: string;
}

export interface NegativeEtvaForSSNsOnPayProfit {
  badgeNumber: number;
  ssn: number;
  etvaValue: number;
}

export interface EmployeesOnMilitaryLeaveRequestDto {
  pagination: SortedPaginationRequestDto;
}

export interface EmployeesOnMilitaryLeaveResponse {
  departmentId: number;
  badgeNumber: number;
  ssn: string;
  fullName: string;
  dateOfBirth: string;
  terminationDate: string;
}

export interface StartAndEndDateRequest extends ProfitYearRequest {
  beginningDate: string;
  endingDate: string;
  pagination: SortedPaginationRequestDto;
  excludeZeroBalance?: boolean;
}

export interface ForfeitureDetail extends ProfitYearRequest {
  forfeiture: number;
  remark: string;
  hoursCurrentYear: number;
  wages: number;
  enrollmentId: number;
  enrollmentName: string;
}

export interface MilitaryAndRehireForfeiture {
  badgeNumber: number;
  fullName: string;
  ssn: string;
  reHiredDate: string;
  companyContributionYears: number;
  employmentStatus: string;
  netBalanceLastYear: number;
  vestedBalanceLastYear: number;
  hireDate: string;
  terminationDate: string;
  storeNumber: number;
  details: ForfeitureDetail[];
}

export interface ExecutiveHoursAndDollarsRequestDto extends ProfitYearRequest {
  badgeNumber?: number;
  socialSecurity?: number;
  fullNameContains?: string;
  hasExecutiveHoursAndDollars: boolean;
  isMonthlyPayroll: boolean;
  pagination: SortedPaginationRequestDto;
}

// This is the state of an editable executive's hours and dollars
export interface ExecutiveHoursAndDollarsRow {
  badgeNumber: number;
  executiveHours: number;
  executiveDollars: number;
}

// This structure is used to store the state of the Manage
// Executive Hours and Dollars Grid; which allows editing
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
  payFrequencyName: string;
  employmentStatusId: string;
  employmentStatusName: string;
}

export interface EmployeeWagesForYear {
  badgeNumber: number;
  hoursExecutive: number;
  incomeExecutive: number;
}

export interface EligibleEmployeesRequestDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface EligibleEmployee {
  oracleHcmId: number;
  badgeNumber: number;
  fullName: string;
  departmentId: number;
  department: string;
  storeNumber: number;
}

export interface EligibleEmployeeResponseDto extends PagedReportResponse<EligibleEmployee> {
  numberReadOnFrozen: number;
  numberNotSelected: number;
  numberWritten: number;
}

export interface ForfeituresAndPointsQueryParams extends ProfitYearRequest {
  useFrozenData: boolean;
}

export interface ExecutiveHoursAndDollarsQueryParams extends ProfitYearRequest {
  badgeNumber: number;
  socialSecurity: number;
  fullNameContains: string;
  hasExecutiveHoursAndDollars: boolean;
  isMonthlyPayroll: boolean;
}

export interface DistributionsAndForfeituresQueryParams extends ProfitYearRequest {
  startDate?: string;
  endDate?: string;
}

export interface MasterInquirySearch {
  endProfitYear?: number | null;
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
  isEmployee: boolean;
  ssn: string;
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
  commentRelatedCheckNumber?: string;
  commentRelatedState?: string;
  commentRelatedOracleHcmId?: number;
  commentRelatedPsnSuffix?: number;
  commentIsPartialTransaction?: boolean;
  profitCodeName?: string;
  zeroContributionReasonName?: string;
  taxCodeName?: string;
  commentTypeName?: string;
  payFrequencyId?: number;
  transactionDate?: Date;
  currentIncomeYear?: number;
  currentHoursYear?: number;
  psnSuffix?: number;
}

export interface MasterInquiryRequest {
  id?: number;
  memberType?: number;
  badgeNumber?: number;
  psnSuffix?: number;
  ssn?: number;
  profitYear?: number;
  endProfitYear?: number;
  startProfitMonth?: number;
  endProfitMonth?: number;
  profitCode?: number;
  contributionAmount?: number;
  earningsAmount?: number;
  forfeitureAmount?: number;
  paymentAmount?: number;
  name?: string;
  paymentType?: number;
  pagination: SortedPaginationRequestDto;
  _timestamp?: number;
}

export interface MasterInquiryMemberRequest {
  memberType?: number;
  id?: number;
  profitYear?: number;
  skip?: number;
  take?: number;
  sortBy?: string;
  isSortDescending?: boolean;
}

export interface MasterInquiryResponseDto {
  isEmployee: boolean;
  id: number;
  ssn: string;
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
  commentRelatedCheckNumber?: string;
  commentRelatedState?: string;
  commentRelatedOracleHcmId?: number;
  commentRelatedPsnSuffix?: number;
  commentIsPartialTransaction?: boolean;
  badgeNumber?: number;
  profitCodeName?: string;
  zeroContributionReasonName?: string;
  taxCodeName?: string;
  commentTypeName?: string;
  psnSuffix: number;
  payment?: number;
  vestedBalance?: number;
  vestingPercent?: number;
  currentBalance?: number;
  payFrequencyId: number;
  transactionDate: string;
  currentIncomeYear: number;
  currentHoursYear: number;
  employmentStatusId: string;
  employmentStatus?: string;
}

export interface GroupedProfitSummaryDto {
  profitYear: number;
  monthToDate: number;
  totalContribution: number;
  totalEarnings: number;
  totalForfeiture: number;
  totalPayment: number;
  transactionCount: number;
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
  pagination: SortedPaginationRequestDto;
  useFrozenData: boolean;
}

export interface ProfitSharingDistributionsByAge extends PagedReportResponse<ProfitSharingDistributionsByAgeResponse> {
  reportType: FrozenReportsByAgeRequestType;
  hardshipTotalEmployees: number;
  regularTotalAmount: number;
  regularTotalEmployees: number;
  hardshipTotalAmount: number;
  distributionTotalAmount: number;
  totalEmployees: number;
  bothHardshipAndRegularEmployees: number;
  bothHardshipAndRegularAmount: number;
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

export interface ContributionsByAge extends PagedReportResponse<ContributionsByAgeDetail> {
  reportType: FrozenReportsByAgeRequestType;
  totalEmployees: number;
  totalAmount: number;
}

export interface ContributionsByAgeDetail {
  age: number;
  employeeCount: number;
  amount: number;
}

export interface ForfeituresByAge extends PagedReportResponse<ForfeituresByAgeDetail> {
  reportType: FrozenReportsByAgeRequestType;
  totalEmployees: number;
  totalAmount: number;
}

export interface ForfeituresAndPointsDetail {
  badgeNumber: number;
  employeeName: string;
  ssn: string;
  forfeitures: number;
  forfeitPoints: number;
  earningPoints: number;
  beneficiaryPsn: number;
}

export interface ForfeituresAndPoints extends PagedReportResponse<ForfeituresAndPointsDetail> {
  useFrozenData: boolean;
  totalEarningPoints: number;
  totalForfeitPoints: number;
  totalForfeitures: number;
  totalProfitSharingBalance: number;
  distributionTotals: number;
  allocationToTotals: number;
  allocationsFromTotals: number;
}

export interface ForfeituresByAgeDetail {
  age: number;
  employeeCount: number;
  amount: number;
}

export interface EmployeeDetails {
  id: number;
  badgeNumber: number;
  psnSuffix: number;
  isEmployee: boolean;
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
  PayClassification: string;
  gender: string;
  phoneNumber: string;
  workLocation: string;
  receivedContributionsLastYear: boolean;
  fullTimeDate: string;
  terminationReason: string;
  missives: number[] | null;
  allocationFromAmount: number;
  allocationToAmount: number;
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

export interface BalanceByBase<TDetail extends BalanceByDetailBase> extends PagedReportResponse<TDetail> {
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
}

export interface BalanceByAgeDetail extends BalanceByDetailBase {
  age: number;
}

/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface BalanceByAge extends BalanceByBase<BalanceByAgeDetail> {}

/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface BalanceByYears extends BalanceByBase<BalanceByAgeDetail> {}

export interface VestedAmountsByAge extends PagedReportResponse<VestedAmountsByAgeDetail> {
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

export interface TerminationDetail {
  badgeNumber: number;
  psnSuffix: number;
  name: string | null;
  badgePSn: string;
  yearDetails: TerminationYearDetail[];
}

export interface TerminationYearDetail {
  profitYear: number;
  beginningBalance: number;
  beneficiaryAllocation: number;
  distributionAmount: number;
  forfeit: number;
  endingBalance: number;
  vestedBalance: number;
  dateTerm: string | null;
  ytdPsHours: number;
  vestedPercent: number;
  suggestedForfeiture: number | null;
  age: number | null;
  enrollmentCode: number | null;
}

export interface TerminationResponse extends PagedReportResponse<TerminationDetail> {
  totalVested: number;
  totalForfeit: number;
  totalEndingBalance: number;
  totalBeneficiaryAllocation: number;
}

export interface ProfitShareUpdateRequest {
  profitYear: number;
  contributionPercent: number;
  earningsPercent: number;
  incomingForfeitPercent: number;
  secondaryEarningsPercent: number;
  maxAllowedContributions: number;
  badgeToAdjust: number;
  adjustContributionAmount: number;
  adjustEarningsAmount: number;
  adjustIncomingForfeitAmount: number;
  badgeToAdjust2: number;
  adjustEarningsSecondaryAmount: number;
  pagination: SortedPaginationRequestDto;
}

export interface ProfitShareMasterApplyRequest {
  profitYear: number;
  contributionPercent: number;
  earningsPercent: number;
  incomingForfeitPercent: number;
  secondaryEarningsPercent: number;
  maxAllowedContributions: number;
  badgeToAdjust: number;
  adjustContributionAmount: number;
  adjustEarningsAmount: number;
  adjustIncomingForfeitAmount: number;
  badgeToAdjust2: number;
  adjustEarningsSecondaryAmount: number;
}

export interface ProfitShareUpdateDetail {
  isEmployee: boolean;
  badge: number;
  psn: string;
  name: string;
  beginningAmount: number;
  distributions: number;
  military: number;
  xfer: number;
  pxfer: number;
  employeeTypeId: number;
  contributions: number;
  incomingForfeitures: number;
  allEarnings: number;
  allSecondaryEarnings: number;
  endingBalance: number;
  zeroContributionReasonId: number;
  etva: number;
  etvaEarnings: number;
  secondaryEtvaEarnings: number;
  treatAsBeneficiary: boolean;
}

export interface ProfitShareUpdateResponse extends PagedReportResponse<ProfitShareUpdateDetail> {
  totalVested: number;
  totalForfeit: number;
  totalEndingBalance: number;
  totalBeneficiaryAllocation: number;
  hasExceededMaximumContributions: true;
  adjustmentsSummary: ProfitShareAdjustmentSummary;
  profitShareUpdateTotals: ProfitShareUpdateTotals;
}

export interface ProfitShareUpdateTotals {
  beginningBalance: number;
  totalContribution: number;
  earnings: number;
  earnings2: number;
  forfeiture: number;
  distributions: number;
  military: number;
  endingBalance: number;
  allocations: number;
  paidAllocations: number;
  classActionFund: number;
  contributionPoints: number;
  earningPoints: number;
  maxOverTotal: number;
  maxPointsTotal: number;
  totalEmployees: number;
  totalBeneficaries: number;
}

export interface ProfitShareAdjustmentSummary {
  badgeNumber?: number;
  incomingForfeitureAmountUnadjusted: number;
  incomingForfeitureAmountAdjusted: number;
  earningsAmountUnadjusted: number;
  earningsAmountAdjusted: number;
  secondaryEarningsAmountUnadjusted: number;
  secondaryEarningsAmountAdjusted: number;
  contributionAmountUnadjusted: number;
  contributionAmountAdjusted: number;
}

export interface ProfitShareEditDetail {
  isEmployee: boolean;
  badgeNumber: number;
  psn: string;
  name: string;
  code: number;
  contributionAmount: number;
  earningsAmount: number;
  forfeitureAmount: number;
  remark: string;
  commentTypeId: number;
  recordChangeSummary: string;
  zeroContStatus: number;
  yearExtension: number;
}

export interface GrossWagesReportRequest extends ProfitYearRequest {
  minGrossAmount: number;
}

export interface GrossWagesReportDetail {
  badgeNumber: number;
  employeeName: string;
  ssn: string;
  dateOfBirth: string;
  grossWages: number;
  profitSharingAmount: number;
  loans: number;
  forfeitures: number;
  enrollmentId: number;
}

export interface GrossWagesReportResponse extends PagedReportResponse<GrossWagesReportDetail> {
  totalGrossWages: number;
  totalProfitSharingAmount: number;
  totalLoans: number;
  totalForfeitures: number;
}

export interface ProfitShareEditResponse extends PagedReportResponse<ProfitShareEditDetail> {
  beginningBalanceTotal: number;
  contributionGrandTotal: number;
  incomingForfeitureGrandTotal: number;
  earningsGrandTotal: number;
}

export interface ProfitShareMasterResponse {
  reportName: string;
  beneficiariesEffected?: number;
  employeesEffected?: number;
  etvasEffected?: number;
}

export interface ProfitMasterParams {
  adjustContributionAmount?: number | null;
  adjustEarningsAmount?: number | null;
  adjustEarningsSecondaryAmount?: number | null;
  adjustIncomingForfeitAmount?: number | null;
  contributionPercent?: number | null;
  earningsPercent?: number | null;
  incomingForfeitPercent?: number | null;
  maxAllowedContributions?: number | null;
  secondaryEarningsPercent?: number | null;
}

export interface ProfitMasterStatus extends ProfitMasterParams {
  badgeAdjusted?: number | null;
  badgeAdjusted2?: number | null;
  beneficiariesEffected?: number | null;
  contributionPercent?: number | null;
  earningsPercent?: number | null;
  employeesEffected?: number | null;
  etvasEffected?: number | null;
  updatedBy?: string | null;
  updatedTime: string;
}

export interface ProfitShareEditUpdateQueryParams extends ProfitMasterParams {
  profitYear: Date;
  badgeToAdjust?: number | null;
  badgeToAdjust2?: number | null;
}

export interface YearEndProfitSharingReportResponse extends PagedReportResponse<YearEndProfitSharingEmployee> {
  wagesTotal: number;
  hoursTotal: number;
  pointsTotal: number;
  balanceTotal: number;
  terminatedWagesTotal: number;
  terminatedHoursTotal: number;
  terminatedPointsTotal: number;
  terminatedBalanceTotal: number;
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
  terminatedWagesTotal: number;
  terminatedHoursTotal: number;
  terminatedPointsTotal: number;
  terminatedBalanceTotal: number;
  numberOfEmployees: number;
  numberOfNewEmployees: number;
  numberOfEmployeesUnder21: number;
  numberOfEmployeesInPlan: number;
}

export interface FrozenStateResponse {
  id: number;
  profitYear: number;
  frozenBy: string;
  asOfDateTime: string;
  createdDateTime: string;
  isActive: boolean;
}

export interface FreezeDemographicsRequest {
  asOfDateTime: string;
  profitYear: number;
}

export interface MilitaryContributionRequest extends ProfitYearRequest {
  badgeNumber: number;
  contributionAmount: number;
  contributionDate: string;
  pagination: SortedPaginationRequestDto;
}

export interface YearEndProfitSharingReportRequest extends FilterParams {
  profitYear: number;
  pagination: SortedPaginationRequestDto;
}

export interface CreateMilitaryContributionRequest extends ProfitYearRequest {
  badgeNumber: number;
  contributionAmount: number;
  contributionDate: Date;
  isSupplementalContribution: boolean;
}

export interface MilitaryContribution {
  contributionDate: Date | null;
  contributionAmount: number | null;
  isSupplementalContribution: boolean | false;
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
  terminationDate: Date | null;
}

export interface BreakdownByStoreRequest extends ProfitYearRequest {
  storeNumber?: number;
  storeManagement?: boolean;
  badgeNumber?: number;
  employeeName?: string;
  pagination: SortedPaginationRequestDto;
}

export interface BreakdownByStoreEmployee {
  storeNumber: number;
  enrollmentId: number;
  badgeNumber: number;
  ssn: string;
  fullName: string;
  payFrequencyId: number;
  departmentId: number;
  payClassificationId: number;
  beginningBalance: number;
  earnings: number;
  contributions: number;
  forfeitures: number;
  distributions: number;
  endingBalance: number;
  vestedAmount: number;
  vestedPercentage: number;
  employmentStatusId: string;
  payClassificationName: string;
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

export interface Under21BreakdownByStoreResponse extends PagedReportResponse<Under21BreakdownByStoreEmployee> {}

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

export interface Under21InactiveResponse extends PagedReportResponse<Under21InactiveEmployee> {}

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

export interface ProfitSharingLabelsRequest extends FrozenProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface ProfitSharingLabel {
  storeNumber: number;
  payClassificationId: number;
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

export interface YearEndProfitSharingReportSummaryLineItem {
  subgroup: string;
  lineItemPrefix: string;
  lineItemTitle: string;
  numberOfMembers: number;
  totalWages: number;
  totalBalance: number;
}

export interface YearEndProfitSharingReportSummaryResponse {
  lineItems: YearEndProfitSharingReportSummaryLineItem[];
}

export interface UpdateSummaryRequest extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface UpdateSummaryEmployee {
  badgeNumber: number;
  storeNumber: number;
  psnSuffix: number;
  name: string;
  isEmployee: boolean;
  before: {
    profitSharingAmount: number;
    vestedProfitSharingAmount: number;
    yearsInPlan: number;
    enrollmentId: number;
  };
  after: {
    profitSharingAmount: number;
    vestedProfitSharingAmount: number;
    yearsInPlan: number;
    enrollmentId: number;
  };
}

export interface UpdateSummaryResponse extends PagedReportResponse<UpdateSummaryEmployee> {
  totalNumberOfEmployees: number;
  totalNumberOfBeneficiaries: number;
  totalBeforeProfitSharingAmount: number;
  totalBeforeVestedAmount: number;
  totalAfterProfitSharingAmount: number;
  totalAfterVestedAmount: number;
}

export interface SuggestForfeitureAdjustmentRequest extends SortedPaginationRequestDto {
  ssn?: string;
  badge?: string;
  profitYear: number;
}

export interface ForfeitureAdjustmentUpdateRequest {
  badgeNumber: number;
  forfeitureAmount: number;
  classAction: boolean;
  profitYear: number;
}

export interface ForfeitureAdjustmentDetail {
  demographicId: number;
  badgeNumber: number;
  startingBalance: number;
  forfeitureAmount: number;
  netBalance: number;
  netVested: number;
}

export interface SuggestedForfeitResponse {
  badgeNumber: number,
  suggestedForfeitAmount: number;
}


export interface MissiveResponse {
  id: number;
  message: string;
  description: string;
  severity: string;
}

export interface RowCountResult {
  tableName: string;
  rowCount: number;
}

export interface CurrentUserResponseDto {
  userName?: string;
  email?: string;
  storeId?: number;
  isHQUser: boolean;
  claims: string[];
  permissions: string[];
}

export interface NavigationRequestDto {
  navigationId?: number;
}

export interface NavigationResponseDto {
  navigation: NavigationDto[];
}

export interface NavigationDto {
  id: number;
  parentId: number;
  title: string;
  subTitle: string;
  url: string;
  statusId?: number;
  statusName?: string;
  orderNumber: number;
  icon: string;
  requiredRoles: string[];
  disabled: boolean;
  items: NavigationDto[];
}

export interface FilterParams {
  reportId: number;
  badgeNumber?: number | null;
}

export interface ReportPreset {
  id: string;
  name: string;
  description: string;
  params: FilterParams;
}

export interface ControlSheetRequest extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface ControlSheetResponse {
  employeeContributionProfitSharingAmount: number;
  nonEmployeeProfitSharingAmount: number;
  employeeBeneficiaryAmount: number;
  profitSharingAmount: number;
}

export interface NavigationStatusDto {
  id: number;
  name?: string;
}

export interface GetNavigationStatusRequestDto {
  id?: number;
}

export interface GetNavigationStatusResponseDto {
  navigationStatusList?: NavigationStatusDto[];
}

export interface UpdateNavigationRequestDto {
  navigationId?: number;
  statusId?: number;
}

export interface UpdateNavigationResponseDto {
  isSuccessful?: boolean;
}

export interface CurrentNavigation {
  navigationId?: number;
  statusId?: number;
  statusName?: string;
}
export interface ContactInfoDto {
  fullName?: string;
  lastName: string;
  firstName: string;
  middleName?: string;
  phoneNumber?: string;
  mobileNumber?: string;
  emailAddress?: string;
}
export interface AddressDto {
  street?: string;
  street2?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  countryIso?: string;
}
export interface BeneficiaryContactDto {
  id: number;
  ssn: string;
  dateOfBirth: Date;
  address?: AddressDto;
  contactInfo?: ContactInfoDto;
  createdDate: Date;
}
export interface BeneficiaryKindDto {
  id: string;
  name?: string;
}
export interface BeneficiaryDto {
  id: number;
  psnSuffix: number;
  badgeNumber: number;
  demographicId: number;
  psn: string;
  //contact?: BeneficiaryContactDto;
  /**Contact information */
  ssn: string;
  dateOfBirth: Date;
  street: string;
  street2?:string;
  city?: string;
  state?: string;
  postalCode?: string;
  countryIso: string;
  createdDate: Date;
  /**ContactInfo */
  fullName?: string;
  lastName: string;
  firstName: string;
  middleName?: string;
  phoneNumber?: string;
  mobileNumber?: string;
  emailAddress?: string;
  /**End */
  /**End */
  relationship?: string;
  kindId?: number;
  kind?: BeneficiaryKindDto;
  percent: number;
  currentBalance?: number;
}

export interface BeneficiaryResponse {
  beneficiaries: Paged<BeneficiaryDto>,
  beneficiaryOf: Paged<BeneficiaryDto>
}

export interface BeneficiarySearchFilterRequest extends SortedPaginationRequestDto {
  badgeNumber?: number,
  psnSuffix?: number,
  name?: string,
  ssn?: string, 
  memberType?: number
}

export interface BeneficiarySearchFilterResponse {
  badgeNumber: number,
  psnSuffix: number,
  name?: string,
  ssn?: string,
  street?: string,
  city?: string,
  state?:string,
  zip?: string,
  age?:string
}

export interface BeneficiaryRequestDto extends SortedPaginationRequestDto {
  badgeNumber?: number;
  psnSuffix?: number;
}

export interface BeneficiaryResponseDto {
  beneficiaryList?: Paged<BeneficiaryDto>;
}

export interface CreateBeneficiaryRequest {
  beneficiaryContactId: number;
  employeeBadgeNumber: number;
  firstLevelBeneficiaryNumber: number | null;
  secondLevelBeneficiaryNumber: number | null;
  thirdLevelBeneficiaryNumber: number | null;
  relationship: string;
  kindId: string;
  //percentage: number;
}

export interface CreateBeneficiaryResponse {
  beneficiaryId: number;
  psnSuffix: number;
  employeeBadgeNumber: number;
  demographicId: number;
  beneficiaryContactId: number;
  relationship: string | null;
  kindId: string | null;
  percent: number;
}

export interface UpdateBeneficiaryRequest extends UpdateBeneficiaryContactRequest {
  relationship?: string;
  kindId?: string;
  percentage?: number;
}

export interface UpdateBeneficiaryResponse {
  badgeNumber: number;
  demographicId: number;
  beneficiaryContactId: number;
  relationship: string | null;
  kindId: string | null;
  percent: number;
}

export interface DeleteBeneficiaryRequest {
  id: number;
}

export interface UpdateBeneficiaryContactRequest {
  id?: number;
  contactSsn?: number;
  dateOfBirth?: string;
  street1?: string;
  street2?: string | null;
  street3?: string | null;
  street4?: string | null;
  city?: string;
  state?: string;
  postalCode?: string;
  countryIso?: string | null;
  firstName?: string;
  lastName?: string;
  middleName?: string | null;
  phoneNumber?: string | null;
  mobileNumber?: string | null;
  emailAddress?: string | null;
}

export interface CreateBeneficiaryContactRequest {
  contactSsn: number;
  dateOfBirth: string;
  street: string;
  street2: string | null;
  street3: string | null;
  street4: string | null;
  city: string;
  state: string;
  postalCode: string;
  countryIso: string | null;
  firstName: string;
  lastName: string;
  middleName: string | null;
  phoneNumber: string | null;
  mobileNumber: string | null;
  emailAddress: string | null;
}

export interface CreateBeneficiaryContactResponse {
  id: number;
  ssn: string;
  dateOfBirth: string;
  street: string;
  street2: string | null;
  street3: string | null;
  street4: string | null;
  city: string;
  state: string;
  postalCode: string;
  countryIso: string | null;
  firstName: string;
  lastName: string;
  middleName: string | null;
  phoneNumber: string | null;
  mobileNumber: string | null;
  emailAddress: string | null;
}

export interface BeneficiaryTypeDto {
  id: number;
  name?: string;
}

export interface BeneficiaryTypesRequestDto {
  id?: number;
}
export interface BeneficiaryTypesResponseDto {
  beneficiaryTypeList?: BeneficiaryTypeDto[];
}

export interface BeneficiaryKindDto {
  id: string;
  name?: string;
}

export interface BeneficiaryKindRequestDto {
  id?: number;
}
export interface BeneficiaryKindResponseDto {
  beneficiaryKindList?: BeneficiaryKindDto[];
}

export interface BeneficiaryDetailRequest{
  badgeNumber: number;
  psnSuffix?: number;
}

export interface BeneficiaryDetailResponse {
  badgeNumber: number;
  psnSuffix: number;
  name?: string;
  ssn? : string;
  street? : string;
  city?: string;
  state?: string;
  zip? : string;
  dateOfBirth : string;
  currentBalance? : number;
}

// New types for rehire forfeitures editing functionality
export interface RehireForfeituresEditedValues {
  [rowKey: string]: {
    value: number;
    hasError: boolean;
  };
}

export interface RehireForfeituresHeaderComponentProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[]) => Promise<void>;
}

export interface RehireForfeituresSaveButtonCellParams extends ICellRendererParams {
  removeRowFromSelectedRows: (id: number) => void;
  addRowToSelectedRows: (id: number) => void;
  onSave?: (request: ForfeitureAdjustmentUpdateRequest) => Promise<void>;
}

export interface RehireForfeituresUpdatePayload {
  badgeNumber: number;
  profitYear: number;
  suggestedForfeit: number;
}

export interface RehireForfeituresSelectedRow {
  id: number;
  badgeNumber: number;
  profitYear: number;
  suggestedForfeit: number;
}

export interface ApiResponse<T> {}

export interface AdhocBeneficiariesReportRequest extends ProfitYearRequest, SortedPaginationRequestDto {
  isAlsoEmployee: boolean;
}
export interface ProfitDetailDto {
  year: number;
  code: string;
  contributions: number;
  earnings: number;
  forfeitures: number;
  date: Date;
  comments?: string;
}
export interface BeneficiaryReportDto {
  beneficiaryId: number;
  fullName: string;
  ssn: string;
  relationship?: string;
  balance?: string;
  badgeNumber: number;
  psnSuffix: number;
  profitDetails?: ProfitDetailDto[];
}
export interface adhocBeneficiariesReportResponse extends PagedReportResponse<BeneficiaryReportDto> {
  totalEndingBalance: number;
}

export interface PayBenReportRequest extends SortedPaginationRequestDto {
  id?: number;
}
export interface PayBenReport {
  ssn: string;
  beneficiaryFullName: string;
  psn: string;
  badge: number;
  demographicFullName: string;
  percentage: number;
}
export interface PayBenReportResponse extends Paged<PayBenReport> {}
