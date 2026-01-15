import type { PagedReportResponse, ProfitYearRequest, SortedPaginationRequestDto } from "../common/api";
import type { MasterUpdateCrossReferenceValidationResponse } from "../validation/cross-reference-validation";

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
  crossReferenceValidation?: MasterUpdateCrossReferenceValidationResponse;
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
  crossReferenceValidation?: MasterUpdateCrossReferenceValidationResponse;
  updatedBy?: string | null;
  updatedTime?: string | null;
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

export interface GrossWagesReportRequest extends ProfitYearRequest {
  minGrossAmount: number;
}

export interface GrossWagesReportDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
  minGrossAmount?: number;
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

export interface UpdateSummaryRequest extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface UpdateSummaryEmployee {
  badgeNumber: number;
  storeNumber: number;
  psnSuffix: number;
  fullName: string;
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

export interface ControlSheetRequest extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface ControlSheetResponse {
  employeeContributionProfitSharingAmount: number;
  nonEmployeeProfitSharingAmount: number;
  employeeBeneficiaryAmount: number;
  profitSharingAmount: number;
}

export interface ProfitShareEditUpdateSearch {
  profitYear: Date;
  contributionPercent?: number | null | undefined;
  earningsPercent?: number | null | undefined;
  secondaryEarningsPercent?: number | null | undefined;
  incomingForfeitPercent?: number | null | undefined;
  maxAllowedContributions: number | null | undefined;
  badgeToAdjust?: number | null | undefined;
  adjustContributionAmount?: number | null | undefined;
  adjustEarningsAmount?: number | null | undefined;
  adjustIncomingForfeitAmount?: number | null | undefined;
  badgeToAdjust2?: number | null | undefined;
  adjustEarningsSecondaryAmount?: number | null | undefined;
}

export interface GrossReportParams {
  profitYear: number;
  gross?: number;
}

export interface UpdateEnrollmentRequest {
  profitYear: number;
}
