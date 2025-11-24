import { PaginationParams } from "smart-ui-library";
import type { PagedReportResponse, ProfitYearRequest, SortedPaginationRequestDto } from "../common/api";
import type { FrozenReportsByAgeRequestType } from "../common/enums";

export interface ReportsByAgeParams extends ProfitYearRequest {
  reportType: FrozenReportsByAgeRequestType;
}

export interface FrozenReportsByAgeRequest extends ProfitYearRequest {
  pagination: PaginationParams;
  reportType: FrozenReportsByAgeRequestType;
}

export interface FrozenReportsForfeituresAndPointsRequest extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
  useFrozenData: boolean;
}

export interface ForfeituresAndPointsQueryParams extends ProfitYearRequest {
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
  age: string;
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
  age: string;
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

export interface ForfeituresAndPointsResponse extends PagedReportResponse<ForfeituresAndPointsDetail> {
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
  age: string;
  employeeCount: number;
  amount: number;
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
  age: string;
}
// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface BalanceByAge extends BalanceByBase<BalanceByAgeDetail> {
  // BalanceByAge extends BalanceByBase with BalanceByAgeDetail
}

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface BalanceByYears extends BalanceByBase<BalanceByAgeDetail> {
  // BalanceByYears extends BalanceByBase with BalanceByAgeDetail
}

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
  age: string;
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
